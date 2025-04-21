using System.Numerics;
using Raylib_cs;
using System.Collections.Generic;

public class ZombieController
{
    public static List<Zombie> zombiesList = new List<Zombie>();
    private float collisionRadius = 25f;
    private float gridSize = 100f; // Taille des cellules de la grille
    private Player player;

    public ZombieController(Player player)
    {
        this.player = player;
        CreateZombies(10); // Exemple : 50 zombies
    }

    public void Update(float dt, Player player)
    {
        foreach (Zombie z in zombiesList)
        {
            z.Move(dt, player.Position);
        }

        HandleCollisions();
        HandleBulletCollisions();
    }

    public void CreateZombies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            zombiesList.Add(new Zombie());
        }
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
                    zombie.TakeDamage(bullet.Damage);
                    player.Score += bullet.points;
                    HUDView.Instance.AddFloatingPoints(bullet.points);
                    BulletController.Instance.BulletList.RemoveAt(j);
                    
                    if (!zombie.IsAlive)
                    {
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
}
