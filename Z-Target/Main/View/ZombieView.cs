using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

public class ZombieView : IView
{
    private List<Texture2D> zombieWalkSprites;
    private float animationTimer = 0f;
    private const float ANIMATION_SPEED = 0.1f;
    private int currentFrame = 0;
    private const int ANIMATION_FRAMES = 20;
    private Player player;

    public ZombieView(Player player)
    {
        this.player = player;
        // Charger les sprites de marche des zombies
        zombieWalkSprites = new List<Texture2D>();
        for (int i = 0; i < ANIMATION_FRAMES; i++)
        {
            string spritePath = $"Main/sprites/zombies/move/skeleton-move_{i}.png";
            var texture = Raylib.LoadTexture(spritePath);
            if (texture.Width == 0 || texture.Height == 0)
            {
                Console.WriteLine($"Erreur: Impossible de charger le sprite {spritePath}");
                // Utiliser le premier sprite valide comme fallback
                if (zombieWalkSprites.Count > 0)
                {
                    zombieWalkSprites.Add(zombieWalkSprites[0]);
                }
                else
                {
                    // Si aucun sprite n'est chargé, créer une texture vide
                    zombieWalkSprites.Add(Raylib.LoadTexture("Main/sprites/zombies/move/skeleton-move_0.png"));
                }
            }
            else
            {
                zombieWalkSprites.Add(texture);
            }
        }
    }

    public void Draw()
    {
        List<Zombie> zombiesList = ZombieController.zombiesList;
        zombiesList.Sort((a, b) => a.Position.Y.CompareTo(b.Position.Y)); // Tri par position Y

        // Mettre à jour l'animation
        UpdateAnimation();

        foreach(Zombie zombie in zombiesList)
        {
            DrawZombie(zombie);
            DrawHealthBar(zombie);
        }
    }

    private void UpdateAnimation()
    {
        animationTimer += Raylib.GetFrameTime();
        if (animationTimer >= ANIMATION_SPEED)
        {
            animationTimer = 0f;
            currentFrame = (currentFrame + 1) % ANIMATION_FRAMES;
        }
    }

    private void DrawZombie(Zombie zombie)
    {
        // Calculer la direction vers le joueur
        Vector2 direction = Vector2.Normalize(player.Position - zombie.Position);
        
        // Calculer l'angle vers le joueur
        float angle = MathF.Atan2(direction.Y, direction.X) * (180f / MathF.PI);

        // Calculer la taille du zombie (doublée)
        Vector2 zombieSize = new Vector2(zombie.radius * 4, zombie.radius * 4);

        // Vérifier si le sprite actuel est valide
        if (currentFrame < zombieWalkSprites.Count && zombieWalkSprites[currentFrame].Width > 0)
        {
            // Dessiner le sprite du zombie
            Raylib.DrawTexturePro(
                zombieWalkSprites[currentFrame],
                new Rectangle(0, 0, zombieWalkSprites[currentFrame].Width, zombieWalkSprites[currentFrame].Height),
                new Rectangle(zombie.Position.X, zombie.Position.Y, zombieSize.X, zombieSize.Y),
                zombieSize / 2f,
                angle,
                Color.White
            );
        }
    }

    public void DrawHealthBar(Zombie zombie)
    {
        float health = zombie.Health;
        float maxHealth = zombie.MaxHealth;
        float length = (health/maxHealth) * zombie.radius * 4; // Doublé pour correspondre à la nouvelle taille
        float height = 5;
        Vector2 size = new Vector2(length, height);
        Vector2 healthBarPos = new Vector2(zombie.Position.X - zombie.radius * 2, zombie.Position.Y - (float)(zombie.radius * 3)); // Ajusté pour la nouvelle taille
        Raylib.DrawRectangleV(healthBarPos, size, Color.Red);
    }
}

