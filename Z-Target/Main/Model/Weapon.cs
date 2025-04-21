using System.Numerics;
using Raylib_cs;
public abstract class Weapon{

    public string name;
    public int Damage;
    public float FireRate;
    public float BulletSpeed;
    protected float lastShotTime = 0;
    public int maxAmmo;
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