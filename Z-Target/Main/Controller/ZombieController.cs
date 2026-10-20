using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;

// Ce qui a tué un zombie (permet par ex. de ne pas faire tomber de bonus avec la Nuke)
public enum KillCause
{
    Bullet,
    Knife,
    Nuke
}

public class ZombieController
{
    public List<Zombie> Zombies { get; } = new List<Zombie>();

    // Prévient les autres systèmes (manches, bonus...) qu'un zombie vient de mourir
    public event Action<Zombie, KillCause>? ZombieKilled;

    private Player player;
    private BulletController bulletController;

    public ZombieController(Player player, BulletController bulletController)
    {
        this.player = player;
        this.bulletController = bulletController;
    }

    public void Update(float dt)
    {
        foreach (Zombie z in Zombies)
        {
            z.Move(dt, player.Position);
        }

        HandleCollisions();
        HandleBulletCollisions();
        HandlePlayerCollisions();
    }

    public void CreateZombie(float health, float speed)
    {
        Vector2 spawnPosition = GetRandomSpawnPosition();
        Zombies.Add(new Zombie(spawnPosition, health, speed));
    }

    private Vector2 GetRandomSpawnPosition()
    {
        // Spawn autour du joueur, hors de l'écran, dans une direction au hasard
        float angle = (float)(Random.Shared.NextDouble() * 2 * Math.PI);
        Vector2 direction = new Vector2(MathF.Cos(angle), MathF.Sin(angle));

        return player.Position + direction * GameConfig.Zombie.SpawnDistance;
    }

    // Inflige des dégâts. Si le zombie meurt, il est retiré de la liste et ZombieKilled est déclenché.
    public void DamageZombie(Zombie zombie, int damage, KillCause cause)
    {
        zombie.TakeDamage(damage);
        if (!zombie.IsAlive)
        {
            Zombies.Remove(zombie);
            ZombieKilled?.Invoke(zombie, cause);
        }
    }

    public void HandleKnifeDamage(Zombie zombie, int damage)
    {
        player.AddPoints(GameConfig.Knife.Points);
        DamageZombie(zombie, damage, KillCause.Knife);
    }

    // Tue tous les zombies (bonus Nuke)
    public void KillAllZombies()
    {
        List<Zombie> killed = new List<Zombie>(Zombies);
        Zombies.Clear();
        foreach (Zombie zombie in killed)
        {
            ZombieKilled?.Invoke(zombie, KillCause.Nuke);
        }
    }

    private void HandleBulletCollisions()
    {
        List<Bullet> bullets = bulletController.BulletList;

        for (int i = Zombies.Count - 1; i >= 0; i--)
        {
            Zombie zombie = Zombies[i];
            for (int j = bullets.Count - 1; j >= 0; j--)
            {
                Bullet bullet = bullets[j];
                if (Vector2.Distance(zombie.Position, bullet.Position) < zombie.Radius + bullet.Radius)
                {
                    // InstaKill : dégâts suffisants pour tuer en un coup
                    int damage = player.HasActiveBonus(BonusType.InstaKill) ? (int)zombie.MaxHealth : bullet.Damage;

                    player.AddPoints(bullet.points);
                    bullets.RemoveAt(j);
                    DamageZombie(zombie, damage, KillCause.Bullet);

                    if (!zombie.IsAlive)
                        break;
                }
            }
        }
    }

    private void HandleCollisions()
    {
        // Vérifier les collisions entre chaque paire de zombies
        for (int i = 0; i < Zombies.Count; i++)
        {
            for (int j = i + 1; j < Zombies.Count; j++)
            {
                Zombie zombie1 = Zombies[i];
                Zombie zombie2 = Zombies[j];

                float distance = Vector2.Distance(zombie1.Position, zombie2.Position);
                float minDist = GameConfig.Zombie.SeparationRadius * 2;

                if (distance < minDist && distance > 0.001f)
                {
                    // Calculer la direction de poussée
                    Vector2 pushDir = Vector2.Normalize(zombie1.Position - zombie2.Position);
                    float pushAmount = (minDist - distance) * 0.5f;

                    // Appliquer la poussée aux deux zombies
                    zombie1.Position += pushDir * pushAmount;
                    zombie2.Position -= pushDir * pushAmount;
                }
            }
        }
    }

    private void HandlePlayerCollisions()
    {
        // Si le joueur est mort, ne pas vérifier les collisions
        if (!player.IsAlive)
            return;

        foreach (Zombie zombie in Zombies)
        {
            float distance = Vector2.Distance(zombie.Position, player.Position);
            float minDistance = zombie.Radius + player.HitBoxRadius;

            if (distance < minDistance)
            {
                // Le zombie touche le joueur
                player.TakeDamage(GameConfig.Zombie.ContactDamage);

                // Repousser légèrement le joueur loin du zombie
                Vector2 pushDirection = Vector2.Normalize(player.Position - zombie.Position);
                player.Position += pushDirection * GameConfig.Zombie.PlayerKnockback;

                // Ne vérifier qu'une collision à la fois pour éviter de perdre trop de vie rapidement
                return;
            }
        }
    }
}
