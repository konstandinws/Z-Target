using System.Numerics;
using Raylib_cs;
public class M1911 : Weapon
{
    public M1911()
    {
        name = "M1911";
        Type = WeaponType.Handgun;
        Damage = 12;
        FireRate = 0.3f;
        BulletSpeed = 800f;
        clipSize = 7;
        maxAmmo = 56;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 1.5f;
        points = 15;
    }
}
