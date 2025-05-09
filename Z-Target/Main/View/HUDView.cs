using Raylib_cs;
using System.Numerics;
using System;
using System.Collections.Generic;

public class HUDView : IView
{
    public static HUDView Instance { get; private set; }
    private Player Player;
    private Random Random = new Random();

    private const float BonusIconSize = 30f;

    // Structure simple pour texte flottant
    private List<(string text, Vector2 position, Vector2 velocity, double spawnTime)> floatingTexts = new();

    public HUDView(Player player)
    {
        this.Player = player;
        if (Instance != null)
            throw new Exception("Une seule instance de HUDView est autorisée.");
        Instance = this;
    }

    public void Draw()
    {
        float deltaTime = Raylib.GetFrameTime();

        // Effet d'écran rouge quand le joueur est touché
        DrawDamageEffect();

        // Affichage des points
        Raylib.DrawText($"{Player.Score}", GameConfig.ScreenWidth - 100, GameConfig.ScreenHeight - 100, 24, Color.Black);

        // Affichage des balles
        Weapon weapon = Player.MainWeapon;
        int textWidth = Raylib.MeasureText($"{Player.MainWeapon.Name}", 24);
        Raylib.DrawText($"{Player.MainWeapon.Name}", GameConfig.ScreenWidth - textWidth - 150, GameConfig.ScreenHeight - 50, 24, Color.Black);
        if(!Player.IsReloading){
            Raylib.DrawText($"{weapon.CurrentAmmo}/{weapon.ReservAmmo}", GameConfig.ScreenWidth - 130, GameConfig.ScreenHeight - 50, 24, Color.Black);
        }
        
        if (Player.IsReloading)
        {
            DrawReloading();
        }

        if(Player.MainWeapon.CurrentAmmo == 0 && Player.MainWeapon.ReservAmmo > 0){
            Raylib.DrawText("Reload", GameConfig.ScreenWidth/2 - 50, GameConfig.ScreenHeight/2 + 70, 24, Color.Red);
        }
        if(Player.MainWeapon.ReservAmmo == 0 && Player.MainWeapon.CurrentAmmo == 0){
            NoAmmo();
        }
        
        // Afficher l'indicateur de sprint et la barre d'endurance
        DrawSprintBar();

        DrawFloatingTexts(deltaTime);
        
        DrawActiveBonus();

        DrawRound();
        
        // Afficher l'écran de game over si le joueur est mort
        if (!Player.IsAlive())
        {
            DrawGameOver();
        }
    }

    public void DrawRound()
    {
        Raylib.DrawText($"Round {RoundController.Instance.CurrentRound + 1}", GameConfig.ScreenWidth - 150, 10, 24, Color.Black);
        Raylib.DrawText($"Zombies left: {RoundController.Instance.zombiesToSpawn - RoundController.Instance.zombiesKilled}", GameConfig.ScreenWidth - 225, 40, 24, Color.Black);

        // Afficher le compte à rebours si on est en transition
        if (RoundController.Instance.isRoundTransitioning)
        {
            string countdownText = $"Next round in {Math.Ceiling(RoundController.Instance.roundTransitionTimer)}s";
            int textWidth = Raylib.MeasureText(countdownText, 30);
            Raylib.DrawText(countdownText, GameConfig.ScreenWidth/2 - textWidth/2, GameConfig.ScreenHeight - 100, 30, Color.Red);
        }
    }

    public void DrawReloading()
    {
        Raylib.DrawCircleSector(
            new Vector2(GameConfig.ScreenWidth - 80, GameConfig.ScreenHeight - 45),20,0,360 * (Player.MainWeapon.ReloadTimer / Player.MainWeapon.ReloadTime),60,Color.DarkGray
        );
    }

    private void DrawSprintBar()
    {
        // Position et taille de la barre de sprint
        int barWidth = 150;
        int barHeight = 15;
        int barX = 20;
        int barY = 20;
        
        // Dessiner le fond de la barre (gris)
        Raylib.DrawRectangle(barX, barY, barWidth, barHeight, Color.Gray);
        
        // Calculer la largeur de la barre d'endurance actuelle
        float sprintRatio = Player.GetSprintRatio();
        int currentBarWidth = (int)(barWidth * sprintRatio);
        
        // Choisir une couleur en fonction de l'état de l'endurance
        Color barColor;
        if (sprintRatio > 0.6f)
            barColor = Color.Green;
        else if (sprintRatio > 0.3f)
            barColor = Color.Yellow;
        else
            barColor = Color.Red;
        
        // Dessiner la barre d'endurance actuelle
        Raylib.DrawRectangle(barX, barY, currentBarWidth, barHeight, barColor);
        
        // Dessiner le contour de la barre
        Raylib.DrawRectangleLines(barX, barY, barWidth, barHeight, Color.Black);
        
        // Afficher un texte si le sprint est actif
        if (Player.IsSprinting())
        {
            Raylib.DrawText("SPRINT", barX + barWidth + 10, barY, 20, Color.SkyBlue);
        }
    }

    private void DrawDamageEffect()
    {
        // Calculer l'intensité de l'effet rouge en fonction de la santé actuelle
        float healthRatio = Player.GetHealthRatio();
        
        // sante basse plus rouge
        int baseAlpha = (int)(200 * (1.0f - healthRatio)); // Base alpha between 0-200
        
        // Si le joueur est invincible (vient d'être touché), augmenter l'intensité
        if (Player.IsInvincible())
        {
            // prise de degats plus rouge
            float pulseFactor = 0.8f + 0.2f * (float)Math.Sin(Raylib.GetTime() * 15.0);
            int pulseAlpha = (int)(255 * pulseFactor);
            
            // Draw rectangle rouge pas opaque 
            Raylib.DrawRectangle(0, 0, GameConfig.ScreenWidth, GameConfig.ScreenHeight, 
                               new Color(255, 0, 0, Math.Min(pulseAlpha, 220)));
        }
        else
        {
            // Effet rouge plus subtil en fonction de la santé actuelle
            Raylib.DrawRectangle(0, 0, GameConfig.ScreenWidth, GameConfig.ScreenHeight, 
                               new Color(255, 0, 0, baseAlpha));
        }
    }

    public void AddFloatingPoints(int points)
    {
        // Crée un angle aléatoire entre -60° et +60° (en radians)
        float angle = (float)(Random.NextDouble() * Math.PI / 3 - Math.PI / 6); // entre -60° et +60°
        float speed = -40f;
        Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

        floatingTexts.Add(($"+{points}", new Vector2(GameConfig.ScreenWidth - 150, GameConfig.ScreenHeight - 100), velocity, Raylib.GetTime()));
    }

    private void DrawFloatingTexts(float deltaTime)
    {
        double currentTime = Raylib.GetTime();

        for (int i = floatingTexts.Count - 1; i >= 0; i--)
        {
            (string text, Vector2 position, Vector2 velocity, double spawnTime) = floatingTexts[i];

            if (currentTime - spawnTime > 0.5)
            {
                floatingTexts.RemoveAt(i);
                continue;
            }

            Vector2 newPosition = position + velocity * deltaTime;
            floatingTexts[i] = (text, newPosition, velocity, spawnTime);

            Raylib.DrawText(text, (int)newPosition.X, (int)newPosition.Y, 20, Color.Red);
        }
    }
    private void DrawActiveBonus()
    {
        List<BonusType> activeBonuses = Player.GetActiveBonuses();
        if (activeBonuses.Count > 0)
        {
            // Position de départ pour le premier bonus
            int startY = GameConfig.ScreenHeight - 50;
            int spacing = 40; // Espacement vertical entre les bonus
            
            for (int i = 0; i < activeBonuses.Count; i++)
            {
                BonusType bonusType = activeBonuses[i];
                Color color = GetBonusColor(bonusType);
                
                // Position verticale basée sur l'index du bonus
                int posY = startY - (i * spacing);
                
                // Dessiner le carré coloré
                Raylib.DrawRectangle(10, posY, (int)BonusIconSize, (int)BonusIconSize, color);
                
                // Récupérer le temps restant pour le bonus
                float remainingTime = Player.GetBonusRemainingTime(bonusType);
                
                // Afficher le nom du bonus et le temps restant
                Raylib.DrawText($"{bonusType} ({remainingTime:F1}s)", 10 + (int)BonusIconSize + 10, posY, 20, Color.Black);
            }
        }
    }

    private Color GetBonusColor(BonusType type)
    {
        return type switch
        {
            BonusType.MaxAmmo => Color.SkyBlue,
            BonusType.InstaKill => Color.Red,
            BonusType.DoublePoints => Color.Gold,
            BonusType.Nuke => Color.Green,
            _ => Color.Gray,
        };
    }

    public void NoAmmo(){
        Raylib.DrawText("No Ammo", GameConfig.ScreenWidth/2 - 50, GameConfig.ScreenHeight/2 + 70, 24, Color.Red);
    }

    public void DrawGameOver()
    {
        // Afficher un message de game over
        string gameOverText = "GAME OVER";
        int fontSize = 60;
        
        // Mesurer la taille du texte pour le centrer
        int textWidth = Raylib.MeasureText(gameOverText, fontSize);
        int textX = (GameConfig.ScreenWidth - textWidth) / 2;
        int textY = GameConfig.ScreenHeight / 2 - fontSize;
        
        // Dessiner le texte avec une ombre
        Raylib.DrawText(gameOverText, textX + 3, textY + 3, fontSize, new Color(0, 0, 0, 150));
        Raylib.DrawText(gameOverText, textX, textY, fontSize, Color.Red);
        
        // Instructions pour quitter
        string quitText = "Appuyez sur R pour quitter";
        int quitFontSize = 30;
        
        // Mesurer la taille du texte pour le centrer
        int quitTextWidth = Raylib.MeasureText(quitText, quitFontSize);
        int quitTextX = (GameConfig.ScreenWidth - quitTextWidth) / 2;
        int quitTextY = textY + fontSize + 20;
        
        // Dessiner le texte avec une ombre
        Raylib.DrawText(quitText, quitTextX + 2, quitTextY + 2, quitFontSize, new Color(0, 0, 0, 150));
        Raylib.DrawText(quitText, quitTextX, quitTextY, quitFontSize, Color.White);
    }
}
