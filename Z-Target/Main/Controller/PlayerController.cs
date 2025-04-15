using Raylib_cs;
using System.Numerics;
public class PlayerController {

    private Player player;
    private ZombieController zombieController;
    Vector2 mouseWorldPos;
    private float damageCooldown = 0f;
    private const float DAMAGE_COOLDOWN_TIME = 0.5f; // Temps entre chaque dégât

    public PlayerController(Player player, ZombieController zombieController){
        this.player = player;
        this.zombieController = zombieController;
    }

    public void HandleInput(Camera2D camera)
    {
        HandleMovement();
        HandleRotation(camera);
        HandleShooting();
    }

    private void HandleMovement()
    {
        Vector2 direction = Vector2.Zero;

        if (Raylib.IsKeyDown(KeyboardKey.W)) direction.Y -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.S)) direction.Y += 1;
        if (Raylib.IsKeyDown(KeyboardKey.A)) direction.X -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.D)) direction.X += 1;

        player.Direction = direction != Vector2.Zero ? Vector2.Normalize(direction) : Vector2.Zero;
    }

    private void HandleShooting()
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            BulletController.Instance.ShootBullet(mouseWorldPos);
        }
    }

    private void HandleRotation(Camera2D camera)
    {
        Vector2 mouseScreenPos = Raylib.GetMousePosition();
        mouseWorldPos = Raylib.GetScreenToWorld2D(mouseScreenPos, camera);

        player.UpdateAngle(mouseWorldPos);
    }

    private void HandleZombieCollisions(float dt)
    {
        if (damageCooldown > 0)
        {
            damageCooldown -= dt;
            return;
        }

        foreach (Zombie zombie in zombieController.GetZombies())
        {
            if (Vector2.Distance(player.Position, zombie.Position) < player.Size.X/2 + zombie.radius)
            {
                player.TakeDamage(zombie.Damage); 
                damageCooldown = DAMAGE_COOLDOWN_TIME;
                Console.WriteLine("Player health: " + player.Health);
                break; // On sort de la boucle après avoir pris des dégâts
            }
        }
    }

    public void Update(float dt, Camera2D camera)
    {
        HandleInput(camera);
        player.Move(dt);
        HandleZombieCollisions(dt);
    }
}