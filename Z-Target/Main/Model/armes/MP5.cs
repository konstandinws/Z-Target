using System.Numerics;
using Raylib_cs;

public class MP5 : Weapon
{
    public MP5()
    {
        name = "MP5";
        Type = WeaponType.Rifle;
        Damage = 18;
        FireRate = 0.05f;
        BulletSpeed = 900f;
        clipSize = 30;
        maxAmmo = 120;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 1.5f;
        points = 7;
        spreadAngle = 1.4f;
    }
}
