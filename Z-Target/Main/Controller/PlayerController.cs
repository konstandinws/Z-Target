using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

public class PlayerController {

    private Player Player;
    private ZombieController ZombieController;
    private BulletController BulletController;
    // private PlayerView view;
    Vector2 MouseWorldPos;
    private const float KNIFE_RANGE = 100f;
    private const int KNIFE_DAMAGE = 50;
    private const float KNIFE_COOLDOWN = 0.5f;
    private const float KNIFE_HALF_ANGLE = 30f; // cône d'attaque de 60° devant le joueur
    private float lastKnifeAttackTime = 0f;
    private const float DEG_TO_RAD = MathF.PI / 180f;
    private const float RAD_TO_DEG = 180f / MathF.PI;

    public PlayerController(Player player, ZombieController zombieController, BulletController bulletController)
    {
        this.Player = player;
        this.ZombieController = zombieController;
        this.BulletController = bulletController;
    }

    public void HandleInput(float dt, Camera2D camera)
    {
        HandleMovement();
        HandleRotation(camera);
        HandleShooting();
        HandleReload(dt);
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
            Player.MainWeapon.Shoot(Player.Position, MouseWorldPos, BulletController);
            Player.StartShooting();
        }
        else
        {
            Player.StopShooting();
        }
    }

    private void HandleReload(float dt)
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
            Player.MainWeapon.ReloadTimer += dt;
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
        for (int i = ZombieController.Zombies.Count - 1; i >= 0; i--)
        {
            Zombie zombie = ZombieController.Zombies[i];
            float distance = Vector2.Distance(player.Position, zombie.Position);
            if (distance <= KNIFE_RANGE)
            {
                // Calculer la direction vers le zombie
                Vector2 zombieDirection = Vector2.Normalize(zombie.Position - player.Position);
                
                // Convertir l'angle de la souris en radians
                float mouseAngleRad = player.AngleToMouse * DEG_TO_RAD;
                Vector2 mouseDirection = new Vector2(MathF.Cos(mouseAngleRad), MathF.Sin(mouseAngleRad));
                
                // Calculer l'angle entre la direction de la souris et la direction vers le zombie
                // (Clamp : à cause des arrondis, le produit scalaire peut dépasser 1 et Acos renverrait NaN)
                float dotProduct = Math.Clamp(Vector2.Dot(mouseDirection, zombieDirection), -1f, 1f);
                float angle = MathF.Acos(dotProduct) * RAD_TO_DEG;

                // Si le zombie est dans le cône d'attaque
                if (angle <= KNIFE_HALF_ANGLE)
                {
                    ZombieController.HandleKnifeDamage(zombie, KNIFE_DAMAGE);
                }
            }
        }
    }

    public void Update(float dt, Camera2D camera)
    {
        HandleInput(dt, camera);
        Player.Move(dt);
    }
}