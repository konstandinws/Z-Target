using Raylib_cs;

public class BonusView : IView
{
    private const int FONT_SIZE = 10;
    private const int TEXT_OFFSET_X = 20;
    private const int TEXT_OFFSET_Y = 10;

    private BonusController BonusController;

    public BonusView(BonusController bonusController)
    {
        this.BonusController = bonusController;
    }

    public void Draw()
    {
        // Afficher tous les bonus au sol
        foreach (Bonus bonus in BonusController.BonusList)
        {
            DrawBonus(bonus);
        }
    }

    private void DrawBonus(Bonus bonus)
    {
        Raylib.DrawCircleV(bonus.Position, Bonus.RADIUS, GetColor(bonus.Type));
        Raylib.DrawText(bonus.Type.ToString(), (int)bonus.Position.X - TEXT_OFFSET_X, (int)bonus.Position.Y - TEXT_OFFSET_Y, FONT_SIZE, Color.Black);
    }

    // Couleur de chaque bonus (utilisée aussi par le HUD)
    public static Color GetColor(BonusType type)
    {
        return type switch
        {
            BonusType.MaxAmmo => Color.SkyBlue,
            BonusType.InstaKill => Color.Red,
            BonusType.DoublePoints => Color.Gold,
            BonusType.Nuke => Color.Green,
            _ => Color.Gray,
        };
    }
}
