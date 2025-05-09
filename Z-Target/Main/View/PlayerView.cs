using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

public class PlayerView : IView{

    private Player Player;
    private Texture2D IdleSprite;
    private List<Texture2D> WalkSprites;
    private List<Texture2D> RunSprites;
    
    // Sprites d'armes
    private Texture2D RifleIdleSprite;
    private List<Texture2D> RifleShootSprites;
    private List<Texture2D> RifleReloadSprites;
    
    private Texture2D HandgunIdleSprite;
    private List<Texture2D> HandgunShootSprites;
    private List<Texture2D> HandgunReloadSprites;
    
    private Texture2D ShotgunIdleSprite;
    private List<Texture2D> ShotgunShootSprites;
    private List<Texture2D> ShotgunReloadSprites;
    
    private float AnimationTimer = 0f;
    private float WeaponAnimationTimer = 0f;
    private const float ANIMATION_SPEED = 0.05f;
    private const float WEAPON_ANIMATION_SPEED = 0.05f;
    private const float WEAPON_RELOAD_ANIMATION_SPEED = 0.1f; // Plus lent pour le rechargement
    private int CurrentFrame = 0;
    private int CurrentWeaponFrame = 0;
    private const int ANIMATION_FRAMES = 20;
    private const int WEAPON_SHOOT_FRAMES = 3;  // 3 frames pour l'animation de tir
    private const int WEAPON_RELOAD_FRAMES = 20; // 20 frames pour l'animation de rechargement

    // Nouveaux sprites pour l'attaque au couteau
    private List<Texture2D> KnifeAttackSprites;
    private const int KNIFE_ANIMATION_FRAMES = 8;
    private const float KNIFE_ANIMATION_SPEED = 0.05f;
    private float KnifeAnimationTimer = 0f;
    private int CurrentKnifeFrame = 0;

    public PlayerView(Player player){
        this.Player = player;
        // Charger le sprite idle
        IdleSprite = Raylib.LoadTexture("Main/sprites/feet/idle/survivor-idle_0.png");
        
        // Charger les sprites de marche
        WalkSprites = new List<Texture2D>();
        for (int i = 0; i < ANIMATION_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/feet/walk/survivor-walk_{i}.png";
            WalkSprites.Add(Raylib.LoadTexture(spritePath));
        }

        // Charger les sprites de course
        RunSprites = new List<Texture2D>();
        for (int i = 0; i < ANIMATION_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/feet/run/survivor-run_{i}.png";
            RunSprites.Add(Raylib.LoadTexture(spritePath));
        }

        // Charger les sprites d'armes
        LoadWeaponSprites();

        // Charger les sprites d'attaque au couteau
        KnifeAttackSprites = new List<Texture2D>();
        for (int i = 0; i < KNIFE_ANIMATION_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/knife/survivor-meleeattack_knife_{i}.png";
            KnifeAttackSprites.Add(Raylib.LoadTexture(spritePath));
        }
    }

    private void LoadWeaponSprites()
    {
        // Charger les sprites du fusil d'assaut
        RifleIdleSprite = Raylib.LoadTexture("Main/sprites/rifle/idle/survivor-idle_rifle_0.png");
        RifleShootSprites = new List<Texture2D>();
        for (int i = 0; i < WEAPON_SHOOT_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/rifle/shoot/survivor-shoot_rifle_{i}.png";
            RifleShootSprites.Add(Raylib.LoadTexture(spritePath));
        }
        RifleReloadSprites = new List<Texture2D>();
        for (int i = 0; i < WEAPON_RELOAD_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/rifle/reload/survivor-reload_rifle_{i}.png";
            RifleReloadSprites.Add(Raylib.LoadTexture(spritePath));
        }

        // Charger les sprites du pistolet
        HandgunIdleSprite = Raylib.LoadTexture("Main/sprites/handgun/idle/survivor-idle_handgun_0.png");
        HandgunShootSprites = new List<Texture2D>();
        for (int i = 0; i < WEAPON_SHOOT_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/handgun/shoot/survivor-shoot_handgun_{i}.png";
            HandgunShootSprites.Add(Raylib.LoadTexture(spritePath));
        }
        HandgunReloadSprites = new List<Texture2D>();
        for (int i = 0; i < WEAPON_RELOAD_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/handgun/reload/survivor-reload_handgun_{i}.png";
            HandgunReloadSprites.Add(Raylib.LoadTexture(spritePath));
        }

        // Charger les sprites du shotgun
        ShotgunIdleSprite = Raylib.LoadTexture("Main/sprites/shotgun/idle/survivor-idle_shotgun_0.png");
        ShotgunShootSprites = new List<Texture2D>();
        for (int i = 0; i < WEAPON_SHOOT_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/shotgun/shoot/survivor-shoot_shotgun_{i}.png";
            ShotgunShootSprites.Add(Raylib.LoadTexture(spritePath));
        }
        ShotgunReloadSprites = new List<Texture2D>();
        for (int i = 0; i < WEAPON_RELOAD_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/shotgun/reload/survivor-reload_shotgun_{i}.png";
            ShotgunReloadSprites.Add(Raylib.LoadTexture(spritePath));
        }
    }

    public void Draw(){
        UpdateAnimation();
        UpdateWeaponAnimation();
        DrawPlayerSprite();
        DrawWeaponSprite();
    }

    private void UpdateAnimation()
    {
        if (Player.CurrentAnimationState == Player.AnimationState.KnifeAttack)
        {
            KnifeAnimationTimer += Raylib.GetFrameTime();
            if (KnifeAnimationTimer >= KNIFE_ANIMATION_SPEED)
            {
                KnifeAnimationTimer = 0f;
                CurrentKnifeFrame = (CurrentKnifeFrame + 1) % KNIFE_ANIMATION_FRAMES;
                if (CurrentKnifeFrame == KNIFE_ANIMATION_FRAMES - 1)
                {
                    Player.StopKnifeAttack();
                }
            }
        }
        else if (Player.CurrentAnimationState != Player.AnimationState.Idle)
        {
            AnimationTimer += Raylib.GetFrameTime();
            if (AnimationTimer >= ANIMATION_SPEED)
            {
                AnimationTimer = 0f;
                CurrentFrame = (CurrentFrame + 1) % ANIMATION_FRAMES;
            }
        }
        else
        {
            // Réinitialiser l'animation quand on revient à l'état idle
            CurrentFrame = 0;
            AnimationTimer = 0f;
            CurrentKnifeFrame = 0;
            KnifeAnimationTimer = 0f;
        }
    }

    private void UpdateWeaponAnimation()
    {
        if (Player.CurrentWeaponState != Player.WeaponAnimationState.Idle)
        {
            WeaponAnimationTimer += Raylib.GetFrameTime();
            float currentAnimationSpeed = Player.CurrentWeaponState == Player.WeaponAnimationState.Reloading 
                ? WEAPON_RELOAD_ANIMATION_SPEED 
                : WEAPON_ANIMATION_SPEED;

            if (WeaponAnimationTimer >= currentAnimationSpeed)
            {
                WeaponAnimationTimer = 0f;
                
                switch (Player.CurrentWeaponState)
                {
                    case Player.WeaponAnimationState.Shooting:
                        CurrentWeaponFrame = (CurrentWeaponFrame + 1) % WEAPON_SHOOT_FRAMES;
                        if (CurrentWeaponFrame == 0)
                        {
                            Player.StopShooting();
                        }
                        break;
                    case Player.WeaponAnimationState.Reloading:
                        CurrentWeaponFrame = (CurrentWeaponFrame + 1) % WEAPON_RELOAD_FRAMES;
                        if (CurrentWeaponFrame == 0)
                        {
                            Player.FinishReload();
                        }
                        break;
                }
            }
        }
        else
        {
            CurrentWeaponFrame = 0;
            WeaponAnimationTimer = 0f;
        }
    }

    private void DrawPlayerSprite()
    {
        Texture2D currentSprite = GetCurrentSprite();
        if (currentSprite.Width == 0 || currentSprite.Height == 0) return;

        Vector2 playerSize = new Vector2(Player.Size.X, Player.Size.Y);
        float angle = Player.AngleToMouse;

        Raylib.DrawTexturePro(
            currentSprite,
            new Rectangle(0, 0, currentSprite.Width, currentSprite.Height),
            new Rectangle(Player.Position.X, Player.Position.Y, playerSize.X, playerSize.Y),
            playerSize / 2f,
            angle,
            Color.White
        );
    }

    private Texture2D GetCurrentSprite()
    {
        switch (Player.CurrentAnimationState)
        {
            case Player.AnimationState.KnifeAttack:
                return KnifeAttackSprites[CurrentKnifeFrame];
            case Player.AnimationState.Walking:
                return WalkSprites[CurrentFrame];
            case Player.AnimationState.Running:
                return RunSprites[CurrentFrame];
            default:
                return IdleSprite;
        }
    }

    private void DrawWeaponSprite()
    {
        Texture2D currentWeaponSprite;
        List<Texture2D> shootSprites;
        List<Texture2D> reloadSprites;

        // Si on est en train d'attaquer au couteau, utiliser l'animation du couteau
        if (Player.CurrentAnimationState == Player.AnimationState.KnifeAttack)
        {
            currentWeaponSprite = KnifeAttackSprites[CurrentKnifeFrame];
            // Quadrupler la taille pour l'animation du couteau
            Vector2 knifeSize = new Vector2(Player.Size.X * 2.5f, Player.Size.Y * 2.5f);
            Vector2 knifePosition = Player.Position;
            Rectangle knifeRect = new Rectangle(
                knifePosition.X,
                knifePosition.Y,
                knifeSize.X,
                knifeSize.Y
            );
            Vector2 knifeOrigin = knifeSize / 2f;
            Raylib.DrawTexturePro(
                currentWeaponSprite,
                new Rectangle(0, 0, currentWeaponSprite.Width, currentWeaponSprite.Height),
                knifeRect,
                knifeOrigin,
                Player.AngleToMouse,
                Color.White
            );
            return;
        }

        // Sinon, utiliser l'arme normale
        switch (Player.MainWeapon.Type)
        {
            case WeaponType.Rifle:
                currentWeaponSprite = RifleIdleSprite;
                shootSprites = RifleShootSprites;
                reloadSprites = RifleReloadSprites;
                break;
            case WeaponType.Shotgun:
                currentWeaponSprite = ShotgunIdleSprite;
                shootSprites = ShotgunShootSprites;
                reloadSprites = ShotgunReloadSprites;
                break;
            default: // Handgun
                currentWeaponSprite = HandgunIdleSprite;
                shootSprites = HandgunShootSprites;
                reloadSprites = HandgunReloadSprites;
                break;
        }
        
        switch (Player.CurrentWeaponState)
        {
            case Player.WeaponAnimationState.Shooting:
                currentWeaponSprite = shootSprites[CurrentWeaponFrame];
                break;
            case Player.WeaponAnimationState.Reloading:
                currentWeaponSprite = reloadSprites[CurrentWeaponFrame];
                break;
        }

        // Position de l'arme au centre du joueur
        Vector2 weaponPosition = Player.Position;

        // Doubler la taille de l'arme
        Vector2 weaponSize = new Vector2(Player.Size.X * 2f, Player.Size.Y * 2f);

        Rectangle destRect = new Rectangle(
            weaponPosition.X,
            weaponPosition.Y,
            weaponSize.X,
            weaponSize.Y
        );

        Vector2 origin = weaponSize / 2f;

        Raylib.DrawTexturePro(
            currentWeaponSprite,
            new Rectangle(0, 0, currentWeaponSprite.Width, currentWeaponSprite.Height),
            destRect,
            origin,
            Player.AngleToMouse,
            Color.White
        );
    }

    public Player GetPlayer()
    {
        return Player;
    }

    public void Pivoter(){
        
        Rectangle rect = new Rectangle(Player.Position.X, Player.Position.Y, Player.Size.X, Player.Size.Y);
        // origine du point de pivot au centre du rectangle 
        Vector2 origin = Player.Size / 2f;

        // Choisir la couleur du joueur en fonction de son état
        Color playerColor = Player.Color;
        
        // Si le joueur est invincible, le faire clignoter
        if (Player.IsInvincible() && (Raylib.GetTime() % 0.4) < 0.2)
        {
            playerColor = Color.White; // Flash blanc pendant l'invincibilité
        }

        Raylib.DrawRectanglePro(rect, origin, Player.AngleToMouse, playerColor);
    }
}