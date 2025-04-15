using Raylib_cs;
using System.Numerics;
public class GameController
{
    private Player player;
    private GameView view;
    private PlayerController playerController; 
    private Camera2D camera;
     // soit je met en static soit je met la listzombie dans zombiecontroller

    private ZombieController zombieController;
    
    
    public GameController()
    {
        player = new Player();
        playerController = new PlayerController(player);
        zombieController = new ZombieController();
        
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

        camera.Target = player.Position;
    }
    public void Draw()
    {
        view.Render(camera);
    }
}
