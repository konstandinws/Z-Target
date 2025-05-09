using System.Numerics;
using Raylib_cs;

public class AWP : Weapon
{
    public float spreadAngle = 0f; // dispersion en degrés

    public AWP()
    {
        name = "AWP";
        Type = WeaponType.Rifle;
        Damage = 100;
        FireRate = 1.5f;
        BulletSpeed = 1200f;
        clipSize = 5;
        maxAmmo = 15;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 2.2f;
        points = 12;
    }
}