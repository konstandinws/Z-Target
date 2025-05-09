using System.Numerics;
using Raylib_cs;
public class SPAS12 : Weapon
{
    public int pelletsPerShot = 8; // nombre de plombs par tir

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
        SpreadAngle = 10f;
    }
    public override void Shoot(Vector2 position, Vector2 direction, BulletController bulletController)
    {
        if (Raylib.GetTime() - LastShotTime >= FireRate && CurrentAmmo > 0)
        {
            Random rand = new Random();
            for (int i = 0; i < pelletsPerShot; i++)
            {
                // Générer une petite variation d'angle pour chaque plomb
                    float spread = ((float)rand.NextDouble() - 0.5f) * SpreadAngle; // entre -spreadAngle/2 et +spreadAngle/2
                    Vector2 spreadDirection = BulletSpread(direction, spread);

                bulletController.ShootBullet(spreadDirection, Damage, Points);
            }

            LastShotTime = (float)Raylib.GetTime();
            CurrentAmmo--;
        }
    }
}
