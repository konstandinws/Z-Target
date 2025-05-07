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

    public ZombieController(Player player)
    {
        this.player = player;
        CreateZombies(1); // Exemple : 50 zombies
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

    public void CreateZombies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            zombiesList.Add(new Zombie());
        }
    }

    public void HandleKnifeDamage(Zombie zombie, int damage)
    {
        zombie.Health -= damage;
        if (zombie.Health <= 0)
        {
            zombie.IsAlive = false;
            zombiesList.Remove(zombie);
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
                        break;
                    }
                }
            }
        }
    }

    private void HandleCollisions()
    {
        Dictionary<(int, int), List<Zombie>> spatialGrid = new Dictionary<(int, int), List<Zombie>>();

        // Placer les zombies dans une grille
        foreach (var zombie in zombiesList)
        {
            var cell = GetCell(zombie.Position);
            if (!spatialGrid.ContainsKey(cell))
                spatialGrid[cell] = new List<Zombie>();

            spatialGrid[cell].Add(zombie);
        }

        // Détecter collisions dans chaque cellule et les voisines
        foreach (var cell in spatialGrid.Keys)
        {
            List<Zombie> currentCellZombies = spatialGrid[cell];

            for (int i = 0; i < currentCellZombies.Count; i++)
            {
                for (int j = i + 1; j < currentCellZombies.Count; j++)
                {
                    ResolveCollision(currentCellZombies[i], currentCellZombies[j]);
                }

                foreach (var offset in neighborOffsets)
                {
                    var neighborCell = (cell.Item1 + offset.Item1, cell.Item2 + offset.Item2);
                    if (!spatialGrid.ContainsKey(neighborCell)) continue;

                    foreach (var neighborZombie in spatialGrid[neighborCell])
                    {
                        ResolveCollision(currentCellZombies[i], neighborZombie);
                    }
                }
            }
        }
    }

    private (int, int) GetCell(Vector2 position)
    {
        return ((int)(position.X / gridSize), (int)(position.Y / gridSize));
    }

    private void ResolveCollision(Zombie a, Zombie b)
    {
        float distance = Vector2.Distance(a.Position, b.Position);
        float minDist = collisionRadius * 2;

        if (distance < minDist && distance > 0.001f)
        {
            Vector2 pushDir = Vector2.Normalize(a.Position - b.Position);
            float pushAmount = (minDist - distance) * 0.5f;
            a.Position += pushDir * pushAmount;
            b.Position -= pushDir * pushAmount;
        }
    }

    private static readonly (int, int)[] neighborOffsets = new (int, int)[]
    {
        (0, 1), (1, 0), (1, 1), (-1, 0), (0, -1), (-1, -1), (1, -1), (-1, 1)
    };

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
