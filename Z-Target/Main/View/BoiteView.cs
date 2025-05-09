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
    private Boite Box;
    private int Multiplier = 4;
    private int BoxSize = 32;  // On s'aligne sur la taille du sprite
    private Texture2D ChestTexture;
    private Rectangle SourceRec;
    private int FrameWidth = 32;
    private int FrameHeight = 32;
    private int CurrentFrame = 0;
    private float FrameTime = 0.15f;
    private float Timer = 0f;
    private int TextOffset = 100;

    public BoiteView(Boite box)
    {
        this.Box = box;
        ChestTexture = Raylib.LoadTexture("Main/sprites/boite/RPG Chests.png");
        Raylib.SetTextureFilter(ChestTexture, TextureFilter.Point);  // Ajout du filtre pour éviter le flou

        // On prend la 1ère colonne
        SourceRec = new Rectangle(0, 0, FrameWidth, FrameHeight);
    }

    public void Draw()
    {
        Vector2 position = Box.Position;

        // Mise à jour de l'animation
        Timer += Raylib.GetFrameTime();

        if (Timer >= FrameTime)
        {
            Timer = 0f;

            // Si la boîte est ouverte, on joue l'animation
            if (Box.IsOpen)
            {
                if (CurrentFrame >= 3) CurrentFrame = 3;
                else CurrentFrame++;
            }
            else
            {
                // Animation de fermeture: 4 -> 5 -> 0
                if (CurrentFrame == 3) CurrentFrame = 4;
                else if (CurrentFrame == 4) CurrentFrame = 5;
                else if (CurrentFrame == 5) CurrentFrame = 0;
            }

            SourceRec.Y = CurrentFrame * FrameHeight;
        }

        // Dessine le coffre animé avec la taille multipliée par 4
        Raylib.DrawTexturePro(
            ChestTexture,
            SourceRec,
            new Rectangle(position.X - (BoxSize * Multiplier) / 2, position.Y - (BoxSize * Multiplier) / 2, BoxSize * Multiplier, BoxSize * Multiplier),
            new Vector2(0, 0),
            0f,
            Color.White
        );

        // Dessine les textes
        if (Box.IsOpen && Box.Weapon != null)
        {
            Raylib.DrawText($"Time to disappear: {Math.Round(Box.WeaponTimeToDisappear, 0)}", (int)position.X - 40, (int)position.Y - TextOffset, 16, Color.Black);
            Raylib.DrawText($"Press F to accept {Box.Weapon.Name}", (int)position.X - 40, (int)position.Y - TextOffset + 20, 16, Color.Black);
        }
        else
        {
            Raylib.DrawText($"Press F ({Box.BoxPrice})", (int)position.X - 40, (int)position.Y - 60, 16, Color.White);
        }
    }
}
