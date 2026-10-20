public class RPK : Weapon
{
    public RPK()
    {
        Name = "RPK";
        Type = WeaponType.Rifle;
        Damage = 20;
        FireRate = 0.12f;
        BulletSpeed = 900f;
        ClipSize = 75;
        MaxAmmo = 340;  
        ReserveAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 3f;
        Points = 15;
        SpreadAngle = 1.4f;
    }
}
