using System.Numerics;
using Raylib_cs;
public class OLYMPIA : Weapon
{
    public int pelletsPerShot = 8; // nombre de plombs par tir

    public OLYMPIA()
    {
        Name = "OLYMPIA";   
        Type = WeaponType.Shotgun;
        Damage = 30;
        FireRate = 1f;
        BulletSpeed = 700f;
        ClipSize = 2;
        MaxAmmo = 12;
        ReservAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 2.5f;
        Points = 20;
        SpreadAngle = 6f;
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
