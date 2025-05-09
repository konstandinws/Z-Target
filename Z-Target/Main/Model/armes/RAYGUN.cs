public class RAYGUN : Weapon
{
    public RAYGUN()
    {
        name = "RAYGUN";
        Type = WeaponType.Handgun;
        Damage = 100;
        FireRate = 0.4f;
        BulletSpeed = 700f;
        clipSize = 11;
        maxAmmo = 45;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 1.8f;
        points = 7;
        spreadAngle = 1.4f;
    }
}

