using System.Numerics;
using Raylib_cs;
public class M1911 : Weapon
{
    public M1911()
    {
        name = "M1911";
        Damage = 15;
        FireRate = 0.3f;
        BulletSpeed = 800f;
        clipSize = 7;
        maxAmmo = 0;
        currentAmmo = clipSize;
        reloadTime = 1.5f;
        points = 30;
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
