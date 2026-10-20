using Raylib_cs;
using System.Numerics;

public class GameView
{
    private const int WALL_THICKNESS = 10;

    List<IView> Views = new List<IView>();

    private HUDView HudView;
    private Texture2D MapTexture;

    public GameView(TextureLoader textures, Player player, MysteryBox mysteryBox, ZombieController zombieController,
                    BulletController bulletController, BonusController bonusController, RoundController roundController)
    {
        Views.Add(new PlayerView(textures, player));
        Views.Add(new ZombieView(textures, player, zombieController));
        Views.Add(new BulletView(bulletController));
        Views.Add(new BonusView(textures, bonusController));
        Views.Add(new MysteryBoxView(textures, mysteryBox));
        HudView = new HUDView(textures, player, roundController);

        // Charger la texture de la carte
        MapTexture = textures.Load(GameConfig.Assets.Map);
    }

    private void DrawMapLimits()
    {
        int offset = GameConfig.Map.PlayAreaOffset;
        int size = GameConfig.Map.PlayAreaSize;
        int wallThickness = WALL_THICKNESS;

        // Mur du haut
        Raylib.DrawRectangle(offset, offset, size, wallThickness, Color.Black);
        
        // Mur du bas
        Raylib.DrawRectangle(offset, offset + size - wallThickness, 
            size, wallThickness, Color.Black);
        
        // Mur de gauche
        Raylib.DrawRectangle(offset, offset, wallThickness, size, Color.Black);
        
        // Mur de droite
        Raylib.DrawRectangle(offset + size - wallThickness, offset, 
            wallThickness, size, Color.Black);
    }

    private void DrawViews()
    {
        foreach (IView view in Views)
            view.Draw();
        
    }

    public void Render(Camera2D camera)
    {
        Raylib.BeginDrawing(); 

        Raylib.ClearBackground(Color.RayWhite);

        Raylib.BeginMode2D(camera);

        // Dessiner la carte
        if (MapTexture.Width > 0 && MapTexture.Height > 0)
        {
            Raylib.DrawTexture(MapTexture, 0, 0, Color.White);
        }

        // Dessiner les limites de la zone de jeu
        DrawMapLimits();

        DrawViews();

        Raylib.EndMode2D();
        
        if (HudView != null)
        {
            HudView.Draw();
        }

        Raylib.EndDrawing();
    }
    
}
