using System.Numerics;
using Raylib_cs;

public class MP5 : Weapon
{
    public MP5()
    {
        Name = "MP5";
        Type = WeaponType.Rifle;
        Damage = 18;
        FireRate = 0.05f;
        BulletSpeed = 900f;
        ClipSize = 30;
        MaxAmmo = 120;
        ReserveAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 1.5f;
        Points = 7;
        SpreadAngle = 1.4f;
    }
}
