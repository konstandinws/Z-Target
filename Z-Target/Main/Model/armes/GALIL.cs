
public class GALIL : Weapon
{
    public GALIL()
    {
        name = "GALIL";
        Type = WeaponType.Rifle;
        Damage = 30;
        FireRate = 0.1f;
        BulletSpeed = 900f;
        clipSize = 30;
        maxAmmo = 120;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 2f;
        points = 10;
        spreadAngle = 1.6f;
    }
}
