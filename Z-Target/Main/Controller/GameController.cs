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
    private BoiteController boiteController;
    private Boite boite;
    
    
    public GameController()
    {
        // S'abonner aux événements de changement d'état
        GameState.Instance.OnStateChanged += OnGameStateChanged;
        
        // Initialisation du jeu
        InitGame();
        
        //settings pour la cam
        camera = new Camera2D();
        camera.Offset = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);
        camera.Rotation = 0f;
        camera.Zoom = 1f;
    }
    
    private void OnGameStateChanged(GameStateType newState)
    {
        // Actions à effectuer en fonction du changement d'état
        switch (newState)
        {
            case GameStateType.Playing:
                // Reprendre le jeu ou autres actions
                break;
                
            case GameStateType.GameOver:
                // Actions spécifiques au game over
                break;
                
            case GameStateType.Paused:
                // Actions lors de la mise en pause
                break;
                
            case GameStateType.MainMenu:
                // Actions pour revenir au menu
                break;
        }
    }
    
    private void InitGame()
    {
        player = new Player();
        boite = new Boite();
        zombieController = new ZombieController(player);
        playerController = new PlayerController(player, zombieController, view.playerView);
        bulletController = new BulletController(player); 
        boiteController = new BoiteController(boite,player);
        view = new GameView(player, boite);
        
        // Démarrer en mode jeu actif
        GameState.Instance.StartGame();
    }

    public void update(float dt)
    {
        // Vérifier l'état du jeu via GameState
        if (GameState.Instance.IsPlaying())
        {
            // Mise à jour de la logique du jeu
            playerController.Update(dt, camera); 
            zombieController.Update(dt, player);
            bulletController.Update(dt);
            BonusController.Instance.Update(dt, player);
            boiteController.Update(dt);
            // Vérifier si le joueur est toujours en vie
            if (!player.IsAlive())
            {
                // Changer l'état en game over
                GameState.Instance.GameOver();
            }
            
            // Vérifier la touche d'échap pour mettre en pause
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                GameState.Instance.PauseGame();
            }
        }
        else if (GameState.Instance.IsPaused())
        {
            // Gestion de la pause
            if (Raylib.IsKeyPressed(KeyboardKey.Escape) || Raylib.IsKeyPressed(KeyboardKey.P))
            {
                GameState.Instance.ResumeGame();
            }
        }
        else if (GameState.Instance.IsGameOver())
        {
            // Gestion de l'état Game Over
            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                RestartGame();
            }
        }

        camera.Target = player.Position;
    }
    
    private void RestartGame()
    {
        // Réinitialiser le joueur
        player = new Player();
        playerController = new PlayerController(player, zombieController, view.playerView);
        
        // Réinitialiser les zombies
        zombieController = new ZombieController(player);
        
        // Réinitialiser les balles
        bulletController = new BulletController(player);
        
        // Mise à jour de la référence au joueur dans la vue
        view.UpdatePlayer(player);
        
        // Retourner à l'état de jeu
        GameState.Instance.StartGame();
    }

    public void Draw()
    {
        view.Render(camera);
    }
}
