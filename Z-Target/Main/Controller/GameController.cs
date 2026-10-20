using Raylib_cs;
using System.Numerics;
using System.Diagnostics.CodeAnalysis;
public class GameController
{
    // Gardé d'une partie à l'autre pour ne pas recharger les images à chaque fois
    private readonly TextureLoader Textures = new TextureLoader();

    private Player Player;
    private GameView View;
    private Camera2D Camera;
    private PlayerController PlayerController;
    private ZombieController ZombieController;
    private BulletController BulletController;
    private BonusController BonusController;
    private BoiteController BoiteController;
    private Boite Boite;
    private RoundController RoundController;

    // La boîte mystère est au centre horizontalement, 600 px au-dessus du point de départ du joueur
    private static readonly Vector2 BOX_POSITION = GameConfig.MapCenter - new Vector2(0, 600);

    public GameController()
    {
        //settings pour la cam
        Camera = new Camera2D();
        Camera.Offset = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);
        Camera.Rotation = 0f;
        Camera.Zoom = 1f;

        InitGame();
    }

    // Crée une partie neuve : tous les objets sont recréés, rien ne reste de la partie précédente
    [MemberNotNull(nameof(Player), nameof(Boite), nameof(BulletController), nameof(ZombieController),
                   nameof(RoundController), nameof(BonusController), nameof(PlayerController),
                   nameof(BoiteController), nameof(View))]
    private void InitGame()
    {
        Player = new Player(new M1911()); // arme de départ
        Boite = new Boite() { Position = BOX_POSITION };

        BulletController = new BulletController(Player);
        ZombieController = new ZombieController(Player, BulletController);
        RoundController = new RoundController(ZombieController);
        BonusController = new BonusController(Player, ZombieController);
        PlayerController = new PlayerController(Player, ZombieController, BulletController);
        BoiteController = new BoiteController(Boite, Player);

        View = new GameView(Textures, Player, Boite, ZombieController, BulletController, BonusController, RoundController);
        Camera.Target = Player.Position;
    }

    public void Update(float dt)
    {
        // Si le joueur est mort, on attend juste la touche R pour rejouer
        if (!Player.IsAlive)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.R))
            {
                InitGame();
            }
            return;
        }

        PlayerController.Update(dt, Camera);
        ZombieController.Update(dt);
        BulletController.Update(dt);
        BonusController.Update(dt);
        BoiteController.Update(dt);
        RoundController.Update(dt);

        Camera.Target = Player.Position;
    }

    public void Draw()
    {
        View.Render(Camera);
    }

    // À appeler avant de fermer la fenêtre
    public void Unload()
    {
        Textures.UnloadAll();
    }
}
