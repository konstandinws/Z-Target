using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

public class PlayerView : IView{

    private const string SPRITES = "Main/sprites";

    // Temps d'affichage d'une image pour les animations en boucle
    private const float FEET_FRAME_TIME = 0.05f;
    private const float SHOOT_FRAME_TIME = 0.05f;

    // Taille des sprites par rapport à la taille du joueur
    private const float WEAPON_SPRITE_SCALE = 2f;
    private const float KNIFE_SPRITE_SCALE = 2.5f;

    // Sprites d'une arme (fusil, pistolet ou fusil à pompe)
    private class WeaponSprites
    {
        public Texture2D Idle;
        public List<Texture2D> Shoot = new List<Texture2D>();
        public List<Texture2D> Reload = new List<Texture2D>();
    }

    private Player Player;
    private Texture2D IdleSprite;
    private List<Texture2D> WalkSprites;
    private List<Texture2D> RunSprites;
    private List<Texture2D> KnifeAttackSprites;
    private Dictionary<WeaponType, WeaponSprites> WeaponSpritesByType;

    private float FeetAnimationTimer = 0f;
    private int CurrentFeetFrame = 0;
    private float ShootAnimationTimer = 0f;
    private int CurrentShootFrame = 0;

    public PlayerView(TextureLoader textures, Player player){
        this.Player = player;

        IdleSprite = textures.Load($"{SPRITES}/feet/idle/survivor-idle_0.png");
        WalkSprites = textures.LoadAnimation($"{SPRITES}/feet/walk");
        RunSprites = textures.LoadAnimation($"{SPRITES}/feet/run");
        KnifeAttackSprites = textures.LoadAnimation($"{SPRITES}/knife");

        WeaponSpritesByType = new Dictionary<WeaponType, WeaponSprites>
        {
            [WeaponType.Rifle] = LoadWeaponSprites(textures, "rifle"),
            [WeaponType.Handgun] = LoadWeaponSprites(textures, "handgun"),
            [WeaponType.Shotgun] = LoadWeaponSprites(textures, "shotgun"),
        };
    }

    private static WeaponSprites LoadWeaponSprites(TextureLoader textures, string weapon)
    {
        return new WeaponSprites
        {
            Idle = textures.Load($"{SPRITES}/{weapon}/idle/survivor-idle_{weapon}_0.png"),
            Shoot = textures.LoadAnimation($"{SPRITES}/{weapon}/shoot"),
            Reload = textures.LoadAnimation($"{SPRITES}/{weapon}/reload"),
        };
    }

    public void Draw(){
        float dt = Raylib.GetFrameTime();
        UpdateFeetAnimation(dt);
        UpdateShootAnimation(dt);
        DrawPlayerSprite();
        DrawWeaponSprite();
    }

    // Image correspondant à un avancement entre 0 et 1 (pour les animations qui suivent une durée précise)
    private static Texture2D FrameAtProgress(List<Texture2D> frames, float progress)
    {
        int index = (int)(progress * frames.Count);
        return frames[Math.Clamp(index, 0, frames.Count - 1)];
    }

    private void UpdateFeetAnimation(float dt)
    {
        if (Player.CurrentAnimationState == Player.AnimationState.Idle)
        {
            CurrentFeetFrame = 0;
            FeetAnimationTimer = 0f;
            return;
        }

        FeetAnimationTimer += dt;
        if (FeetAnimationTimer >= FEET_FRAME_TIME)
        {
            FeetAnimationTimer = 0f;
            CurrentFeetFrame++; // ramené dans la bonne plage au moment du dessin (walk et run n'ont pas forcément le même nombre d'images)
        }
    }

    private void UpdateShootAnimation(float dt)
    {
        if (Player.CurrentWeaponState != Player.WeaponAnimationState.Shooting)
        {
            CurrentShootFrame = 0;
            ShootAnimationTimer = 0f;
            return;
        }

        ShootAnimationTimer += dt;
        if (ShootAnimationTimer >= SHOOT_FRAME_TIME)
        {
            ShootAnimationTimer = 0f;
            int frameCount = WeaponSpritesByType[Player.MainWeapon.Type].Shoot.Count;
            CurrentShootFrame = (CurrentShootFrame + 1) % frameCount;
            if (CurrentShootFrame == 0)
            {
                Player.StopShooting();
            }
        }
    }

    private void DrawPlayerSprite()
    {
        Texture2D currentSprite = GetCurrentFeetSprite();
        if (currentSprite.Width == 0 || currentSprite.Height == 0) return;

        DrawRotated(currentSprite, Player.Size);
    }

    private Texture2D GetCurrentFeetSprite()
    {
        switch (Player.CurrentAnimationState)
        {
            case Player.AnimationState.KnifeAttack:
                return FrameAtProgress(KnifeAttackSprites, Player.KnifeAttackProgress);
            case Player.AnimationState.Walking:
                return WalkSprites[CurrentFeetFrame % WalkSprites.Count];
            case Player.AnimationState.Running:
                return RunSprites[CurrentFeetFrame % RunSprites.Count];
            default:
                return IdleSprite;
        }
    }

    private void DrawWeaponSprite()
    {
        // Pendant l'attaque au couteau, on affiche l'animation du couteau à la place de l'arme
        if (Player.CurrentAnimationState == Player.AnimationState.KnifeAttack)
        {
            Texture2D knifeSprite = FrameAtProgress(KnifeAttackSprites, Player.KnifeAttackProgress);
            DrawRotated(knifeSprite, Player.Size * KNIFE_SPRITE_SCALE);
            return;
        }

        WeaponSprites sprites = WeaponSpritesByType[Player.MainWeapon.Type];
        Texture2D weaponSprite = sprites.Idle;

        switch (Player.CurrentWeaponState)
        {
            case Player.WeaponAnimationState.Shooting:
                weaponSprite = sprites.Shoot[CurrentShootFrame % sprites.Shoot.Count];
                break;
            case Player.WeaponAnimationState.Reloading:
                // L'animation suit la vraie durée de rechargement de l'arme
                Weapon weapon = Player.MainWeapon;
                weaponSprite = FrameAtProgress(sprites.Reload, weapon.ReloadTimer / weapon.ReloadTime);
                break;
        }

        DrawRotated(weaponSprite, Player.Size * WEAPON_SPRITE_SCALE);
    }

    // Dessine une texture centrée sur le joueur et tournée vers la souris
    private void DrawRotated(Texture2D texture, Vector2 size)
    {
        Raylib.DrawTexturePro(
            texture,
            new Rectangle(0, 0, texture.Width, texture.Height),
            new Rectangle(Player.Position.X, Player.Position.Y, size.X, size.Y),
            size / 2f,
            Player.AngleToMouse,
            Color.White
        );
    }
}
