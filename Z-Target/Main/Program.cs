using Raylib_cs;

class Program
{
    static void Main(string[] args)
    {

        Raylib.InitWindow(GameConfig.ScreenWidth, GameConfig.ScreenHeight, GameConfig.WindowTitle);
        Raylib.SetTargetFPS(GameConfig.TargetFPS);

        GameController gameController = new GameController();

        while (!Raylib.WindowShouldClose())
        {
            float deltaTime = Raylib.GetFrameTime();
            
            
            gameController.Update(deltaTime);
            gameController.Draw();

        }

        gameController.Unload();
        Raylib.CloseWindow();
    }
}