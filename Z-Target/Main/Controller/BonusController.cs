using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

public class BonusController
{
    public List<Bonus> BonusList = new List<Bonus>();

    private static readonly BonusType[] DroppableBonuses =
    {
        BonusType.MaxAmmo,
        BonusType.InstaKill,
        BonusType.DoublePoints,
        BonusType.Nuke,
    };

    private Player player;
    private ZombieController zombieController;

    public BonusController(Player player, ZombieController zombieController)
    {
        this.player = player;
        this.zombieController = zombieController;
        zombieController.ZombieKilled += OnZombieKilled;
    }

    private void OnZombieKilled(Zombie zombie, KillCause cause)
    {
        // Pas de bonus sur les zombies tués par une Nuke (sinon une Nuke en ferait tomber plein d'autres)
        if (cause == KillCause.Nuke)
            return;

        if (Random.Shared.NextDouble() < GameConfig.Bonus.DropChance)
        {
            BonusType type = DroppableBonuses[Random.Shared.Next(DroppableBonuses.Length)];
            CreateBonus(zombie.Position, type);
        }
    }

    public void CreateBonus(Vector2 position, BonusType type)
    {
        BonusList.Add(new Bonus(position, type));
    }

    public void Update(float deltaTime)
    {
        for (int i = BonusList.Count - 1; i >= 0; i--)
        {
            Bonus bonus = BonusList[i];
            bonus.Update(deltaTime);

            // Vérifie si le joueur ramasse le bonus
            if (!bonus.IsCollected && Vector2.Distance(bonus.Position, player.Position) < GameConfig.Bonus.PickupRadius + player.HitBoxRadius)
            {
                bonus.Collect();
                ApplyBonus(bonus.Type);
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

    private void ApplyBonus(BonusType type)
    {
        switch (type)
        {
            case BonusType.MaxAmmo:
                player.MainWeapon.ReplenishAmmo();
                player.SecondaryWeapon?.ReplenishAmmo();
                break;
            case BonusType.InstaKill:
                player.EnableInstaKill(GameConfig.Bonus.EffectDuration);
                break;
            case BonusType.DoublePoints:
                player.EnableDoublePoints(GameConfig.Bonus.EffectDuration);
                break;
            case BonusType.Nuke:
                zombieController.KillAllZombies();
                break;
        }
    }
}
