using System.Numerics;
using Raylib_cs;

public class M4 : Weapon
{
    public M4()
    {
        name = "M4";
        Type = WeaponType.Rifle;
        Damage = 25;
        FireRate = 0.1f;
        BulletSpeed = 1000f;
        clipSize = 30;
        maxAmmo = 120;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 2f;
        points = 10;
        spreadAngle = 1.2f;
    }
}
