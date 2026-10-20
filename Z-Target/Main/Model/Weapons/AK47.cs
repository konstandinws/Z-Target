using System.Numerics;
using Raylib_cs;

public class AK47 : Weapon
{

    public AK47()
    {
        Name = "AK-47";
        Type = WeaponType.Rifle;
        Damage = 30;
        FireRate = 0.12f;
        BulletSpeed = 900f;
        ClipSize = 30;
        MaxAmmo = 90;
        ReserveAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 2.2f;
        Points = 12;
        SpreadAngle = 2f;
    }
}