public class FAMAS : Weapon
{
    public FAMAS()
    {
        name = "FAMAS";
        Type = WeaponType.Rifle;
        Damage = 22;
        FireRate = 0.1f;
        BulletSpeed = 900f;
        clipSize = 30;
        maxAmmo = 120;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 2f;
        points = 10;
        spreadAngle = 1.1f;
    }
}
