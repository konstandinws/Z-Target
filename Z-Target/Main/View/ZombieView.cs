using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

public class ZombieView : IView
{
    private const float FRAME_TIME = 0.1f;       // temps d'affichage d'une image
    private const float SPRITE_SCALE = 4f;       // taille du sprite = rayon du zombie x 4
    private const float HEALTH_BAR_HEIGHT = 5f;
    private const float HEALTH_BAR_OFFSET = 3f;  // barre de vie placée à 3 rayons au-dessus du centre

    private List<Texture2D> ZombieWalkSprites;
    private float AnimationTimer = 0f;
    private int CurrentFrame = 0;
    private Player Player;
    private ZombieController ZombieController;

    public ZombieView(TextureLoader textures, Player player, ZombieController zombieController)
    {
        this.Player = player;
        this.ZombieController = zombieController;
        ZombieWalkSprites = textures.LoadAnimation(GameConfig.Assets.Zombie.Move);
    }

    public void Draw()
    {
        List<Zombie> zombiesList = ZombieController.Zombies;
        zombiesList.Sort((a, b) => a.Position.Y.CompareTo(b.Position.Y)); // Tri par position Y

        UpdateAnimation();

        foreach(Zombie zombie in zombiesList)
        {
            DrawZombie(zombie);
            DrawHealthBar(zombie);
        }
    }

    private void UpdateAnimation()
    {
        AnimationTimer += Raylib.GetFrameTime();
        if (AnimationTimer >= FRAME_TIME)
        {
            AnimationTimer = 0f;
            CurrentFrame = (CurrentFrame + 1) % ZombieWalkSprites.Count;
        }
    }

    private void DrawZombie(Zombie zombie)
    {
        // Angle vers le joueur
        Vector2 direction = Vector2.Normalize(Player.Position - zombie.Position);
        float angle = MathF.Atan2(direction.Y, direction.X) * (180f / MathF.PI);

        float spriteSize = zombie.Radius * SPRITE_SCALE;
        Vector2 zombieSize = new Vector2(spriteSize, spriteSize);
        Texture2D sprite = ZombieWalkSprites[CurrentFrame];

        Raylib.DrawTexturePro(
            sprite,
            new Rectangle(0, 0, sprite.Width, sprite.Height),
            new Rectangle(zombie.Position.X, zombie.Position.Y, zombieSize.X, zombieSize.Y),
            zombieSize / 2f,
            angle,
            Color.White
        );
    }

    public void DrawHealthBar(Zombie zombie)
    {
        float fullWidth = zombie.Radius * SPRITE_SCALE; // même largeur que le sprite
        float width = (zombie.Health / zombie.MaxHealth) * fullWidth;
        Vector2 healthBarPos = new Vector2(zombie.Position.X - fullWidth / 2, zombie.Position.Y - zombie.Radius * HEALTH_BAR_OFFSET);
        Raylib.DrawRectangleV(healthBarPos, new Vector2(width, HEALTH_BAR_HEIGHT), Color.Red);
    }
}
