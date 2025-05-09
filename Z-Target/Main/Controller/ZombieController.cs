using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;

public class ZombieController
{
    public static List<Zombie> zombiesList = new List<Zombie>();
    private float collisionRadius = 25f;
    private float gridSize = 100f; // Taille des cellules de la grille
    private Player player;
    private List<Zombie> zombiesToRemove = new List<Zombie>();
    private RoundController roundController;
    public ZombieController(Player player, RoundController roundController)
    {
        this.player = player;
        this.roundController = roundController;
    }

    public void Update(float dt, Player player)
    {
        foreach (Zombie z in zombiesList)
        {
            z.Move(dt, player.Position);
        }

        HandleCollisions();
        HandleBulletCollisions();
        HandlePlayerCollisions(player);

        // Supprimer les zombies morts après toutes les mises à jour
        foreach (var zombie in zombiesToRemove)
        {
            zombiesList.Remove(zombie);
        }
        zombiesToRemove.Clear();
    }

    public void CreateZombies(float health, float speed)
    {
        Vector2 spawnPosition = GetRandomSpawnPosition();

        Zombie zombie = new Zombie(spawnPosition, health, speed);
        zombiesList.Add(zombie);
    }

    private Vector2 GetRandomSpawnPosition()
{
    // Spawn autour de la map (hors de l'écran par exemple)
    float distanceFromPlayer = 1000f;  // 600 unités du joueur
    double angle = Raylib.GetRandomValue(0, 360) * Math.PI / 180.0;

    float x = player.Position.X + (float)Math.Cos(angle) * distanceFromPlayer;
    float y = player.Position.Y + (float)Math.Sin(angle) * distanceFromPlayer;

    return new Vector2(x, y);
}

    public void HandleKnifeDamage(Zombie zombie, int damage)
    {
        zombie.Health -= damage;
        if (zombie.Health <= 0)
        {
            zombie.IsAlive = false;
            zombiesList.Remove(zombie);
            roundController.OnZombieKilled();
        }
        
        // Ajouter des points pour chaque coup de couteau
        int points = 50; // Points de base pour un coup de couteau
        if (player.HasActiveBonus(BonusType.DoublePoints))
        {
            points *= 2;
        }
        player.Score += points;
        HUDView.Instance.AddFloatingPoints(points);
    }

    private void HandleBulletCollisions()
    {
        for (int i = zombiesList.Count - 1; i >= 0; i--)
        {
            Zombie zombie = zombiesList[i];
            for (int j = BulletController.Instance.BulletList.Count - 1; j >= 0; j--)
            {
                Bullet bullet = BulletController.Instance.BulletList[j];
                if (Vector2.Distance(zombie.Position, bullet.Position) < zombie.radius + bullet.Radius)
                {
                    BonusType? bonus = null;
                    
                    // Vérifier si InstaKill est actif - tuer instantanément
                    if (player.HasActiveBonus(BonusType.InstaKill))
                    {
                        bonus = zombie.HandleDamage((int)zombie.MaxHealth); // Force la mort
                    }
                    else 
                    {
                        // Dégâts normaux
                        bonus = zombie.HandleDamage(bullet.Damage);
                    }
                    
                    // Calculer les points avec bonus DoublePoints si actif
                    int points = bullet.points;
                    if (player.HasActiveBonus(BonusType.DoublePoints))
                    {
                        points *= 2;
                    }
                    
                    // Attribuer les points
                    player.Score += points;
                    HUDView.Instance.AddFloatingPoints(points);
                    
                    BulletController.Instance.BulletList.RemoveAt(j);
                    
                    if (!zombie.IsAlive)
                    {
                        if (bonus.HasValue)
                        {
                            BonusController.Instance.CreateBonus(zombie.Position, bonus.Value);
                        }
                        zombiesList.RemoveAt(i);
                        roundController.OnZombieKilled();
                        break;
                    }
                }
            }
        }
    }

    private void HandleCollisions()
    {
        // Vérifier les collisions entre chaque paire de zombies
        for (int i = 0; i < zombiesList.Count; i++)
        {
            for (int j = i + 1; j < zombiesList.Count; j++)
            {
                Zombie zombie1 = zombiesList[i];
                Zombie zombie2 = zombiesList[j];
                
                float distance = Vector2.Distance(zombie1.Position, zombie2.Position);
                float minDist = collisionRadius * 2;

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

    // Méthode pour tuer tous les zombies (utilisée par le bonus Nuke)
    public void KillAllZombies()
    {
        for (int i = zombiesList.Count - 1; i >= 0; i--)
        {
            // Possibilité de créer une animation ou effet spécial ici
            zombiesList.RemoveAt(i);
        }
    }

    private void HandlePlayerCollisions(Player player)
    {
        // Si le joueur est mort, ne pas vérifier les collisions
        if (!player.IsAlive())
            return;
            
        float playerRadius = player.Size.X / 2;
        
        foreach (Zombie zombie in zombiesList)
        {
            float distance = Vector2.Distance(zombie.Position, player.Position);
            float minDistance = zombie.radius + playerRadius;
            
            if (distance < minDistance)
            {
                // Le zombie touche le joueur
                player.TakeDamage(1); // Infliger 1 point de dégâts
                
                // Repousser légèrement le joueur loin du zombie
                Vector2 pushDirection = Vector2.Normalize(player.Position - zombie.Position);
                player.Position += pushDirection * 20f; // Pousser le joueur de 20 unités
                
                // Ne vérifier qu'une collision à la fois pour éviter de perdre trop de vie rapidement
                return;
            }
        }
    }
}
