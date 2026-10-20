using Raylib_cs;
using System.Numerics;
using System;
using System.Collections.Generic;

public class HUDView : IView
{
    private const int W = GameConfig.ScreenWidth;
    private const int H = GameConfig.ScreenHeight;

    // Tailles de police
    private const int FONT_SIZE = 24;
    private const int SMALL_FONT_SIZE = 20;
    private const int COUNTDOWN_FONT_SIZE = 30;
    private const int GAME_OVER_FONT_SIZE = 60;
    private const int GAME_OVER_HINT_FONT_SIZE = 30;

    // Score et arme (en bas à droite)
    private static readonly Vector2 SCORE_POS = new Vector2(W - 100, H - 100);
    private const int WEAPON_ROW_Y = H - 50;
    private const int WEAPON_NAME_RIGHT_X = W - 150; // le nom de l'arme est aligné à droite sur cette position
    private const int AMMO_X = W - 130;
    private static readonly Vector2 RELOAD_CIRCLE_CENTER = new Vector2(W - 80, H - 45);
    private const float RELOAD_CIRCLE_RADIUS = 20f;
    private const int RELOAD_CIRCLE_SEGMENTS = 60;

    // Messages "Reload" / "No Ammo" (sous le joueur)
    private const int AMMO_WARNING_X = W / 2 - 50;
    private const int AMMO_WARNING_Y = H / 2 + 70;

    // Barre de sprint (en haut à gauche)
    private const int SPRINT_BAR_X = 20;
    private const int SPRINT_BAR_Y = 20;
    private const int SPRINT_BAR_WIDTH = 150;
    private const int SPRINT_BAR_HEIGHT = 15;
    private const int SPRINT_LABEL_SPACING = 10;
    private const float SPRINT_HIGH_RATIO = 0.6f;  // au-dessus : vert
    private const float SPRINT_LOW_RATIO = 0.3f;   // au-dessus : jaune, en dessous : rouge

    // Effet rouge quand le joueur est blessé
    private const int DAMAGE_MAX_ALPHA = 200;      // opacité max selon la vie manquante
    private const float HIT_PULSE_SPEED = 15f;
    private const int HIT_MAX_ALPHA = 220;

    // Points flottants "+X"
    private const double FLOATING_TEXT_LIFETIME = 0.5;
    private const float FLOATING_TEXT_SPEED = 40f;
    private const double FLOATING_TEXT_MAX_ANGLE = Math.PI / 6; // ±30° autour de la verticale

    // Bonus actifs (en bas à gauche, empilés vers le haut)
    private const int BONUS_LIST_X = 10;
    private const int BONUS_LIST_START_Y = H - 50;
    private const int BONUS_LIST_SPACING = 40;
    private const int BONUS_ICON_SIZE = 30;
    private const int BONUS_LABEL_SPACING = 10;

    // Manche (en haut à droite)
    private const int ROUND_X = W - 150;
    private const int ROUND_Y = 10;
    private const int ZOMBIES_LEFT_X = W - 225;
    private const int ZOMBIES_LEFT_Y = 40;
    private const int COUNTDOWN_Y = H - 100;

    // Game over
    private const int GAME_OVER_SHADOW_OFFSET = 3;
    private const int GAME_OVER_HINT_SHADOW_OFFSET = 2;
    private const int GAME_OVER_HINT_SPACING = 20;
    private static readonly Color SHADOW_COLOR = new Color(0, 0, 0, 150);

    private Player Player;
    private RoundController RoundController;
    private Random Random = new Random();

    // Structure simple pour texte flottant
    private List<(string text, Vector2 position, Vector2 velocity, double spawnTime)> floatingTexts = new();

    public HUDView(Player player, RoundController roundController)
    {
        this.Player = player;
        this.RoundController = roundController;
        player.PointsEarned += AddFloatingPoints;
    }

    public void Draw()
    {
        float deltaTime = Raylib.GetFrameTime();

        // Effet d'écran rouge quand le joueur est touché
        DrawDamageEffect();

        // Affichage des points
        Raylib.DrawText($"{Player.Score}", (int)SCORE_POS.X, (int)SCORE_POS.Y, FONT_SIZE, Color.Black);

        // Affichage de l'arme et des balles
        Weapon weapon = Player.MainWeapon;
        int textWidth = Raylib.MeasureText(weapon.Name, FONT_SIZE);
        Raylib.DrawText(weapon.Name, WEAPON_NAME_RIGHT_X - textWidth, WEAPON_ROW_Y, FONT_SIZE, Color.Black);

        if (Player.IsReloading)
        {
            DrawReloading();
        }
        else
        {
            Raylib.DrawText($"{weapon.CurrentAmmo}/{weapon.ReserveAmmo}", AMMO_X, WEAPON_ROW_Y, FONT_SIZE, Color.Black);
        }

        if (weapon.CurrentAmmo == 0 && weapon.ReserveAmmo > 0)
        {
            Raylib.DrawText("Reload", AMMO_WARNING_X, AMMO_WARNING_Y, FONT_SIZE, Color.Red);
        }
        if (weapon.ReserveAmmo == 0 && weapon.CurrentAmmo == 0)
        {
            Raylib.DrawText("No Ammo", AMMO_WARNING_X, AMMO_WARNING_Y, FONT_SIZE, Color.Red);
        }

        DrawSprintBar();
        DrawFloatingTexts(deltaTime);
        DrawActiveBonus();
        DrawRound();

        if (!Player.IsAlive)
        {
            DrawGameOver();
        }
    }

    public void DrawRound()
    {
        Raylib.DrawText($"Round {RoundController.CurrentRound}", ROUND_X, ROUND_Y, FONT_SIZE, Color.Black);
        Raylib.DrawText($"Zombies left: {RoundController.zombiesToSpawn - RoundController.zombiesKilled}", ZOMBIES_LEFT_X, ZOMBIES_LEFT_Y, FONT_SIZE, Color.Black);

        // Afficher le compte à rebours si on est en transition
        if (RoundController.isRoundTransitioning)
        {
            string countdownText = $"Next round in {Math.Ceiling(RoundController.roundTransitionTimer)}s";
            int textWidth = Raylib.MeasureText(countdownText, COUNTDOWN_FONT_SIZE);
            Raylib.DrawText(countdownText, W / 2 - textWidth / 2, COUNTDOWN_Y, COUNTDOWN_FONT_SIZE, Color.Red);
        }
    }

    public void DrawReloading()
    {
        float progress = Player.MainWeapon.ReloadTimer / Player.MainWeapon.ReloadTime;
        Raylib.DrawCircleSector(RELOAD_CIRCLE_CENTER, RELOAD_CIRCLE_RADIUS, 0, 360 * progress, RELOAD_CIRCLE_SEGMENTS, Color.DarkGray);
    }

    private void DrawSprintBar()
    {
        // Fond de la barre
        Raylib.DrawRectangle(SPRINT_BAR_X, SPRINT_BAR_Y, SPRINT_BAR_WIDTH, SPRINT_BAR_HEIGHT, Color.Gray);

        float sprintRatio = Player.GetSprintRatio();
        int currentBarWidth = (int)(SPRINT_BAR_WIDTH * sprintRatio);

        // Couleur selon l'endurance restante
        Color barColor;
        if (sprintRatio > SPRINT_HIGH_RATIO)
            barColor = Color.Green;
        else if (sprintRatio > SPRINT_LOW_RATIO)
            barColor = Color.Yellow;
        else
            barColor = Color.Red;

        Raylib.DrawRectangle(SPRINT_BAR_X, SPRINT_BAR_Y, currentBarWidth, SPRINT_BAR_HEIGHT, barColor);
        Raylib.DrawRectangleLines(SPRINT_BAR_X, SPRINT_BAR_Y, SPRINT_BAR_WIDTH, SPRINT_BAR_HEIGHT, Color.Black);

        if (Player.IsSprinting())
        {
            Raylib.DrawText("SPRINT", SPRINT_BAR_X + SPRINT_BAR_WIDTH + SPRINT_LABEL_SPACING, SPRINT_BAR_Y, SMALL_FONT_SIZE, Color.SkyBlue);
        }
    }

    private void DrawDamageEffect()
    {
        int alpha;
        if (Player.IsInvincible())
        {
            // Vient d'être touché : rouge qui pulse fort
            float pulseFactor = 0.8f + 0.2f * (float)Math.Sin(Raylib.GetTime() * HIT_PULSE_SPEED);
            alpha = Math.Min((int)(255 * pulseFactor), HIT_MAX_ALPHA);
        }
        else
        {
            // Rouge plus subtil, d'autant plus fort que la vie est basse
            alpha = (int)(DAMAGE_MAX_ALPHA * (1.0f - Player.GetHealthRatio()));
        }

        Raylib.DrawRectangle(0, 0, W, H, new Color(255, 0, 0, alpha));
    }

    private void AddFloatingPoints(int points)
    {
        // Le texte monte avec un angle aléatoire autour de la verticale
        double angle = Random.NextDouble() * 2 * FLOATING_TEXT_MAX_ANGLE - FLOATING_TEXT_MAX_ANGLE;
        Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * -FLOATING_TEXT_SPEED;

        floatingTexts.Add(($"+{points}", SCORE_POS - new Vector2(50, 0), velocity, Raylib.GetTime()));
    }

    private void DrawFloatingTexts(float deltaTime)
    {
        double currentTime = Raylib.GetTime();

        for (int i = floatingTexts.Count - 1; i >= 0; i--)
        {
            (string text, Vector2 position, Vector2 velocity, double spawnTime) = floatingTexts[i];

            if (currentTime - spawnTime > FLOATING_TEXT_LIFETIME)
            {
                floatingTexts.RemoveAt(i);
                continue;
            }

            Vector2 newPosition = position + velocity * deltaTime;
            floatingTexts[i] = (text, newPosition, velocity, spawnTime);

            Raylib.DrawText(text, (int)newPosition.X, (int)newPosition.Y, SMALL_FONT_SIZE, Color.Red);
        }
    }

    private void DrawActiveBonus()
    {
        List<BonusType> activeBonuses = Player.GetActiveBonuses();

        for (int i = 0; i < activeBonuses.Count; i++)
        {
            BonusType bonusType = activeBonuses[i];
            int posY = BONUS_LIST_START_Y - i * BONUS_LIST_SPACING;

            Raylib.DrawRectangle(BONUS_LIST_X, posY, BONUS_ICON_SIZE, BONUS_ICON_SIZE, BonusView.GetColor(bonusType));

            float remainingTime = Player.GetBonusRemainingTime(bonusType);
            int labelX = BONUS_LIST_X + BONUS_ICON_SIZE + BONUS_LABEL_SPACING;
            Raylib.DrawText($"{bonusType} ({remainingTime:F1}s)", labelX, posY, SMALL_FONT_SIZE, Color.Black);
        }
    }

    public void DrawGameOver()
    {
        string gameOverText = "GAME OVER";
        int textX = (W - Raylib.MeasureText(gameOverText, GAME_OVER_FONT_SIZE)) / 2;
        int textY = H / 2 - GAME_OVER_FONT_SIZE;

        Raylib.DrawText(gameOverText, textX + GAME_OVER_SHADOW_OFFSET, textY + GAME_OVER_SHADOW_OFFSET, GAME_OVER_FONT_SIZE, SHADOW_COLOR);
        Raylib.DrawText(gameOverText, textX, textY, GAME_OVER_FONT_SIZE, Color.Red);

        string hintText = "Press R to play again (Esc to quit)";
        int hintX = (W - Raylib.MeasureText(hintText, GAME_OVER_HINT_FONT_SIZE)) / 2;
        int hintY = textY + GAME_OVER_FONT_SIZE + GAME_OVER_HINT_SPACING;

        Raylib.DrawText(hintText, hintX + GAME_OVER_HINT_SHADOW_OFFSET, hintY + GAME_OVER_HINT_SHADOW_OFFSET, GAME_OVER_HINT_FONT_SIZE, SHADOW_COLOR);
        Raylib.DrawText(hintText, hintX, hintY, GAME_OVER_HINT_FONT_SIZE, Color.White);
    }
}
