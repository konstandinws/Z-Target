using Raylib_cs;
using System.Data;
using System.Numerics;

public class GameView
{
    List<IView> Views = new List<IView>();

    private HUDView HudView;
    private Player Player;
    private Boite Boite;
    private Texture2D MapTexture;
    
    public GameView(Player player, Boite boite)
    {
        this.Player = player;
        this.Boite = boite;
        InitializeViews();
        
        // Charger la texture de la carte
        MapTexture = Raylib.LoadTexture("Main/sprites/map.png");
    }

    private void InitializeViews()
    {
        Views.Clear();
        Views.Add(new PlayerView(Player));
        Views.Add(new ZombieView(Player));
        Views.Add(new BulletView());
        Views.Add(new BonusView(Player));
        Views.Add(new BoiteView(Boite));
        HudView = new HUDView(Player);
    }

    public void AddZombieView()
    {
        Views.Add(new ZombieView(Player));
    }

    public void ResetViews()
    {
        InitializeViews();
    }

    private void DrawGrid(int spacing){
        for (int i = 0; i <= GameConfig.ScreenWidth; i += spacing)
            {
                Raylib.DrawLine(i, 0, i, GameConfig.ScreenHeight, Color.LightGray);
            }
            for (int i = 0; i <= GameConfig.ScreenHeight; i += 50)
            {
                Raylib.DrawLine(0, i, GameConfig.ScreenWidth, i, Color.LightGray);
            }
    }

    private void DrawMapLimits()
    {
        // Calculer les coordonnées des limites de la zone de jeu
        int offset = (GameConfig.MAP_SIZE - GameConfig.PLAY_AREA_SIZE) / 2;
        int wallThickness = 10;

        // Mur du haut
        Raylib.DrawRectangle(offset, offset, GameConfig.PLAY_AREA_SIZE, wallThickness, Color.Black);
        
        // Mur du bas
        Raylib.DrawRectangle(offset, offset + GameConfig.PLAY_AREA_SIZE - wallThickness, 
            GameConfig.PLAY_AREA_SIZE, wallThickness, Color.Black);
        
        // Mur de gauche
        Raylib.DrawRectangle(offset, offset, wallThickness, GameConfig.PLAY_AREA_SIZE, Color.Black);
        
        // Mur de droite
        Raylib.DrawRectangle(offset + GameConfig.PLAY_AREA_SIZE - wallThickness, offset, 
            wallThickness, GameConfig.PLAY_AREA_SIZE, Color.Black);
    }

    private void DrawViews()
    {
        foreach (IView view in Views)
            view.Draw();
        
    }

    public void UpdatePlayer(Player player)
    {
        this.Player = player;
        InitializeViews();
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
