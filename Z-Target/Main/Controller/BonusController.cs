using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

public class BonusController
{
    public static BonusController Instance = new BonusController();

    public List<Bonus> BonusList = new List<Bonus>();

    private float BonusRadius = 25f;  // Rayon du bonus pour collisions
    private float BonusDuration = 10f;

    private BonusController() { }

    public void CreateBonus(Vector2 position, BonusType type)
    {
        Bonus bonus = new Bonus(position, type);
        BonusList.Add(bonus);
    }

    public void Update(float deltaTime, Player player)
    {
        for (int i = BonusList.Count - 1; i >= 0; i--)
        {
            Bonus bonus = BonusList[i];
            bonus.Update(deltaTime);

            // Vérifie si le joueur ramasse le bonus
            if (!bonus.IsCollected && Vector2.Distance(bonus.Position, player.Position) < BonusRadius + player.Size.X/2)
            {
                bonus.Collect();
                ApplyBonus(player, bonus.Type);
                // Une fois ramassé, on le supprime de la liste
                BonusList.RemoveAt(i);
            }
            // Si le bonus est expiré (délai au sol dépassé)
            else if (bonus.IsExpired())
            {
                BonusList.RemoveAt(i);
            }
        }
    }

    private void ApplyBonus(Player player, BonusType type)
    {
        switch (type)
        {
            case BonusType.MaxAmmo:
                player.MainWeapon.resplenishAmmo();  // Remplit les munitions
                player.SecondaryWeapon.resplenishAmmo();  // Remplit les munitions
                break;
            case BonusType.InstaKill:
                player.EnableInstaKill(BonusDuration);  // InstaKill pendant 10 sec
                break;
            case BonusType.DoublePoints:
                player.EnableDoublePoints(BonusDuration);  // Double points 10 sec
                break;
            case BonusType.Nuke:
                // Effet Nuke : tue tous les zombies
                ZombieController.zombiesList.Clear();  // Tue tous les zombies
                // Ou si le ZombieController a une instance statique:
                // ZombieController.Instance.KillAllZombies();
                break;
        }
    }
}