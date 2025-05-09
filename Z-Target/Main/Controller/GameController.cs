using Raylib_cs;
using System.Numerics;
public class GameController
{
    private Player Player;
    private GameView View;
    private Camera2D Camera;
    private PlayerController PlayerController; 
    private ZombieController ZombieController;
    private BulletController BulletController;
    private BoiteController BoiteController;
    private Boite Boite;
    private RoundController RoundController;
    
    public GameController()
    {
        // Initialisation du jeu
        InitGame();
        
        //settings pour la cam
        Camera = new Camera2D();
        Camera.Offset = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);
        Camera.Rotation = 0f;
        Camera.Zoom = 1f;
    }
    
    private void InitGame()
    {
        Player = new Player();
        Boite = new Boite() { Position = new Vector2(1600, 1000) };
        RoundController = new RoundController();
        ZombieController = new ZombieController(Player, RoundController);
        RoundController.SetZombieController(ZombieController);

        PlayerController = new PlayerController(Player, ZombieController);
        BulletController = new BulletController(Player); 
        BoiteController = new BoiteController(Boite,Player);
        View = new GameView(Player, Boite);
    }

    public void Update(float dt)
    {
        // Si le joueur est mort, ne faire que la vérification de la touche R
        if (!Player.IsAlive())
        {
            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                Raylib.CloseWindow();
            }
            return;
        }

        // Mise à jour de la logique du jeu seulement si le joueur est en vie
        PlayerController.Update(dt, Camera); 
        ZombieController.Update(dt, Player);
        BulletController.Update(dt);
        BonusController.Instance.Update(dt, Player);
        BoiteController.Update(dt);
        RoundController.Update(dt);

        Camera.Target = Player.Position;
    }
    
    public void Draw()
    {
        View.Render(Camera);
    }
}
