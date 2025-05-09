using System.Numerics;
using Raylib_cs;
public class OLYMPIA : Weapon
{
    public int pelletsPerShot = 8; // nombre de plombs par tir

    public OLYMPIA()
    {
        name = "OLYMPIA";   
        Type = WeaponType.Shotgun;
        Damage = 30;
        FireRate = 1f;
        BulletSpeed = 700f;
        clipSize = 2;
        maxAmmo = 12;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 2.5f;
        points = 20;
        spreadAngle = 6f;
    }
    public override void Shoot(Vector2 position, Vector2 direction, BulletController bulletController)
    {
        if (Raylib.GetTime() - lastShotTime >= FireRate && currentAmmo > 0)
        {
            Random rand = new Random();
            for (int i = 0; i < pelletsPerShot; i++)
            {
                // Générer une petite variation d'angle pour chaque plomb
                    float spread = ((float)rand.NextDouble() - 0.5f) * spreadAngle; // entre -spreadAngle/2 et +spreadAngle/2
                    Vector2 spreadDirection = BulletSpread(direction, spread);

                bulletController.ShootBullet(spreadDirection, Damage, points);
            }

            lastShotTime = (float)Raylib.GetTime();
            currentAmmo--;
        }
    }
}
