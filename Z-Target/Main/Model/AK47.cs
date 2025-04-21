using System.Numerics;
using Raylib_cs;
public class AK47 : Weapon
{
    public AK47()
    {
        name = "AK47";
        Damage = 30;
        FireRate = 0.1f;
        BulletSpeed = 800f;
        clipSize = 30;
        maxAmmo = 20;
        currentAmmo = clipSize;
        reloadTime = 2f;
        points = 10;
    }

    public override void Shoot(Vector2 position, Vector2 direction, BulletController bulletController)
    {
        if (Raylib.GetTime() - lastShotTime >= FireRate)
        {
            bulletController.ShootBullet(direction, Damage, points);
            lastShotTime = (float)Raylib.GetTime();
            currentAmmo--;
        }
    }

    public override void Reload()
    {
        maxAmmo += currentAmmo;

        if (maxAmmo >= clipSize) {
            currentAmmo = clipSize;
            maxAmmo -= clipSize;
        } else {
            currentAmmo = maxAmmo;
            maxAmmo = 0;
        }
    }
}