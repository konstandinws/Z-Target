public class UMP45 : Weapon
{
    public UMP45()
    {
        Name = "UMP45";
        Type = WeaponType.Rifle;
        Damage = 15;
        FireRate = 0.08f;
        BulletSpeed = 900f;
        ClipSize = 30;
        MaxAmmo = 120;
        ReservAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 1.3f;
        Points = 7;
        SpreadAngle = 1.2f;
    }
}
