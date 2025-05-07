using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

public class PlayerController {

    private Player player;
    private ZombieController zombieController;
    // private PlayerView view;
    Vector2 mouseWorldPos;
    private const float KNIFE_RANGE = 100f;
    private const float KNIFE_DAMAGE = 50f;
    private const float KNIFE_COOLDOWN = 0.5f;
    private float lastKnifeAttackTime = 0f;

    public PlayerController(Player player, ZombieController zombieController)
    {
        this.player = player;
        this.zombieController = zombieController;
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

    private void HandleKnifeAttack()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.V))
        {
            if (Raylib.GetTime() - lastKnifeAttackTime >= KNIFE_COOLDOWN)
            {
                lastKnifeAttackTime = (float)Raylib.GetTime();
                player.StartKnifeAttack();
                AttackWithKnife(player);
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
                    zombieController.HandleKnifeDamage(zombie, (int)KNIFE_DAMAGE);
                }
            }
        }
    }

    public void Update(float dt, Camera2D camera)
    {
        HandleInput(camera);
        player.Move(dt);
    }
}