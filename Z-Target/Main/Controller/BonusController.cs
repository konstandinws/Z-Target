using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

public class BonusController
{
    public List<Bonus> BonusList = new List<Bonus>();

    private const float DROP_BONUS_CHANCE = 0.20f; // 20 % de chances qu'un zombie tué fasse tomber un bonus

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

        if (Random.Shared.NextDouble() < DROP_BONUS_CHANCE)
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
            if (!bonus.IsCollected && Vector2.Distance(bonus.Position, player.Position) < Bonus.PICKUP_RADIUS + player.HitBoxRadius)
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
                player.MainWeapon.resplenishAmmo();
                player.SecondaryWeapon?.resplenishAmmo();
                break;
            case BonusType.InstaKill:
                player.EnableInstaKill(Bonus.EFFECT_DURATION);
                break;
            case BonusType.DoublePoints:
                player.EnableDoublePoints(Bonus.EFFECT_DURATION);
                break;
            case BonusType.Nuke:
                zombieController.KillAllZombies();
                break;
        }
    }
}
