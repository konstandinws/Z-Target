public class RoundController
{
    // Réglages de difficulté
    private const int MAX_ZOMBIES_ON_SCREEN = 24;     // limite de zombies vivants en même temps
    private const float SPAWN_COOLDOWN = 1.0f;        // temps entre deux apparitions
    private const float ROUND_TRANSITION_DURATION = 5f; // pause entre deux manches

    private const int BASE_ZOMBIE_COUNT = 5;          // zombies à la manche 1
    private const float ZOMBIES_ADDED_PER_ROUND = 2.2f;
    private const float BASE_ZOMBIE_HEALTH = 50f;     // vie à la manche 1
    private const float HEALTH_ADDED_PER_ROUND = 20f;
    private const float BASE_ZOMBIE_SPEED = 50f;      // vitesse à la manche 1
    private const float SPEED_ADDED_PER_ROUND = 2f;
    private const float MAX_ZOMBIE_SPEED = 150f;

    // 0 = avant la première manche, puis 1, 2, 3...
    public int CurrentRound { get; private set; } = 0;
    public int zombiesToSpawn;      // Zombies à spawn total pour ce round
    public int zombiesSpawned;      // Zombies déjà apparus
    public int zombiesKilled;       // Zombies tués

    private ZombieController zombieController;
    private float spawnTimer = 0f;
    public float roundTransitionTimer = 0f;
    public bool isRoundTransitioning = false;

    public RoundController(ZombieController zombieController)
    {
        this.zombieController = zombieController;
        zombieController.ZombieKilled += OnZombieKilled;
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

        int currentAliveZombies = zombieController.Zombies.Count;

        // Spawn si on n'a pas atteint le quota ET pas trop de zombies à l'écran
        if (zombiesSpawned < zombiesToSpawn && currentAliveZombies < MAX_ZOMBIES_ON_SCREEN && spawnTimer <= 0f)
        {
            SpawnZombieForRound();
            zombiesSpawned++;
            spawnTimer = SPAWN_COOLDOWN;
        }

        // Fin du round si tout est spawné et tué
        if (zombiesSpawned >= zombiesToSpawn && zombieController.Zombies.Count == 0)
        {
            isRoundTransitioning = true;
            roundTransitionTimer = ROUND_TRANSITION_DURATION;
        }
    }

    private void OnZombieKilled(Zombie zombie, KillCause cause)
    {
        zombiesKilled++;
    }

    private void SpawnZombieForRound()
    {
        float zombieHealth = GetZombieHealthForRound(CurrentRound);
        float zombieSpeed = GetZombieSpeedForRound(CurrentRound);

        zombieController.CreateZombie(zombieHealth, zombieSpeed);
    }

    // Les formules partent de la manche 1 : (round - 1) vaut 0 à la première manche

    private int GetZombiesForRound(int round)
    {
        return BASE_ZOMBIE_COUNT + (int)((round - 1) * ZOMBIES_ADDED_PER_ROUND);
    }

    private float GetZombieHealthForRound(int round)
    {
        return BASE_ZOMBIE_HEALTH + (round - 1) * HEALTH_ADDED_PER_ROUND;
    }

    private float GetZombieSpeedForRound(int round)
    {
        return Math.Min(BASE_ZOMBIE_SPEED + (round - 1) * SPEED_ADDED_PER_ROUND, MAX_ZOMBIE_SPEED);
    }
}
