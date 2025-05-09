public class FAMAS : Weapon
{
    public FAMAS()
    {
        Name = "FAMAS";
        Type = WeaponType.Rifle;
        Damage = 22;
        FireRate = 0.1f;
        BulletSpeed = 900f;
        ClipSize = 30;
        MaxAmmo = 120;
        ReservAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 2f;
        Points = 10;
        SpreadAngle = 1.1f;
    }
}
