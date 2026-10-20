using Raylib_cs;
using System.Numerics;

public class BonusView : IView
{
    private const float SPRITE_SCALE = 3f; // icônes 16 px -> 48 px au sol

    private BonusController BonusController;
    private Dictionary<BonusType, Texture2D> Sprites;

    public BonusView(TextureLoader textures, BonusController bonusController)
    {
        this.BonusController = bonusController;
        Sprites = LoadSprites(textures);
    }

    // Icône de chaque bonus (utilisée aussi par le HUD)
    public static Dictionary<BonusType, Texture2D> LoadSprites(TextureLoader textures)
    {
        BonusAssets paths = GameConfig.Assets.Bonus;
        return new Dictionary<BonusType, Texture2D>
        {
            [BonusType.MaxAmmo] = textures.Load(paths.MaxAmmo),
            [BonusType.InstaKill] = textures.Load(paths.InstaKill),
            [BonusType.DoublePoints] = textures.Load(paths.DoublePoints),
            [BonusType.Nuke] = textures.Load(paths.Nuke),
        };
    }

    public void Draw()
    {
        // Afficher tous les bonus au sol
        foreach (Bonus bonus in BonusController.BonusList)
        {
            Texture2D sprite = Sprites[bonus.Type];
            Vector2 size = new Vector2(sprite.Width, sprite.Height) * SPRITE_SCALE;
            Raylib.DrawTextureEx(sprite, bonus.Position - size / 2, 0f, SPRITE_SCALE, Color.White);
        }
    }
}
