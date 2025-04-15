using Raylib_cs;
using System.Numerics;
public class GameController
{
    private Player player;
    private GameView view;
    private Camera2D camera;
    private PlayerController playerController; 
    private ZombieController zombieController;
    private BulletController bulletController;
    
    
    public GameController()
    {
        player = new Player();
        zombieController = new ZombieController();
        playerController = new PlayerController(player, zombieController);
        bulletController = new BulletController(player); 
        
        view = new GameView(player, zombieController); // Instanciation de GameView avec le zombie
        
        //settings pour la cam
        camera = new Camera2D();
        camera.Offset = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);
        camera.Rotation = 0f;
        camera.Zoom = 1f;
    }

    public void update(float dt)
    {
        playerController.Update(dt,camera); 
        zombieController.Update(dt,player);
        bulletController.Update(dt);

        camera.Target = player.Position;

        // MOUVEMENT
        player.Direction = Vector2.Zero;
        if (Raylib.IsKeyDown(KeyboardKey.W) || Raylib.IsKeyDown(KeyboardKey.Up)) player.Direction.Y -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.S) || Raylib.IsKeyDown(KeyboardKey.Down)) player.Direction.Y += 1;
        if (Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left)) player.Direction.X -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right)) player.Direction.X += 1;
    }
    public void Draw()
    {
        view.Render(camera);
    }
}
