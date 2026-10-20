using System.Numerics;
using Raylib_cs;
using System;

public class MysteryBox{
    public Vector2 Position = new Vector2(GameConfig.MysteryBox.PositionX, GameConfig.MysteryBox.PositionY);
    public bool IsOpen = false;
    public float WeaponTimeToDisappear = GameConfig.MysteryBox.WeaponDisplayDuration;
    public int BoxPrice = GameConfig.MysteryBox.Price;

    public Weapon? Weapon = null;

    public bool CanOpen(Player player)
    {
        return !IsOpen && player.Score >= BoxPrice;
    }

    public void TimeToDisappear(float dt)
    {
        if (IsOpen)
        {
            WeaponTimeToDisappear -= dt;
        }
    }

    public void Open(Player player)
    {
        if (!CanOpen(player)) return;

        player.Score -= BoxPrice;
        IsOpen = true;

        // Tire une arme au hasard parmi celles listées dans config.jsonc
        List<string> weapons = GameConfig.MysteryBox.Weapons;
        Weapon = WeaponFactory.Create(weapons[Random.Shared.Next(weapons.Count)]);
    }

    public void AcceptWeapon(Player player)
    {
        if (Weapon == null) return;

        if (player.SecondaryWeapon == null)
        {
            player.SecondaryWeapon = player.MainWeapon;
        }
        player.MainWeapon = Weapon;
    }

    public void Close()
    {
        IsOpen = false;
        Weapon = null;
        WeaponTimeToDisappear = GameConfig.MysteryBox.WeaponDisplayDuration;
    }
}
