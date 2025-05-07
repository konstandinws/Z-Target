using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

public class PlayerView : IView{

    private Player player;
    private Texture2D idleSprite;
    private List<Texture2D> walkSprites;
    private List<Texture2D> runSprites;
    
    // Sprites d'armes
    private Texture2D rifleIdleSprite;
    private List<Texture2D> rifleShootSprites;
    private List<Texture2D> rifleReloadSprites;
    
    private Texture2D handgunIdleSprite;
    private List<Texture2D> handgunShootSprites;
    private List<Texture2D> handgunReloadSprites;
    
    private float animationTimer = 0f;
    private float weaponAnimationTimer = 0f;
    private const float ANIMATION_SPEED = 0.05f;
    private const float WEAPON_ANIMATION_SPEED = 0.05f;
    private const float WEAPON_RELOAD_ANIMATION_SPEED = 0.1f; // Plus lent pour le rechargement
    private int currentFrame = 0;
    private int currentWeaponFrame = 0;
    private const int ANIMATION_FRAMES = 20;
    private const int WEAPON_SHOOT_FRAMES = 3;  // 3 frames pour l'animation de tir
    private const int WEAPON_RELOAD_FRAMES = 20; // 20 frames pour l'animation de rechargement

    public PlayerView(Player player){
        this.player = player;
        // Charger le sprite idle
        idleSprite = Raylib.LoadTexture("Main/sprites/feet/idle/survivor-idle_0.png");
        
        // Charger les sprites de marche
        walkSprites = new List<Texture2D>();
        for (int i = 0; i < ANIMATION_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/feet/walk/survivor-walk_{i}.png";
            walkSprites.Add(Raylib.LoadTexture(spritePath));
        }

        // Charger les sprites de course
        runSprites = new List<Texture2D>();
        for (int i = 0; i < ANIMATION_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/feet/run/survivor-run_{i}.png";
            runSprites.Add(Raylib.LoadTexture(spritePath));
        }

        // Charger les sprites d'armes
        LoadWeaponSprites();
    }

    private void LoadWeaponSprites()
    {
        // Charger les sprites du fusil d'assaut
        rifleIdleSprite = Raylib.LoadTexture("Main/sprites/rifle/idle/survivor-idle_rifle_0.png");
        rifleShootSprites = new List<Texture2D>();
        for (int i = 0; i < WEAPON_SHOOT_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/rifle/shoot/survivor-shoot_rifle_{i}.png";
            rifleShootSprites.Add(Raylib.LoadTexture(spritePath));
        }
        rifleReloadSprites = new List<Texture2D>();
        for (int i = 0; i < WEAPON_RELOAD_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/rifle/reload/survivor-reload_rifle_{i}.png";
            rifleReloadSprites.Add(Raylib.LoadTexture(spritePath));
        }

        // Charger les sprites du pistolet
        handgunIdleSprite = Raylib.LoadTexture("Main/sprites/handgun/idle/survivor-idle_handgun_0.png");
        handgunShootSprites = new List<Texture2D>();
        for (int i = 0; i < WEAPON_SHOOT_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/handgun/shoot/survivor-shoot_handgun_{i}.png";
            handgunShootSprites.Add(Raylib.LoadTexture(spritePath));
        }
        handgunReloadSprites = new List<Texture2D>();
        for (int i = 0; i < WEAPON_RELOAD_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/handgun/reload/survivor-reload_handgun_{i}.png";
            handgunReloadSprites.Add(Raylib.LoadTexture(spritePath));
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
        if (player.CurrentAnimationState != Player.AnimationState.Idle)
        {
            animationTimer += Raylib.GetFrameTime();
            if (animationTimer >= ANIMATION_SPEED)
            {
                animationTimer = 0f;
                currentFrame = (currentFrame + 1) % ANIMATION_FRAMES;
            }
        }
        else
        {
            // Réinitialiser l'animation quand on revient à l'état idle
            currentFrame = 0;
            animationTimer = 0f;
        }
    }

    private void UpdateWeaponAnimation()
    {
        if (player.CurrentWeaponState != Player.WeaponAnimationState.Idle)
        {
            weaponAnimationTimer += Raylib.GetFrameTime();
            float currentAnimationSpeed = player.CurrentWeaponState == Player.WeaponAnimationState.Reloading 
                ? WEAPON_RELOAD_ANIMATION_SPEED 
                : WEAPON_ANIMATION_SPEED;

            if (weaponAnimationTimer >= currentAnimationSpeed)
            {
                weaponAnimationTimer = 0f;
                
                switch (player.CurrentWeaponState)
                {
                    case Player.WeaponAnimationState.Shooting:
                        currentWeaponFrame = (currentWeaponFrame + 1) % WEAPON_SHOOT_FRAMES;
                        if (currentWeaponFrame == 0)
                        {
                            player.StopShooting();
                        }
                        break;
                    case Player.WeaponAnimationState.Reloading:
                        currentWeaponFrame = (currentWeaponFrame + 1) % WEAPON_RELOAD_FRAMES;
                        if (currentWeaponFrame == 0)
                        {
                            player.FinishReload();
                        }
                        break;
                }
            }
        }
        else
        {
            currentWeaponFrame = 0;
            weaponAnimationTimer = 0f;
        }
    }

    private void DrawPlayerSprite()
    {
        Texture2D currentSprite = idleSprite;
        switch (player.CurrentAnimationState)
        {
            case Player.AnimationState.Walking:
                currentSprite = walkSprites[currentFrame];
                break;
            case Player.AnimationState.Running:
                currentSprite = runSprites[currentFrame];
                break;
        }

        // Calculer la destination rectangle
        Rectangle destRect = new Rectangle(
            player.Position.X,
            player.Position.Y,
            player.Size.X,
            player.Size.Y
        );

        // Origine pour la rotation
        Vector2 origin = player.Size / 2f;

        // Dessiner le sprite avec rotation
        Raylib.DrawTexturePro(
            currentSprite,
            new Rectangle(0, 0, currentSprite.Width, currentSprite.Height),
            destRect,
            origin,
            player.AngleToMouse,
            Color.White
        );
    }

    private void DrawWeaponSprite()
    {
        Texture2D currentWeaponSprite;
        List<Texture2D> shootSprites;
        List<Texture2D> reloadSprites;

        // Sélectionner les sprites en fonction de l'arme équipée
        if (player.mainWeapon is AK47)
        {
            currentWeaponSprite = rifleIdleSprite;
            shootSprites = rifleShootSprites;
            reloadSprites = rifleReloadSprites;
        }
        else // M1911
        {
            currentWeaponSprite = handgunIdleSprite;
            shootSprites = handgunShootSprites;
            reloadSprites = handgunReloadSprites;
        }
        
        switch (player.CurrentWeaponState)
        {
            case Player.WeaponAnimationState.Shooting:
                currentWeaponSprite = shootSprites[currentWeaponFrame];
                break;
            case Player.WeaponAnimationState.Reloading:
                currentWeaponSprite = reloadSprites[currentWeaponFrame];
                break;
        }

        // Position de l'arme au centre du joueur
        Vector2 weaponPosition = player.Position;

        // Doubler la taille de l'arme
        Vector2 weaponSize = new Vector2(player.Size.X * 2f, player.Size.Y * 2f);

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
            player.AngleToMouse,
            Color.White
        );
    }

    public Player GetPlayer()
    {
        return player;
    }

    public void Pivoter(){
        
        Rectangle rect = new Rectangle(player.Position.X, player.Position.Y, player.Size.X, player.Size.Y);
        // origine du point de pivot au centre du rectangle 
        Vector2 origin = player.Size / 2f;

        // Choisir la couleur du joueur en fonction de son état
        Color playerColor = player.Color;
        
        // Si le joueur est invincible, le faire clignoter
        if (player.IsInvincible() && (Raylib.GetTime() % 0.4) < 0.2)
        {
            playerColor = Color.White; // Flash blanc pendant l'invincibilité
        }

        Raylib.DrawRectanglePro(rect, origin, player.AngleToMouse, playerColor);
    }
}