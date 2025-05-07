public class M4 : Weapon
{
    public M4()
    {
        name = "M4";
        Damage = 25;
        FireRate = 0.1f;
        BulletSpeed = 800f;
        clipSize = 30;
        maxAmmo = 210;
        reservAmmo = maxAmmo;
        currentAmmo = clipSize;
        reloadTime = 2f;
        points = 10;
    }

}   
