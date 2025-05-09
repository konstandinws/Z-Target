public class RPK : Weapon
{
    public RPK()
    {
        name = "RPK";
        Type = WeaponType.Rifle;
        Damage = 20;
        FireRate = 0.12f;
        BulletSpeed = 900f;
        clipSize = 75;
        maxAmmo = 340;  
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 3f;
        points = 15;
        spreadAngle = 1.4f;
    }
}
