public class PYTHON : Weapon
{
    public PYTHON()
    {
        name = "PYTHON";
        Type = WeaponType.Handgun;
        Damage = 25;
        FireRate = 0.3f;
        BulletSpeed = 900f;
        clipSize = 6;
        maxAmmo = 36;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 1.8f;
        points = 7;
        spreadAngle = 1.1f;
    }
}

