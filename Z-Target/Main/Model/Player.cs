using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;
using System;

public class Player : IDamageable
{
    public enum AnimationState
    {
        Idle,
        Walking,
        Running,
        Shooting,
        Reloading,
        KnifeAttack
    }

    public enum WeaponAnimationState
    {
        Idle,
        Shooting,
        Reloading
    }

    private const float SIZE = 50f;
    private const int STARTING_SCORE = 500; // points au départ (comme dans CoD Zombies)

    public AnimationState CurrentAnimationState { get; private set; } = AnimationState.Idle;
    public WeaponAnimationState CurrentWeaponState { get; private set; } = WeaponAnimationState.Idle;
    public Vector2 Position { get; set; } = GameConfig.MapCenter; // le joueur commence au centre de la map
    public Vector2 Size { get; set; } = new Vector2(SIZE, SIZE);
    public float HitBoxRadius => Size.X / 2;
    public Color Color { get; set; } = Color.Blue;
    public Vector2 Direction = Vector2.Zero;
    public float AngleToMouse;
    private Vector2 Velocity = Vector2.Zero;
    private Vector2 Acceleration = Vector2.Zero;

    private float AccelerationSpeed = 10000f;
    private float MaxSpeed = 300f;
    private float SprintMultiplier = 1.6f;  // Multiplicateur de vitesse en sprint
    private bool isSprinting = false;
    private float Friction = 0.85f; // augmenter fait effet patinoire
    private float MinVelocity = 0.01f; // vitesse minimale avant de s'arreter
    private const float WALK_ANIMATION_MIN_SPEED = 28.3f; // en dessous, on affiche l'animation idle

    // Variables pour la gestion du sprint
    private float MaxSprintDuration = 3.0f;    // Durée maximale du sprint en secondes
    private float CurrentSprintDuration = 3.0f; // Durée actuelle de sprint restante
    private float SprintRecoveryRate = 0.5f;    // Vitesse de récupération du sprint (par seconde)
    private float SprintDepletionRate = 1.0f;   // Vitesse d'épuisement du sprint (par seconde)
    private bool CanSprint = true;              // Indique si le joueur peut sprinter

    public Weapon MainWeapon;
    public Weapon? SecondaryWeapon;

    public bool IsReloading = false;

    public int Score = STARTING_SCORE;

    // Déclenché à chaque gain de points (le HUD s'en sert pour afficher "+X")
    public event Action<int>? PointsEarned;

    // Système de santé
    private int MaxHealth = 3;
    private int CurrentHealth;
    private float InvincibilityTime = 0.5f; // Temps d'invincibilité après être touché (en secondes)
    private float CurrentInvincibilityTime = 0f;
    private bool isInvincible = false;

    // Régénération de santé
    private float HealthRegenerationDelay = 2.0f; // Délai avant de commencer à régénérer (en secondes)
    private float CurrentRegenerationDelay = 0f;  // Compteur actuel pour le délai de régénération
    private bool IsRegenerating = false;         // Indique si le joueur est en train de régénérer sa santé
    private float RegenerationRate = 1.0f;       // Vitesse de régénération en points de vie par seconde
    private float RegenerationAccumulator = 0f;  // Accumulateur pour la régénération (pour les points de vie fractionnés)

    // Bonus actifs et temps restant pour chacun
    private Dictionary<BonusType, float> ActiveBonuses = new Dictionary<BonusType, float>();

    private float KnifeAttackTimer = 0f;
    public const float KNIFE_ATTACK_DURATION = 0.3f;
    // Avancement de l'attaque au couteau, de 0 (début) à 1 (fin) — utilisé par la vue pour l'animation
    public float KnifeAttackProgress => Math.Clamp(KnifeAttackTimer / KNIFE_ATTACK_DURATION, 0f, 1f);

    public Player(Weapon startingWeapon)
    {
        CurrentHealth = MaxHealth;
        MainWeapon = startingWeapon;
    }

    public void Move(float deltaTime)
    {
        Acceleration = Direction * AccelerationSpeed;

        Velocity += Acceleration * deltaTime;

        // Mise à jour de l'état d'animation
        bool isMoving = Velocity.LengthSquared() > WALK_ANIMATION_MIN_SPEED * WALK_ANIMATION_MIN_SPEED;
        UpdateAnimationState(deltaTime, isMoving, isSprinting, CurrentWeaponState == WeaponAnimationState.Shooting, IsReloading);

        // Gestion du sprint
        UpdateSprintState(deltaTime);

        // Appliquer le multiplicateur de sprint à la vitesse max si nécessaire
        float currentMaxSpeed = isSprinting ? MaxSpeed * SprintMultiplier : MaxSpeed;

        if (Velocity.LengthSquared() > currentMaxSpeed * currentMaxSpeed)
            Velocity = Vector2.Normalize(Velocity) * currentMaxSpeed;

        Velocity *= Friction;

        if (Velocity.LengthSquared() < MinVelocity * MinVelocity)
            Velocity = Vector2.Zero;

        // Mettre à jour la position
        Position += Velocity * deltaTime;

        // Limiter la position du joueur à la zone de jeu au centre de la map
        float minPos = GameConfig.PLAY_AREA_OFFSET + HitBoxRadius;
        float maxPos = GameConfig.PLAY_AREA_OFFSET + GameConfig.PLAY_AREA_SIZE - HitBoxRadius;
        Position = new Vector2(Math.Clamp(Position.X, minPos, maxPos), Math.Clamp(Position.Y, minPos, maxPos));

        UpdateBonusTimers(deltaTime);
        UpdateInvincibility(deltaTime);
        UpdateHealthRegeneration(deltaTime);
    }

    private void UpdateSprintState(float deltaTime)
    {
        // Si le joueur sprinte et a encore de l'endurance
        if (isSprinting && CanSprint)
        {
            CurrentSprintDuration -= SprintDepletionRate * deltaTime;

            // Vérifier si l'endurance est épuisée
            if (CurrentSprintDuration <= 0)
            {
                CurrentSprintDuration = 0;
                CanSprint = false;
                isSprinting = false;
            }
        }
        // Si le joueur ne sprinte pas, récupérer de l'endurance
        else if (!isSprinting)
        {
            CurrentSprintDuration += SprintRecoveryRate * deltaTime;

            if (CurrentSprintDuration >= MaxSprintDuration)
            {
                CurrentSprintDuration = MaxSprintDuration;
                CanSprint = true;
            }
        }
    }

    private void UpdateBonusTimers(float deltaTime)
    {
        List<BonusType> expiredBonuses = new List<BonusType>();

        foreach (var bonus in ActiveBonuses)
        {
            float remaining = bonus.Value - deltaTime;

            if (remaining <= 0)
                expiredBonuses.Add(bonus.Key);
            else
                ActiveBonuses[bonus.Key] = remaining;
        }

        foreach (BonusType expired in expiredBonuses)
        {
            ActiveBonuses.Remove(expired);
        }
    }

    // Ajoute des points au score en tenant compte du bonus Double Points
    public void AddPoints(int basePoints)
    {
        int points = HasActiveBonus(BonusType.DoublePoints) ? basePoints * 2 : basePoints;
        Score += points;
        PointsEarned?.Invoke(points);
    }

    public void UpdateAngle(Vector2 mousePosition)
    {
        AngleToMouse = MathF.Atan2(mousePosition.Y - Position.Y, mousePosition.X - Position.X) * (180f / MathF.PI);
    }

    public bool HasActiveBonus(BonusType type)
    {
        return ActiveBonuses.ContainsKey(type);
    }

    public List<BonusType> GetActiveBonuses()
    {
        return new List<BonusType>(ActiveBonuses.Keys);
    }

    public float GetBonusRemainingTime(BonusType type)
    {
        return ActiveBonuses.TryGetValue(type, out float remaining) ? remaining : 0f;
    }

    public void EnableInstaKill(float duration)
    {
        ActiveBonuses[BonusType.InstaKill] = duration;
    }

    public void EnableDoublePoints(float duration)
    {
        ActiveBonuses[BonusType.DoublePoints] = duration;
    }

    public void DisableBonus(BonusType type)
    {
        ActiveBonuses.Remove(type);
    }

    public void DisableAllBonuses()
    {
        ActiveBonuses.Clear();
    }

    public void SetSprinting(bool sprinting)
    {
        // Ne permettre le sprint que s'il reste de l'endurance et qu'il est autorisé
        if (sprinting && CurrentSprintDuration > 0 && CanSprint)
        {
            isSprinting = true;
        }
        else if (!sprinting)
        {
            isSprinting = false;
        }
    }

    public bool IsSprinting()
    {
        return isSprinting;
    }

    public float GetSprintRatio()
    {
        // Retourne une valeur entre 0 et 1 représentant la quantité d'endurance restante
        return CurrentSprintDuration / MaxSprintDuration;
    }

    private void UpdateInvincibility(float deltaTime)
    {
        if (isInvincible)
        {
            CurrentInvincibilityTime -= deltaTime;
            if (CurrentInvincibilityTime <= 0)
            {
                isInvincible = false;
            }
        }
    }

    private void UpdateHealthRegeneration(float deltaTime)
    {
        // Si la santé est déjà au maximum, pas besoin de régénérer
        if (CurrentHealth >= MaxHealth)
        {
            IsRegenerating = false;
            CurrentRegenerationDelay = 0f;
            return;
        }

        // Si le joueur vient d'être touché (est invincible), réinitialiser le délai de régénération
        if (isInvincible)
        {
            CurrentRegenerationDelay = 0f;
            IsRegenerating = false;
            return;
        }

        // Si le joueur n'est pas en train de régénérer, attendre le délai avant de commencer
        if (!IsRegenerating)
        {
            CurrentRegenerationDelay += deltaTime;

            if (CurrentRegenerationDelay >= HealthRegenerationDelay)
            {
                IsRegenerating = true;
                RegenerationAccumulator = 0f;
            }
        }
        // Si le joueur est en train de régénérer, augmenter sa santé progressivement
        else
        {
            RegenerationAccumulator += RegenerationRate * deltaTime;

            // Dès que l'accumulateur dépasse 1, ajouter les points de vie entiers
            if (RegenerationAccumulator >= 1.0f)
            {
                int pointsToAdd = (int)RegenerationAccumulator;
                RegenerationAccumulator -= pointsToAdd;

                CurrentHealth += pointsToAdd;

                if (CurrentHealth > MaxHealth)
                {
                    CurrentHealth = MaxHealth;
                    IsRegenerating = false;
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        // Si le joueur est invincible, ne pas prendre de dégâts
        if (isInvincible)
            return;

        CurrentHealth -= damage;

        // Activer l'invincibilité temporaire
        isInvincible = true;
        CurrentInvincibilityTime = InvincibilityTime;

        // Réinitialiser le délai de régénération
        CurrentRegenerationDelay = 0f;
        IsRegenerating = false;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
        }
    }

    public bool IsAlive => CurrentHealth > 0;

    public int GetCurrentHealth()
    {
        return CurrentHealth;
    }

    public int GetMaxHealth()
    {
        return MaxHealth;
    }

    public float GetHealthRatio()
    {
        return (float)CurrentHealth / MaxHealth;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    private void UpdateAnimationState(float deltaTime, bool isMoving, bool isSprinting, bool isShooting, bool isReloading)
    {
        if (CurrentAnimationState == AnimationState.KnifeAttack)
        {
            KnifeAttackTimer += deltaTime;
            if (KnifeAttackTimer >= KNIFE_ATTACK_DURATION)
            {
                CurrentAnimationState = AnimationState.Idle;
            }
            return;
        }

        if (isReloading)
            CurrentAnimationState = AnimationState.Reloading;
        else if (isShooting)
            CurrentAnimationState = AnimationState.Shooting;
        else if (isSprinting)
            CurrentAnimationState = AnimationState.Running;
        else if (isMoving)
            CurrentAnimationState = AnimationState.Walking;
        else
            CurrentAnimationState = AnimationState.Idle;
    }

    public void SetWeaponState(WeaponAnimationState state)
    {
        CurrentWeaponState = state;
    }

    public void StartReload()
    {
        CurrentWeaponState = WeaponAnimationState.Reloading;
    }

    public void StartShooting()
    {
        CurrentWeaponState = WeaponAnimationState.Shooting;
    }

    public void StopShooting()
    {
        if (CurrentWeaponState == WeaponAnimationState.Shooting)
        {
            CurrentWeaponState = WeaponAnimationState.Idle;
        }
    }

    public void FinishReload()
    {
        if (CurrentWeaponState == WeaponAnimationState.Reloading)
        {
            CurrentWeaponState = WeaponAnimationState.Idle;
        }
    }

    public void StartKnifeAttack()
    {
        CurrentAnimationState = AnimationState.KnifeAttack;
        KnifeAttackTimer = 0f;
    }
}
