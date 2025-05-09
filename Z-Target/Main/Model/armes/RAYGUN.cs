public class RAYGUN : Weapon
{
    public RAYGUN()
    {
        Name = "RAYGUN";
        Type = WeaponType.Handgun;
        Damage = 100;
        FireRate = 0.4f;
        BulletSpeed = 700f;
        ClipSize = 11;
        MaxAmmo = 45;
        ReservAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 1.8f;
        Points = 7;
        SpreadAngle = 1.4f;
    }
}

