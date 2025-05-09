using Raylib_cs;

public class BonusView : IView
{
    private Player Player;

    public BonusView(Player player)
    {
        this.Player = player;
    }

    public void Draw()
    {
        // Afficher tous les bonus au sol
        foreach (Bonus bonus in BonusController.Instance.BonusList)
        {
            DrawBonus(bonus);
        }
    }

    private void DrawBonus(Bonus bonus)
    {
        Color color = GetBonusColor(bonus.Type);
        Raylib.DrawCircleV(bonus.Position, 20f, color); // Taille du cercle pour le bonus
        Raylib.DrawText(bonus.Type.ToString(), (int)bonus.Position.X - 20, (int)bonus.Position.Y - 10, 10, Color.Black);
    }

    private Color GetBonusColor(BonusType type)
    {
        return type switch
        {
            BonusType.MaxAmmo => Color.SkyBlue,
            BonusType.InstaKill => Color.Red,
            BonusType.DoublePoints => Color.Gold,
            BonusType.Nuke => Color.Green,
        };
    }
}
