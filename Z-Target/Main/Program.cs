using System.Security.Cryptography.X509Certificates;
using Raylib_cs;

class Program
{
    static void Main(string[] args)
    {

        Raylib.InitWindow(GameConfig.ScreenWidth, GameConfig.ScreenHeight, "test");
        Raylib.SetTargetFPS(GameConfig.TargetFPS);

        GameController gameController = new GameController();

        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();
            
            
            gameController.update(deltaTime);
            gameController.Draw();

        }

        Raylib.CloseWindow();
    }
}
