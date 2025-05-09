public class MP40 : Weapon
{
    public MP40()
    {
        name = "MP40";
        Type = WeaponType.Rifle;
        Damage = 18;
        FireRate = 0.1f;
        BulletSpeed = 900f;
        clipSize = 30;
        maxAmmo = 120;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 1.8f;
        points = 7;
        spreadAngle = 1.4f;
    }
}
