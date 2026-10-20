using System.Numerics;
using Raylib_cs;

public enum WeaponType
{
    Rifle,
    Handgun,
    Shotgun
}

public abstract class Weapon{

    public string Name = "";
    public WeaponType Type { get; protected set; }
    public int Damage;
    public float FireRate;
    public float BulletSpeed;
    protected float LastShotTime = 0;
    public int MaxAmmo;
    public int ReserveAmmo;
    public int CurrentAmmo;
    public int ClipSize;
    public float ReloadTimer = 0f;
    public int Points;
    public float SpreadAngle = 1; // largeur totale du cône de dispersion, en degrés
    public int PelletsPerShot = 1; // > 1 pour les fusils à pompe

    public float ReloadTime;

    // position = position du tireur, target = point visé (position de la souris dans le monde)
    public virtual void Shoot(Vector2 position, Vector2 target, BulletController bulletController)
    {
        if (Raylib.GetTime() - LastShotTime < FireRate || CurrentAmmo <= 0)
            return;

        Vector2 toTarget = target - position;
        if (toTarget.LengthSquared() < 0.0001f)
            return; // souris pile sur le joueur : pas de direction possible

        Vector2 aimDirection = Vector2.Normalize(toTarget);

        for (int i = 0; i < PelletsPerShot; i++)
        {
            bulletController.ShootBullet(BulletSpread(aimDirection, SpreadAngle), BulletSpeed, Damage, Points);
        }

        LastShotTime = (float)Raylib.GetTime();
        CurrentAmmo--;
    }

    public virtual void Reload()
    {
        ReserveAmmo += CurrentAmmo;

        if (ReserveAmmo >= ClipSize) {
            CurrentAmmo = ClipSize;
            ReserveAmmo -= ClipSize;
        } else {
            CurrentAmmo = ReserveAmmo;
            ReserveAmmo = 0;
        }
    }


    public void ReplenishAmmo(){
        ReserveAmmo = MaxAmmo;
    }

    // Fait tourner une direction d'un angle aléatoire entre -degrees/2 et +degrees/2
    protected Vector2 BulletSpread(Vector2 direction, float degrees)
    {
        float spread = ((float)Random.Shared.NextDouble() - 0.5f) * degrees;
        float radians = spread * (MathF.PI / 180f);
        float cos = MathF.Cos(radians);
        float sin = MathF.Sin(radians);
        return new Vector2(direction.X * cos - direction.Y * sin, direction.X * sin + direction.Y * cos);
    }
}
