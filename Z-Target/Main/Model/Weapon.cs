using System.Numerics;
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

    public abstract void Shoot(Vector2 position, Vector2 direction, BulletController bulletController);
    public abstract void Reload();
}