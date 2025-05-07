using System.Numerics;
using Raylib_cs;

public enum WeaponType
{
    Rifle,
    Handgun
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

    public float reloadTime;

    public virtual void Shoot(Vector2 position, Vector2 direction, BulletController bulletController)
    {
        if (Raylib.GetTime() - lastShotTime >= FireRate)
        {
            bulletController.ShootBullet(direction, Damage, points);
            lastShotTime = (float)Raylib.GetTime();
            currentAmmo--;
        }
    }
    public virtual void Reload(){
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
}