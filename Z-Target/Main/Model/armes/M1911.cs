using System.Numerics;
using Raylib_cs;
public class M1911 : Weapon
{
    public M1911()
    {
        Name = "M1911";
        Type = WeaponType.Handgun;
        Damage = 12;
        FireRate = 0.3f;
        BulletSpeed = 800f;
        ClipSize = 7;
        MaxAmmo = 56;
        ReservAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 1.5f;
        Points = 15;
    }
}
