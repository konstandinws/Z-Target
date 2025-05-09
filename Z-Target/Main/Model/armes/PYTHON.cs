public class PYTHON : Weapon
{
    public PYTHON()
    {
        Name = "PYTHON";
        Type = WeaponType.Handgun;
        Damage = 25;
        FireRate = 0.3f;
        BulletSpeed = 900f;
        ClipSize = 6;
        MaxAmmo = 36;
        ReservAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 1.8f;
        Points = 7;
        SpreadAngle = 1.1f;
    }
}

