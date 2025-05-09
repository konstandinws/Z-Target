using System.Numerics;
using Raylib_cs;
public class GLOCK : Weapon
{
    public GLOCK()
    {
        name = "GLOCK";
        Type = WeaponType.Handgun;
        Damage = 12;
        FireRate = 0.3f;
        BulletSpeed = 800f;
        clipSize = 15;
        maxAmmo = 60;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 1.5f;
        points = 15;
        spreadAngle = 1.1f;
    }
}