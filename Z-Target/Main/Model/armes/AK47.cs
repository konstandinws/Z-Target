using System.Numerics;
using Raylib_cs;

public class AK47 : Weapon
{

    public AK47()
    {
        name = "AK-47";
        Type = WeaponType.Rifle;
        Damage = 30;
        FireRate = 0.12f;
        BulletSpeed = 900f;
        clipSize = 30;
        maxAmmo = 90;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 2.2f;
        points = 12;
        spreadAngle = 2f;
    }
}