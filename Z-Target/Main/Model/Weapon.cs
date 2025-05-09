using System.Numerics;
using Raylib_cs;

public enum WeaponType
{
    Rifle,
    Handgun,
    Shotgun
}

public abstract class Weapon{

    public string Name;
    public WeaponType Type { get; protected set; }
    public int Damage;
    public float FireRate;
    public float BulletSpeed;
    protected float LastShotTime = 0;
    public int MaxAmmo;
    public int ReservAmmo;
    public int CurrentAmmo;
    public int ClipSize;
    public float ReloadTimer = 0f;
    public int Points;
    public float SpreadAngle = 1;

    public float ReloadTime;

    public virtual void Shoot(Vector2 position, Vector2 direction, BulletController bulletController)
    {
        if (Raylib.GetTime() - LastShotTime >= FireRate)
        {
            Vector2 spreadDirection = BulletSpread(direction, SpreadAngle);
            bulletController.ShootBullet(spreadDirection, Damage, Points);
            LastShotTime = (float)Raylib.GetTime();
            CurrentAmmo--;
        }
    }
    public virtual void Reload()
    {
        ReservAmmo += CurrentAmmo;

        if (ReservAmmo >= ClipSize) {
            CurrentAmmo = ClipSize;
            ReservAmmo -= ClipSize;
        } else {
            CurrentAmmo = ReservAmmo;
            ReservAmmo = 0;
        }
    }   


    public void resplenishAmmo(){
        ReservAmmo = MaxAmmo;
    }

    protected Vector2 BulletSpread(Vector2 v, float degrees)
    {   
        Random rand = new Random(); 
        float spread = ((float)rand.NextDouble() - 0.5f) * degrees; // entre -spreadAngle/2 et +spreadAngle/2
        float radians = spread * (float)(Math.PI / 180f);
        float cos = (float)Math.Cos(radians);
        float sin = (float)Math.Sin(radians);
        return new Vector2(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
    }
}