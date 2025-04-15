using Raylib_cs;
using System.Numerics;
public class PlayerController {

    private Player player;
    // private PlayerView view;
    Vector2 mouseWorldPos;

    public PlayerController(Player player){
        this.player = player;
        // view = new PlayerView(player);
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
    public void Update(float dt, Camera2D camera)
    {
        HandleInput(camera);
        player.Move(dt);
    }
}