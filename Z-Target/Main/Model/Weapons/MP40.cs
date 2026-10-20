public class MP40 : Weapon
{
    public MP40()
    {
        Name = "MP40";
        Type = WeaponType.Rifle;
        Damage = 18;
        FireRate = 0.1f;
        BulletSpeed = 900f;
        ClipSize = 30;
        MaxAmmo = 120;
        ReserveAmmo = MaxAmmo;
        CurrentAmmo = ClipSize;
        ReloadTime = 1.8f;
        Points = 7;
        SpreadAngle = 1.4f;
    }
}
