using Raylib_cs;
using System.Data;
using System.Numerics;

public class GameView
{
    List<IView> views = new List<IView>();

    // private PlayerView playerView;
    // private ZombieView zombieView;
    // private BulletView bulletView;
    private HUDView hudView;

    
    public GameView(Player player)
    {
        // playerView = new PlayerView(player);
        // zombieView = new ZombieView();
        // bulletView = new BulletView();
        views.Add(new PlayerView(player));
        views.Add(new ZombieView());
        views.Add(new BulletView());
        views.Add(new BonusView(player));
        hudView = new HUDView(player);
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

    private void DrawViews()
    {
        foreach (var view in views)
            view.Draw();
        
    }

    public void UpdatePlayer(Player player)
    {
        // Recréer les vues avec le nouveau joueur
        views.Clear();
        views.Add(new PlayerView(player));
        views.Add(new ZombieView());
        views.Add(new BulletView());
        views.Add(new BonusView(player));
        
        // Créer une nouvelle instance de HUDView
        hudView = new HUDView(player);
    }
    
    public void Render(Camera2D camera)
    {
        Raylib.BeginDrawing(); 

        Raylib.ClearBackground(Color.RayWhite);

        Raylib.BeginMode2D(camera); // dans mode 2d ca bouge pas hors ca bouge avec le joueur

        // Draw a grid
        DrawGrid(50);

        DrawViews();

        Raylib.EndMode2D();
        hudView.Draw();

        Raylib.EndDrawing();
    }
    
    private Player GetPlayerFromViews()
    {
        // Récupérer le joueur depuis la PlayerView
        foreach (var view in views)
        {
            if (view is PlayerView playerView)
            {
                return playerView.GetPlayer();
            }
        }
        
        // Fallback (ne devrait jamais arriver)
        return null;
    }
}
