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
        playerController = new PlayerController(player);
        zombieController = new ZombieController(player);
        bulletController = new BulletController(player); 
        
        view = new GameView(player); // Instanciation de GameView avec le zombie
        
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
    }
    public void Draw()
    {
        view.Render(camera);
    }
}
