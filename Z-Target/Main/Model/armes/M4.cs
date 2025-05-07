public class M4 : Weapon
{
    public M4()
    {
        name = "M4";
        Type = WeaponType.Rifle;
        Damage = 15;
        FireRate = 0.08f;
        BulletSpeed = 12f;
        maxAmmo = 120;
        reservAmmo = 120;
        currentAmmo = 30;
        clipSize = 30;
        reloadTime = 1.8f;
        points = 8;
    }
}   
