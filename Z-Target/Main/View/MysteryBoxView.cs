using Raylib_cs;
using System.Numerics;

public class MysteryBoxView : IView
{
    // Le sprite sheet "RPG Chests.png" : chaque colonne est un coffre, chaque ligne une étape de l'animation
    private const int FRAME_SIZE = 32;         // taille d'une image dans le sprite sheet
    private const int CHEST_COLUMN = 1;        // colonne du coffre utilisé (0 = première colonne)
    private const int LAST_OPENING_FRAME = 3;  // ouverture : images 0 -> 3
    private const int FIRST_CLOSING_FRAME = 4; // fermeture : images 3 -> 4 -> 5 -> 0
    private const int LAST_CLOSING_FRAME = 5;
    private const int CLOSED_FRAME = 0;
    private const float FRAME_TIME = 0.15f;
    private const int SCALE = 4;               // le coffre est affiché 4 fois plus grand
    private const int DRAWN_SIZE = FRAME_SIZE * SCALE;

    // Position des textes par rapport au centre de la boîte
    private const int FONT_SIZE = 16;
    private const int TEXT_OFFSET_X = 40;
    private const int OPEN_TEXT_OFFSET_Y = 100;
    private const int LINE_SPACING = 20;
    private const int PRICE_TEXT_OFFSET_Y = 60;

    private MysteryBox Box;
    private Texture2D ChestTexture;
    private Rectangle SourceRec;
    private int CurrentFrame = CLOSED_FRAME;
    private float Timer = 0f;

    public MysteryBoxView(TextureLoader textures, MysteryBox box)
    {
        this.Box = box;
        ChestTexture = textures.Load(GameConfig.Assets.MysteryBox);
        SourceRec = new Rectangle(CHEST_COLUMN * FRAME_SIZE, 0, FRAME_SIZE, FRAME_SIZE);
    }

    public void Draw()
    {
        Vector2 position = Box.Position;

        UpdateAnimation();

        Raylib.DrawTexturePro(
            ChestTexture,
            SourceRec,
            new Rectangle(position.X - DRAWN_SIZE / 2, position.Y - DRAWN_SIZE / 2, DRAWN_SIZE, DRAWN_SIZE),
            Vector2.Zero,
            0f,
            Color.White
        );

        int textX = (int)position.X - TEXT_OFFSET_X;
        if (Box.IsOpen && Box.Weapon != null)
        {
            int textY = (int)position.Y - OPEN_TEXT_OFFSET_Y;
            Raylib.DrawText($"Time to disappear: {Math.Round(Box.WeaponTimeToDisappear, 0)}", textX, textY, FONT_SIZE, Color.Black);
            Raylib.DrawText($"Press F to accept {Box.Weapon.Name}", textX, textY + LINE_SPACING, FONT_SIZE, Color.Black);
        }
        else
        {
            Raylib.DrawText($"Press F ({Box.BoxPrice})", textX, (int)position.Y - PRICE_TEXT_OFFSET_Y, FONT_SIZE, Color.White);
        }
    }

    private void UpdateAnimation()
    {
        Timer += Raylib.GetFrameTime();
        if (Timer < FRAME_TIME)
            return;

        Timer = 0f;

        if (Box.IsOpen)
        {
            // Ouverture : on avance jusqu'à la dernière image, puis on reste dessus
            if (CurrentFrame < LAST_OPENING_FRAME) CurrentFrame++;
        }
        else if (CurrentFrame == LAST_OPENING_FRAME)
        {
            CurrentFrame = FIRST_CLOSING_FRAME;
        }
        else if (CurrentFrame >= FIRST_CLOSING_FRAME && CurrentFrame < LAST_CLOSING_FRAME)
        {
            CurrentFrame++;
        }
        else if (CurrentFrame == LAST_CLOSING_FRAME)
        {
            CurrentFrame = CLOSED_FRAME;
        }

        SourceRec.Y = CurrentFrame * FRAME_SIZE;
    }
}
