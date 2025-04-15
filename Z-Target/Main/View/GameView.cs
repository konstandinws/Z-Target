using Raylib_cs;
using System.Numerics;

public class GameView
{

    private PlayerView playerView;
    private ZombieView zombieView;
    private BulletView bulletView;
    public GameView(Player player)
    {
        playerView = new PlayerView(player);
        zombieView = new ZombieView();
        bulletView = new BulletView(); // Instanciation de BulletView
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
    public void Render(Camera2D camera)
    {
        Raylib.BeginDrawing(); 

        Raylib.ClearBackground(Color.RayWhite);

        Raylib.BeginMode2D(camera); // dans mode 2d ca bouge pas hors ca bouge avec le joueur

        // Draw a grid
        DrawGrid(50);

        playerView.Draw();
        zombieView.Draw();
        bulletView.Draw(BulletController.Instance.BulletList); 



        // Raylib.DrawText("test 2D",20,20,20,Color.Red);

        Raylib.EndMode2D();

        // Raylib.DrawText(Raylib.GetFPS().ToString(),20,60,20,Color.Red);


        Raylib.EndDrawing();
    }
}
