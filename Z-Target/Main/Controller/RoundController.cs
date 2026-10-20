public class RoundController
{
    // Réglages de difficulté (voir la section "Rounds" de config.jsonc)
    private static RoundSettings Settings => GameConfig.Rounds;

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
        if (zombiesSpawned < zombiesToSpawn && currentAliveZombies < Settings.MaxZombiesOnScreen && spawnTimer <= 0f)
        {
            SpawnZombieForRound();
            zombiesSpawned++;
            spawnTimer = Settings.SpawnCooldown;
        }

        // Fin du round si tout est spawné et tué
        if (zombiesSpawned >= zombiesToSpawn && zombieController.Zombies.Count == 0)
        {
            isRoundTransitioning = true;
            roundTransitionTimer = Settings.TransitionDuration;
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
        return Settings.BaseZombieCount + (int)((round - 1) * Settings.ZombiesAddedPerRound);
    }

    private float GetZombieHealthForRound(int round)
    {
        return Settings.BaseZombieHealth + (round - 1) * Settings.HealthAddedPerRound;
    }

    private float GetZombieSpeedForRound(int round)
    {
        return Math.Min(Settings.BaseZombieSpeed + (round - 1) * Settings.SpeedAddedPerRound, Settings.MaxZombieSpeed);
    }
}
