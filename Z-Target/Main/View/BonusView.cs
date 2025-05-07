using Raylib_cs;

public class BonusView : IView
{
    private Player player;
    private const float bonusIconSize = 30f; // Taille du carré affiché pour le bonus actif
    private const int textOffset = 40; // Décalage du texte de statut

    public BonusView(Player player)
    {
        this.player = player;
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
