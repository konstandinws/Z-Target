using System.Numerics;
using Raylib_cs;
public class SPAS12 : Weapon
{
    public SPAS12()
    {
        Name = "SPAS-12";   
        Type = WeaponType.Shotgun;
        Damage = 20;
        FireRate = 0.8f;
        BulletSpeed = 700f;
        ClipSize = 8;
        MaxAmmo = 48;
        ReservAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 2.5f;
        Points = 20;
        PelletsPerShot = 8; // nombre de plombs par tir
        SpreadAngle = 10f;
    }
}
