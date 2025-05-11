using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;
using System;

public class Player
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

    public AnimationState CurrentAnimationState { get; private set; } = AnimationState.Idle;
    public WeaponAnimationState CurrentWeaponState { get; private set; } = WeaponAnimationState.Idle;
    public Vector2 Position { get; set; } = new Vector2(GameConfig.ScreenWidth / 2, GameConfig.ScreenHeight / 2);
    public Vector2 Size { get; set; } = new Vector2(50, 50);
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
    
    // Variables pour la gestion du sprint
    private float MaxSprintDuration = 3.0f;    // Durée maximale du sprint en secondes
    private float CurrentSprintDuration = 3.0f; // Durée actuelle de sprint restante
    private float SprintRecoveryRate = 0.5f;    // Vitesse de récupération du sprint (par seconde)
    private float SprintDepletionRate = 1.0f;   // Vitesse d'épuisement du sprint (par seconde)
    private bool CanSprint = true;              // Indique si le joueur peut sprinter

    public Weapon MainWeapon;
    public Weapon SecondaryWeapon;

    public bool IsReloading = false;

    public int Score = 0;

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
    
    // Remplacer le bonus unique par un Dictionary pour suivre plusieurs bonus actifs
    private Dictionary<BonusType, float> ActiveBonuses = new Dictionary<BonusType, float>();
    
    private float KnifeAttackTimer = 0f;
    private const float KNIFE_ATTACK_DURATION = 0.3f;

    
    private const int PLAY_AREA_OFFSET = (GameConfig.MAP_SIZE - GameConfig.PLAY_AREA_SIZE) / 2;  // 600 pixels de marge de chaque côté

    public Player()
    {
        CurrentHealth = MaxHealth;
        // Centrer le joueur sur la map
        Position = new Vector2(GameConfig.MAP_SIZE / 2, GameConfig.MAP_SIZE / 2);
    }
    
    public void Move(float deltaTime)
    {
        Acceleration = Vector2.Zero;
        Acceleration = Direction * AccelerationSpeed;
        
        Velocity += Acceleration * deltaTime;

        // Mise à jour de l'état d'animation
        UpdateAnimationState(Velocity.LengthSquared() > MinVelocity * MinVelocity + 800.0f, isSprinting, CurrentWeaponState == WeaponAnimationState.Shooting, IsReloading);

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

        // Limiter la position du joueur à la zone de jeu de 2000x2000 pixels au centre de la map
        Position = new Vector2(
            Math.Clamp(Position.X, PLAY_AREA_OFFSET + Size.X/2, PLAY_AREA_OFFSET + GameConfig.PLAY_AREA_SIZE - Size.X/2),
            Math.Clamp(Position.Y, PLAY_AREA_OFFSET + Size.Y/2, PLAY_AREA_OFFSET + GameConfig.PLAY_AREA_SIZE - Size.Y/2)
        );
        
        // Mettre à jour les timers de tous les bonus actifs
        UpdateBonusTimers(deltaTime);
        
        // Mettre à jour le temps d'invincibilité
        UpdateInvincibility(deltaTime);
        
        // Gérer la régénération de santé
        UpdateHealthRegeneration(deltaTime);
    }
    
    private void UpdateSprintState(float deltaTime)
    {
        // Si le joueur sprinte et a encore de l'endurance
        if (isSprinting && CanSprint)
        {
            // Réduire l'endurance de sprint
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
            // Augmenter progressivement l'endurance de sprint
            CurrentSprintDuration += SprintRecoveryRate * deltaTime;
            
            // Limiter à la durée maximale
            if (CurrentSprintDuration >= MaxSprintDuration)
            {
                CurrentSprintDuration = MaxSprintDuration;
                CanSprint = true;
            }
        }
    }
    
    // Méthode pour mettre à jour les timers de bonus
    private void UpdateBonusTimers(float deltaTime)
    {
        List<BonusType> expiredBonuses = new List<BonusType>();
        
        foreach (var bonus in ActiveBonuses)
        {
            BonusType bonusType = bonus.Key;
            float remaining = bonus.Value - deltaTime;
            
            if (remaining <= 0)
            {
                expiredBonuses.Add(bonusType);
            }
            else
            {
                ActiveBonuses[bonusType] = remaining;
            }
        }
        
        // Supprimer les bonus expirés
        foreach (BonusType expired in expiredBonuses)
        {
            ActiveBonuses.Remove(expired);
        }
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
        if (ActiveBonuses.ContainsKey(type))
        {
            return ActiveBonuses[type];
        }
        return 0f;
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
        if (ActiveBonuses.ContainsKey(type))
        {
            ActiveBonuses.Remove(type);
        }
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
            
            // Commencer la régénération si le délai est écoulé
            if (CurrentRegenerationDelay >= HealthRegenerationDelay)
            {
                IsRegenerating = true;
                RegenerationAccumulator = 0f;
            }
        }
        // Si le joueur est en train de régénérer, augmenter sa santé progressivement
        else
        {
            // Accumuler la régénération en fonction du temps écoulé
            RegenerationAccumulator += RegenerationRate * deltaTime;
            
            // Si l'accumulateur dépasse 1, ajouter un point de vie
            if (RegenerationAccumulator >= 1.0f)
            {
                int pointsToAdd = (int)RegenerationAccumulator;
                RegenerationAccumulator -= pointsToAdd;
                
                CurrentHealth += pointsToAdd;
                
                // Limiter la santé au maximum
                if (CurrentHealth > MaxHealth)
                {
                    CurrentHealth = MaxHealth;
                    IsRegenerating = false; // Arrêter la régénération quand la santé est pleine
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
            // Le joueur est mort
            CurrentHealth = 0;
            // Logique de mort du joueur à implémenter
        }
    }
    
    public bool IsAlive()
    {
        return CurrentHealth > 0;
    }
    
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

    private void UpdateAnimationState(bool isMoving, bool isSprinting, bool isShooting, bool isReloading)
    {
        if (CurrentAnimationState == AnimationState.KnifeAttack)
        {
            KnifeAttackTimer += Raylib.GetFrameTime();
            if (KnifeAttackTimer >= KNIFE_ATTACK_DURATION)
            {
                CurrentAnimationState = AnimationState.Idle;
            }
            return;
        }

        if (isReloading)
        {
            CurrentAnimationState = AnimationState.Reloading;
        }
        else if (isShooting)
        {
            CurrentAnimationState = AnimationState.Shooting;
        }
        else if (isSprinting)
        {
            CurrentAnimationState = AnimationState.Running;
        }
        else if (isMoving)
        {
            CurrentAnimationState = AnimationState.Walking;
        }
        else
        {
            CurrentAnimationState = AnimationState.Idle;
        }
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

    public void StopKnifeAttack()
    {
        CurrentAnimationState = AnimationState.Idle;
    }
}
