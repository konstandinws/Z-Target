using System.Numerics;
using Raylib_cs;

public class M4 : Weapon
{
    public M4()
    {
        Name = "M4";
        Type = WeaponType.Rifle;
        Damage = 25;
        FireRate = 0.1f;
        BulletSpeed = 1000f;
        ClipSize = 30;
        MaxAmmo = 120;
        ReservAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 2f;
        Points = 10;
        SpreadAngle = 1.2f;
    }
}
