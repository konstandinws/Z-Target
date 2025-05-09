using System.Numerics;
using Raylib_cs;
public class GLOCK : Weapon
{
    public GLOCK()
    {
        Name = "GLOCK";
        Type = WeaponType.Handgun;
        Damage = 12;
        FireRate = 0.3f;
        BulletSpeed = 800f;
        ClipSize = 15;
        MaxAmmo = 60;
        ReservAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 1.5f;
        Points = 15;
        SpreadAngle = 1.1f;
    }
}