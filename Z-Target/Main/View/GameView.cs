using Raylib_cs;
using System.Data;
using System.Numerics;

public class GameView
{
    List<IView> views = new List<IView>();

    private HUDView hudView;
    private Player player;
    private Boite boite;
    private Texture2D mapTexture;
    
    public GameView(Player player, Boite boite)
    {
        this.player = player;
        this.boite = boite;
        InitializeViews();
        
        // Charger la texture de la carte
        mapTexture = Raylib.LoadTexture("Main/sprites/map.png");
    }

    private void InitializeViews()
    {
        views.Clear();
        views.Add(new PlayerView(player));
        views.Add(new ZombieView(player));
        views.Add(new BulletView());
        views.Add(new BonusView(player));
        hudView = new HUDView(player);
        views.Add(new BoiteView(boite));
    }

    public void AddZombieView()
    {
        views.Add(new ZombieView(player));
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
        foreach (IView view in views)
            view.Draw();
        
    }

    public void UpdatePlayer(Player player)
    {
        this.player = player;
        InitializeViews();
    }
    
    public void Render(Camera2D camera)
    {
        Raylib.BeginDrawing(); 

        Raylib.ClearBackground(Color.RayWhite);

        Raylib.BeginMode2D(camera);

        // Dessiner la carte
        if (mapTexture.Width > 0 && mapTexture.Height > 0)
        {
            Raylib.DrawTexture(mapTexture, 0, 0, Color.White);
        }

        // Dessiner les limites de la zone de jeu
        DrawMapLimits();

        DrawViews();

        Raylib.EndMode2D();
        
        if (hudView != null)
        {
            hudView.Draw();
        }

        Raylib.EndDrawing();
    }
    
    // private Player GetPlayerFromViews() // pas obligatoire ????
    // {
    //     // Récupérer le joueur depuis la PlayerView
    //     foreach (var view in views)
    //     {
    //         if (view is PlayerView playerView)
    //         {
    //             return playerView.GetPlayer();
    //         }
    //     }
        
    //     // Fallback (ne devrait jamais arriver)
    //     return null;
    // }
}
