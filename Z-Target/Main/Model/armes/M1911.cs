using System.Numerics;
using Raylib_cs;
public class M1911 : Weapon
{
    public M1911()
    {
        name = "M1911";
        Type = WeaponType.Handgun;
        Damage = 10;
        FireRate = 0.2f;
        BulletSpeed = 8f;
        maxAmmo = 60;
        reservAmmo = 60;
        currentAmmo = 7;
        clipSize = 7;
        reloadTime = 1.5f;
        points = 5;
    }

    // public override void Shoot(Vector2 position, Vector2 direction, BulletController bulletController)
    // {
    //     if (Raylib.GetTime() - lastShotTime >= FireRate)
    //     {
    //         bulletController.ShootBullet(direction, Damage, points);
    //         lastShotTime = (float)Raylib.GetTime();
    //         currentAmmo--;
    //     }
    // }
}
