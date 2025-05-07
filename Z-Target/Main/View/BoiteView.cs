// using Raylib_cs;
// using System.Numerics;

// public class BoiteView : IView
// {
//     private Boite box;
//     private int boxSize = 40;

//     public BoiteView(Boite box)
//     {
//         this.box = box;
//     }

//     public void Draw()
//     {
//         Color color = box.IsOpen ? Color.Gold : Color.Brown;
//         Raylib.DrawRectangle((int)box.Position.X - boxSize/2, (int)box.Position.Y - boxSize/2, boxSize, boxSize, color);

//         if (box.IsOpen && box.Weapon != null)
//         {

//             Raylib.DrawText($"Time to disappear: {box.WeaponTimeToDisappear}", (int)box.Position.X - 40, (int)box.Position.Y - 40, 16, Color.Black);
//             Raylib.DrawText($"Press R to accept {box.Weapon.name}", (int)box.Position.X - 40, (int)box.Position.Y - 20, 16, Color.Black);
//         }
//         else
//         {
//             Raylib.DrawText($"Press F ({box.BoxPrice})", (int)box.Position.X - 40, (int)box.Position.Y - 60, 16, Color.White);
//         }
//     }
// }

// "Main/sprites/boite/RPG Chests.png"
using Raylib_cs;
using System.Numerics;

public class BoiteView : IView
{
    private Boite box;
    private int multiplier = 4;
    private int boxSize = 32;  // On s'aligne sur la taille du sprite
    private Texture2D chestTexture;
    private Rectangle sourceRec;
    private int frameWidth = 32;
    private int frameHeight = 32;
    private int currentFrame = 0;
    private float frameTime = 0.15f;
    private float timer = 0f;
    private float closeAnimationTimer = 0f;
    private const float CLOSE_ANIMATION_DELAY = 0.3f;  // Délai entre les frames de fermeture

    public BoiteView(Boite box)
    {
        this.box = box;
        chestTexture = Raylib.LoadTexture("Main/sprites/boite/RPG Chests.png");
        Raylib.SetTextureFilter(chestTexture, TextureFilter.Point);  // Ajout du filtre pour éviter le flou

        // On prend la 1ère colonne
        sourceRec = new Rectangle(0, 0, frameWidth, frameHeight);
    }

    public void Draw()
    {
        Vector2 position = box.Position;

        // Mise à jour de l'animation
        timer += Raylib.GetFrameTime();

        if (timer >= frameTime)
        {
            timer = 0f;

            // Si la boîte est ouverte, on joue l'animation
            if (box.IsOpen)
            {
                if (currentFrame >= 3) currentFrame = 3;
                else currentFrame++;
            }
            else
            {
                // Animation de fermeture: 4 -> 5 -> 0
                if (currentFrame == 3) currentFrame = 4;
                else if (currentFrame == 4) currentFrame = 5;
                else if (currentFrame == 5) currentFrame = 0;
            }

            sourceRec.Y = currentFrame * frameHeight;
        }

        // Dessine le coffre animé avec la taille multipliée par 4
        Raylib.DrawTexturePro(
            chestTexture,
            sourceRec,
            new Rectangle(position.X - (boxSize * multiplier) / 2, position.Y - (boxSize * multiplier) / 2, boxSize * multiplier, boxSize * multiplier),
            new Vector2(0, 0),
            0f,
            Color.White
        );

        // Dessine les textes
        if (box.IsOpen && box.Weapon != null)
        {
            Raylib.DrawText($"Time to disappear: {box.WeaponTimeToDisappear}", (int)position.X - 40, (int)position.Y - 40, 16, Color.Black);
            Raylib.DrawText($"Press F to accept {box.Weapon.name}", (int)position.X - 40, (int)position.Y - 20, 16, Color.Black);
        }
        else
        {
            Raylib.DrawText($"Press F ({box.BoxPrice})", (int)position.X - 40, (int)position.Y - 60, 16, Color.White);
        }
    }

    public void Unload()
    {
        Raylib.UnloadTexture(chestTexture);
    }
}
