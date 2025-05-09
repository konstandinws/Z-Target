public class UMP45 : Weapon
{
    public UMP45()
    {
        name = "UMP45";
        Type = WeaponType.Rifle;
        Damage = 15;
        FireRate = 0.08f;
        BulletSpeed = 900f;
        clipSize = 30;
        maxAmmo = 120;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 1.3f;
        points = 7;
        spreadAngle = 1.2f;
    }
}
