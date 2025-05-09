using System.Numerics;
using Raylib_cs;
using System;

public class Boite{
    public Vector2 Position = new Vector2(0, 0);
    public int Width = 300;
    public int Height = 50;
    public bool IsOpen = false;
    public float WeaponTimeToDisappear = 10f;
    public int BoxPrice = 950;
    Random random = new Random();
    List<string> AvailableWeapons = new List<string>() {"AK47", "M4", "M1911", "FAMAS", "GALIL", "MP5",
     "GLOCK", "SPAS12", "OLYMPIA", "UMP45", "RPK", "AWP", "PYTHON", "MP40", "RAYGUN"};

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
        int index = random.Next(AvailableWeapons.Count);
        string weaponName = AvailableWeapons[index];

        switch (weaponName)
        {
            case "AK47":
                Weapon = new AK47();
                break;
            case "M4":
                Weapon = new M4();
                break;
            case "M1911":
                Weapon = new M1911();
                break;
            case "FAMAS":
                Weapon = new FAMAS();
                break;
            case "GALIL":
                Weapon = new GALIL();
                break;
            case "MP5":
                Weapon = new MP5();
                break;
            case "MP40":
                Weapon = new MP40();
                break;
            case "UMP45":
                Weapon = new UMP45();
                break;
            case "RPK":
                Weapon = new RPK();
                break;
            case "AWP":
                Weapon = new AWP();
                break;
            case "PYTHON":
                Weapon = new PYTHON();
                break;
            case "RAYGUN":
                Weapon = new RAYGUN();
                break;
            case "OLYMPIA":
                Weapon = new OLYMPIA();
                break;
            case "GLOCK":
                Weapon = new GLOCK();
                break;
            case "SPAS12":
                Weapon = new SPAS12();
                break;
        }
    }

    public void AcceptWeapon(Player player)
    {
        if(player.SecondaryWeapon == null){
            player.SecondaryWeapon = player.MainWeapon;
            player.MainWeapon = Weapon;
        }
        else{
            player.MainWeapon = Weapon;
        }
    }

    public void Close()
    {
        IsOpen = false;
        Weapon = null;
        WeaponTimeToDisappear = 10f;
    }
}