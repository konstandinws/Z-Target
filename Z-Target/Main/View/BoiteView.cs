using Raylib_cs;
using System.Numerics;

public class BoiteView : IView
{
    private Boite box;
    private int boxSize = 40;

    public BoiteView(Boite box)
    {
        this.box = box;
    }

    public void Draw()
    {
        Color color = box.IsOpen ? Color.Gold : Color.Brown;
        Raylib.DrawRectangle((int)box.Position.X - boxSize/2, (int)box.Position.Y - boxSize/2, boxSize, boxSize, color);

        if (box.IsOpen && box.Weapon != null)
        {

            Raylib.DrawText($"Time to disappear: {box.WeaponTimeToDisappear}", (int)box.Position.X - 40, (int)box.Position.Y - 40, 16, Color.Black);
            Raylib.DrawText($"Press R to accept {box.Weapon.name}", (int)box.Position.X - 40, (int)box.Position.Y - 20, 16, Color.Black);
        }
        else
        {
            Raylib.DrawText($"Press F ({box.BoxPrice})", (int)box.Position.X - 40, (int)box.Position.Y - 60, 16, Color.White);
        }
    }
}