using System.Numerics;
using Raylib_cs;

public enum WeaponType
{
    Rifle,
    Handgun,
    Shotgun
}

public abstract class Weapon{

    public string name;
    public WeaponType Type { get; protected set; }
    public int Damage;
    public float FireRate;
    public float BulletSpeed;
    protected float lastShotTime = 0;
    public int maxAmmo;
    public int reservAmmo;
    public int currentAmmo;
    public int clipSize;
    public float reloadTimer = 0f;
    public int points;
    public float spreadAngle = 1;

    public float reloadTime;

    public virtual void Shoot(Vector2 position, Vector2 direction, BulletController bulletController)
    {
        if (Raylib.GetTime() - lastShotTime >= FireRate)
        {
            Vector2 spreadDirection = BulletSpread(direction, spreadAngle);
            bulletController.ShootBullet(spreadDirection, Damage, points);
            lastShotTime = (float)Raylib.GetTime();
            currentAmmo--;
        }
    }
    public virtual void Reload()
    {
        reservAmmo += currentAmmo;

        if (reservAmmo >= clipSize) {
            currentAmmo = clipSize;
            reservAmmo -= clipSize;
        } else {
            currentAmmo = reservAmmo;
            reservAmmo = 0;
        }
    }   


    public void resplenishAmmo(){
        reservAmmo = maxAmmo;
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