using Raylib_cs;
using System.Numerics;
using System;
using System.Collections.Generic;

public class HUDView : IView
{
    public static HUDView Instance { get; private set; }
    private Player player;
    private Random random = new Random();

    // Structure simple pour texte flottant
    private List<(string text, Vector2 position, Vector2 velocity, double spawnTime)> floatingTexts = new();

    public HUDView(Player player)
    {
        this.player = player;
        if (Instance != null)
            throw new Exception("Une seule instance de HUDView est autorisée.");
        Instance = this;
    }

    public void Draw()
    {
        float deltaTime = Raylib.GetFrameTime();

        // Affichage des points
        Raylib.DrawText($"{player.Score}", GameConfig.ScreenWidth - 100, GameConfig.ScreenHeight - 100, 24, Color.Black);

        // Affichage des balles
        Weapon weapon = player.mainWeapon;
        int textWidth = Raylib.MeasureText($"{player.mainWeapon.name}", 24);
        Raylib.DrawText($"{player.mainWeapon.name}", GameConfig.ScreenWidth - textWidth - 150, GameConfig.ScreenHeight - 50, 24, Color.Black);
        if(!player.isReloading){
            Raylib.DrawText($"{weapon.currentAmmo}/{weapon.maxAmmo}", GameConfig.ScreenWidth - 130, GameConfig.ScreenHeight - 50, 24, Color.Black);
        }
        
        if (player.isReloading)
        {
            DrawReloading();
        }

        DrawFloatingTexts(deltaTime);
    }

    public void DrawReloading()
    {
        Raylib.DrawCircleSector(
            new Vector2(GameConfig.ScreenWidth - 80, GameConfig.ScreenHeight - 45),20,0,360 * (player.mainWeapon.reloadTimer / player.mainWeapon.reloadTime),60,Color.DarkGray
        );
    }

    public void AddFloatingPoints(int points)
    {
        // Crée un angle aléatoire entre -60° et +60° (en radians)
        float angle = (float)(random.NextDouble() * Math.PI / 3 - Math.PI / 6); // entre -60° et +60°
        float speed = -40f;
        Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

        floatingTexts.Add(($"+{points}", new Vector2(GameConfig.ScreenWidth - 150, GameConfig.ScreenHeight - 100), velocity, Raylib.GetTime()));
    }

    private void DrawFloatingTexts(float deltaTime)
    {
        double currentTime = Raylib.GetTime();

        for (int i = floatingTexts.Count - 1; i >= 0; i--)
        {
            var (text, position, velocity, spawnTime) = floatingTexts[i];

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

    public void NoAmmo(){
        Raylib.DrawText("No Ammo", GameConfig.ScreenWidth/2 - 50, GameConfig.ScreenHeight/2 + 70, 24, Color.Red);
    }
}
