using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

public class PlayerController {

    private Player Player;
    private ZombieController ZombieController;
    // private PlayerView view;
    Vector2 MouseWorldPos;
    private const float KNIFE_RANGE = 100f;
    private const float KNIFE_DAMAGE = 50f;
    private const float KNIFE_COOLDOWN = 0.5f;
    private float lastKnifeAttackTime = 0f;

    public PlayerController(Player player, ZombieController zombieController)
    {
        this.Player = player;
        this.ZombieController = zombieController;
        Player.MainWeapon = new M1911();
    }

    public void HandleInput(Camera2D camera)
    {
        HandleMovement();
        HandleRotation(camera);
        HandleShooting();
        HandleReload();
        changeWeapon();
        HandleKnifeAttack();
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
        Player.SetSprinting(isSprinting);

        Player.Direction = direction != Vector2.Zero ? Vector2.Normalize(direction) : Vector2.Zero;
    }

    private void HandleShooting()
    {
        if (Raylib.IsMouseButtonDown(MouseButton.Left) && Player.MainWeapon.CurrentAmmo > 0 && !Player.IsReloading)
        {
            Player.MainWeapon.Shoot(Player.Position, MouseWorldPos, BulletController.Instance);
            Player.StartShooting();
        }
        else
        {
            Player.StopShooting();
        }
    }

    private void HandleReload()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.R) && Player.MainWeapon.CurrentAmmo < Player.MainWeapon.ClipSize && !Player.IsReloading && Player.MainWeapon.ReservAmmo > 0)
        {
            Player.IsReloading = true;
            Player.MainWeapon.ReloadTimer = 0f;
            Player.MainWeapon.Reload();
            Player.StartReload();
        }

        if (Player.IsReloading)
        {
            Player.MainWeapon.ReloadTimer += Raylib.GetFrameTime();
            if (Player.MainWeapon.ReloadTimer >= Player.MainWeapon.ReloadTime)
            {
                Player.IsReloading = false;
                Player.FinishReload();
            }
        }
    }

    private void changeWeapon(){

        if(Raylib.IsKeyPressed(KeyboardKey.E)){
            if(Player.SecondaryWeapon != null){
                Weapon temp = Player.MainWeapon;
                Player.MainWeapon = Player.SecondaryWeapon;
                Player.SecondaryWeapon = temp;
            }
        }
    }

    private void HandleRotation(Camera2D camera)
    {
        Vector2 mouseScreenPos = Raylib.GetMousePosition();
        MouseWorldPos = Raylib.GetScreenToWorld2D(mouseScreenPos, camera);

        Player.UpdateAngle(MouseWorldPos);
    }

    private void HandleKnifeAttack()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.V))
        {
            if (Raylib.GetTime() - lastKnifeAttackTime >= KNIFE_COOLDOWN)
            {
                lastKnifeAttackTime = (float)Raylib.GetTime();
                Player.StartKnifeAttack();
                AttackWithKnife(Player);
            }
        }
    }

    private void AttackWithKnife(Player player)
    {
        for (int i = ZombieController.zombiesList.Count - 1; i >= 0; i--)
        {
            Zombie zombie = ZombieController.zombiesList[i];
            float distance = Vector2.Distance(player.Position, zombie.Position);
            if (distance <= KNIFE_RANGE)
            {
                // Calculer la direction vers le zombie
                Vector2 zombieDirection = Vector2.Normalize(zombie.Position - player.Position);
                
                // Convertir l'angle de la souris en radians
                float mouseAngleRad = player.AngleToMouse * (MathF.PI / 180f);
                Vector2 mouseDirection = new Vector2(MathF.Cos(mouseAngleRad), MathF.Sin(mouseAngleRad));
                
                // Calculer l'angle entre la direction de la souris et la direction vers le zombie
                float dotProduct = Vector2.Dot(mouseDirection, zombieDirection);
                float angle = MathF.Acos(dotProduct) * (180f / MathF.PI);
                
                // Si le zombie est dans le cône d'attaque (angle de 60 degrés)
                if (angle <= 30f)
                {
                    ZombieController.HandleKnifeDamage(zombie, (int)KNIFE_DAMAGE);
                }
            }
        }
    }

    public void Update(float dt, Camera2D camera)
    {
        HandleInput(camera);
        Player.Move(dt);
    }
}