using Raylib_cs;
using System.Numerics;
public class PlayerController {

    private Player player;
    // private PlayerView view;
    Vector2 mouseWorldPos;


    public PlayerController(Player player){
        this.player = player;
        player.mainWeapon = new M1911();
        player.secondaryWeapon = new AK47();
    }

    public void HandleInput(Camera2D camera)
    {
        HandleMovement();
        HandleRotation(camera);
        HandleShooting();
        HandleReload();
        changeWeapon();
    }

    private void HandleMovement()
    {
        Vector2 direction = Vector2.Zero;

        if (Raylib.IsKeyDown(KeyboardKey.W)) direction.Y -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.S)) direction.Y += 1;
        if (Raylib.IsKeyDown(KeyboardKey.A)) direction.X -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.D)) direction.X += 1;

        // Détecter si le joueur appuie sur Shift pour sprinter
        bool isSprinting = Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);
        player.SetSprinting(isSprinting);

        player.Direction = direction != Vector2.Zero ? Vector2.Normalize(direction) : Vector2.Zero;
    }

    private void HandleShooting()
    {
        if (Raylib.IsMouseButtonDown(MouseButton.Left) && player.mainWeapon.currentAmmo > 0 && !player.isReloading)
        {
            player.mainWeapon.Shoot(player.Position, mouseWorldPos, BulletController.Instance);
            player.StartShooting();
        }
        else
        {
            player.StopShooting();
        }
    }

    private void HandleReload()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.R) && player.mainWeapon.currentAmmo < player.mainWeapon.clipSize && !player.isReloading && player.mainWeapon.reservAmmo > 0)
        {
            player.isReloading = true;
            player.mainWeapon.reloadTimer = 0f;
            player.mainWeapon.Reload();
            player.StartReload();
        }

        if (player.isReloading)
        {
            player.mainWeapon.reloadTimer += Raylib.GetFrameTime();
            if (player.mainWeapon.reloadTimer >= player.mainWeapon.reloadTime)
            {
                player.mainWeapon.currentAmmo = player.mainWeapon.clipSize;
                player.isReloading = false;
                player.FinishReload();
            }
        }
        if(player.mainWeapon.reservAmmo == 0 && player.mainWeapon.currentAmmo == 0){
            HUDView.Instance.NoAmmo();
        }
    }

    private void changeWeapon(){

        if(Raylib.IsKeyPressed(KeyboardKey.E)){
            if(player.secondaryWeapon != null){
                Weapon temp = player.mainWeapon;
                player.mainWeapon = player.secondaryWeapon;
                player.secondaryWeapon = temp;
            }
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