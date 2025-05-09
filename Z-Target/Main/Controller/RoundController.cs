public class RoundController
{
    public int CurrentRound { get; private set; } = -1;
    public int zombiesToSpawn;      // Zombies à spawn total pour ce round
    public int zombiesSpawned;      // Zombies déjà apparus
    public int zombiesKilled;       // Zombies tués
    public static RoundController Instance { get; private set; }  
    private const int maxZombiesOnScreen = 24; // Limite d'affichage simultané

    private ZombieController zombieController;
    private float spawnCooldown = 1.0f; // temps entre les spawns
    private float spawnTimer = 0f;
    public float roundTransitionTimer = 0f;
    public bool isRoundTransitioning = false;

    public RoundController()
    {
        if (Instance != null)
            throw new Exception("Une seule instance de RoundController est autorisée.");
        Instance = this;
    }

    public void SetZombieController(ZombieController zombieController)
    {
        this.zombieController = zombieController;
    }

    public void StartRound()
    {
        zombiesToSpawn = GetZombiesForRound(CurrentRound);
        zombiesSpawned = 0;
        zombiesKilled = 0;
        spawnTimer = 0f;
        isRoundTransitioning = false;
    }

    public void Update(float deltaTime)
    {
        if (isRoundTransitioning)
        {
            roundTransitionTimer -= deltaTime;
            if (roundTransitionTimer <= 0f)
            {
                CurrentRound++;
                StartRound();
            }
            return;
        }

        spawnTimer -= deltaTime;

        int currentAliveZombies = ZombieController.zombiesList.Count;

        // Spawn si on n'a pas atteint le quota ET moins de 24 zombies à l'écran
        if (zombiesSpawned < zombiesToSpawn && currentAliveZombies < maxZombiesOnScreen && spawnTimer <= 0f)
        {
            SpawnZombieForRound();
            zombiesSpawned++;
            spawnTimer = spawnCooldown;
        }

        // Fin du round si tout est spawné et tué
        if (zombiesSpawned >= zombiesToSpawn && ZombieController.zombiesList.Count == 0)
        {
            isRoundTransitioning = true;
            roundTransitionTimer = 5f; // 5 secondes de délai
        }
    }


    public void OnZombieKilled()
    {
        zombiesKilled++;
    }

    private void SpawnZombieForRound()
    {
        float zombieHealth = GetZombieHealthForRound(CurrentRound);
        float zombieSpeed = GetZombieSpeedForRound(CurrentRound);

        zombieController.CreateZombies(zombieHealth, zombieSpeed);
    }

    private int GetZombiesForRound(int round)
    {
        // Exemple de scaling (tu peux ajuster)
        return 5 + (int)(round * 2.2f);
    }

    private float GetZombieHealthForRound(int round)
    {
        // Vie de base + 20% par round
        return 50f + (round - 1) * 20f;
    }

    private float GetZombieSpeedForRound(int round)
    {
        // Vitesse de base = 50, augmente de 2 unités par round (cap à 150 max)
        return Math.Min(50f + (round - 1) * 2f, 150f);
    }
}
