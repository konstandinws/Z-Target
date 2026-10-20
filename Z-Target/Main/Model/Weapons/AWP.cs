using System.Numerics;
using Raylib_cs;

public class AWP : Weapon
{
    public float spreadAngle = 0f; // dispersion en degrés

    public AWP()
    {
        Name = "AWP";
        Type = WeaponType.Rifle;
        Damage = 100;
        FireRate = 1.5f;
        BulletSpeed = 1200f;
        ClipSize = 5;
        MaxAmmo = 15;
        ReserveAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 2.2f;
        Points = 12;
    }
}