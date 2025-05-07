using System.Numerics;
using Raylib_cs;

public class Boite{
    public Vector2 Position = new Vector2(0, 0);
    public int width = 300;
    public int height = 50;
    public bool IsOpen = false;
    public float WeaponTimeToDisappear = 10f;
    public int BoxPrice = 10;
    Random random = new Random();
    List<Weapon> availableWeapons = new List<Weapon>() {new AK47(), new M4(), new M1911()};

    public Weapon Weapon = null;

    public bool CanOpen(Player player)
    {
        return !IsOpen && player.Score >= BoxPrice;
    }

    public void TimeToDisappear(float dt)
    {
        if (IsOpen)
        {
            WeaponTimeToDisappear -= dt;
        }
    }

    public void Open(Player player)
    {
        if (!CanOpen(player)) return;

        player.Score -= BoxPrice;
        IsOpen = true;

        // Tire une arme au hasard
        int index = random.Next(availableWeapons.Count);
        Weapon = availableWeapons[index];
    }
    public void AcceptWeapon(Player player)
    {
        player.mainWeapon = Weapon;
    }

    public void Close()
    {
        IsOpen = false;
        Weapon = null;
        WeaponTimeToDisappear = 10f;
    }
    
}