using System.Numerics;
using Raylib_cs;
public class OLYMPIA : Weapon
{
    public OLYMPIA()
    {
        Name = "OLYMPIA";   
        Type = WeaponType.Shotgun;
        Damage = 30;
        FireRate = 1f;
        BulletSpeed = 700f;
        ClipSize = 2;
        MaxAmmo = 12;
        ReserveAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 2.5f;
        Points = 20;
        PelletsPerShot = 8; // nombre de plombs par tir
        SpreadAngle = 6f;
    }
}
