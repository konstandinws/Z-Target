using Raylib_cs;

class Program
{
    static void Main(string[] args)
    {

        GameConfig.Load();

        Raylib.InitWindow(GameConfig.Window.Width, GameConfig.Window.Height, GameConfig.Window.Title);
        Raylib.SetTargetFPS(GameConfig.Window.TargetFPS);

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