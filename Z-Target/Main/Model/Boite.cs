using System.Numerics;
using Raylib_cs;
using System;

public class Boite{
    private const float WEAPON_DISPLAY_DURATION = 10f; // temps pour accepter l'arme avant que la boîte se referme
    private const int BOX_PRICE = 950;

    public Vector2 Position = Vector2.Zero;
    public bool IsOpen = false;
    public float WeaponTimeToDisappear = WEAPON_DISPLAY_DURATION;
    public int BoxPrice = BOX_PRICE;

    // Toutes les armes que la boîte peut donner (pour en ajouter une, il suffit de l'ajouter ici)
    private static readonly Func<Weapon>[] AvailableWeapons =
    {
        () => new AK47(), () => new M4(), () => new M1911(), () => new FAMAS(), () => new GALIL(),
        () => new MP5(), () => new GLOCK(), () => new SPAS12(), () => new OLYMPIA(), () => new UMP45(),
        () => new RPK(), () => new AWP(), () => new PYTHON(), () => new MP40(), () => new RAYGUN(),
    };

    public Weapon? Weapon = null;

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
        Weapon = AvailableWeapons[Random.Shared.Next(AvailableWeapons.Length)]();
    }

    public void AcceptWeapon(Player player)
    {
        if (Weapon == null) return;

        if (player.SecondaryWeapon == null)
        {
            player.SecondaryWeapon = player.MainWeapon;
        }
        player.MainWeapon = Weapon;
    }

    public void Close()
    {
        IsOpen = false;
        Weapon = null;
        WeaponTimeToDisappear = WEAPON_DISPLAY_DURATION;
    }
}
