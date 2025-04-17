using Raylib_cs;
using System.Numerics;
using System.Linq;

public class ZombieView : IView
{
    private ZombieController zombieController;
    private const int sliceCount = 65;
    private Texture2D[] slices;
    private const float scale = 3f;
    private const int sliceOffset = 2;
    private Player player;

    public ZombieView(ZombieController zombieController, Player player)
    {
        this.zombieController = zombieController;
        this.player = player;
        slices = new Texture2D[sliceCount];
        for (int i = 1; i < sliceCount; i++)
        {
            slices[i] = Raylib.LoadTexture($"zombie/Zombie-{i}.png");
        }
    }

    public void Draw()
    {
        // Trier les zombies par position Y (du plus bas au plus haut)
        List<Zombie> sortedZombies = zombieController.GetZombies()
            .OrderBy(z => z.Position.Y)
            .ToList();

        foreach(Zombie zombie in sortedZombies)
        {
            float centerX = zombie.Position.X;
            float centerY = zombie.Position.Y - (sliceCount * sliceOffset) + 150;

            // Calcul de l'angle entre le zombie et le joueur
            Vector2 direction = player.Position - zombie.Position;
            float angle = MathF.Atan2(direction.Y, direction.X) * (180f / MathF.PI) - 90f;

            for (int i = 0; i < sliceCount; i++)
            {
                int offsetY = i * -sliceOffset;
                Texture2D slice = slices[i];

                // Calcul du centre de la tranche pour la rotation
                float sliceCenterX = centerX;
                float sliceCenterY = centerY + offsetY;

                // Calcul du rectangle de destination pour DrawTextureEx
                Rectangle sourceRec = new Rectangle(0, 0, slice.Width, slice.Height);
                Rectangle destRec = new Rectangle(sliceCenterX, sliceCenterY, slice.Width * scale, slice.Height * scale);
                Vector2 origin = new Vector2(slice.Width * scale / 2, slice.Height * scale / 2);

                Raylib.DrawTexturePro(slice, sourceRec, destRec, origin, angle, Color.White);
            }

            DrawHealthBar(zombie, centerY);
        }
    }

    private void DrawHealthBar(Zombie zombie, float centerY)
    {
        // Ajustement de la position de la barre de vie pour qu'elle soit au-dessus du zombie
        float healthBarY = centerY - (sliceCount * sliceOffset) - 10;
        Raylib.DrawRectangleV(
            new Vector2(zombie.Position.X - (zombie.radius * 1.5f) / 2, healthBarY),
            new Vector2((zombie.Health / 100) * (zombie.radius * 1.5f), 5),
            Color.Red
        );
    }
}

