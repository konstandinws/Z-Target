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
        Running
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
    private Vector2 velocity = Vector2.Zero;
    private Vector2 acceleration = Vector2.Zero;

    private float accelerationSpeed = 10000f;
    private float maxSpeed = 300f;
    private float sprintMultiplier = 1.6f;  // Multiplicateur de vitesse en sprint
    private bool isSprinting = false;
    private float friction = 0.85f; // augmenter fait effet patinoire
    private float minVelocity = 0.01f; // vitesse minimale avant de s'arreter
    
    // Variables pour la gestion du sprint
    private float maxSprintDuration = 3.0f;    // Durée maximale du sprint en secondes
    private float currentSprintDuration = 3.0f; // Durée actuelle de sprint restante
    private float sprintRecoveryRate = 0.5f;    // Vitesse de récupération du sprint (par seconde)
    private float sprintDepletionRate = 1.0f;   // Vitesse d'épuisement du sprint (par seconde)
    private bool canSprint = true;              // Indique si le joueur peut sprinter

    public Weapon mainWeapon;
    public Weapon secondaryWeapon;

    public bool isReloading = false;

    public int Score = 0;

    // Système de santé
    private int maxHealth = 3;
    private int currentHealth;
    private float invincibilityTime = 1.0f; // Temps d'invincibilité après être touché (en secondes)
    private float currentInvincibilityTime = 0f;
    private bool isInvincible = false;
    
    // Régénération de santé
    private float healthRegenerationDelay = 2.0f; // Délai avant de commencer à régénérer (en secondes)
    private float currentRegenerationDelay = 0f;  // Compteur actuel pour le délai de régénération
    private bool isRegenerating = false;         // Indique si le joueur est en train de régénérer sa santé
    private float regenerationRate = 1.0f;       // Vitesse de régénération en points de vie par seconde
    private float regenerationAccumulator = 0f;  // Accumulateur pour la régénération (pour les points de vie fractionnés)
    
    // Remplacer le bonus unique par un Dictionary pour suivre plusieurs bonus actifs
    private Dictionary<BonusType, float> activeBonuses = new Dictionary<BonusType, float>();
    
    public Player()
    {
        currentHealth = maxHealth; // Initialisation de la santé à sa valeur maximale
    }
    
    public void Move(float deltaTime)
    {
        acceleration = Vector2.Zero;
        acceleration = Direction * accelerationSpeed;
        
        velocity += acceleration * deltaTime;

        // Mise à jour de l'état d'animation
        UpdateAnimationState();

        // Gestion du sprint
        UpdateSprintState(deltaTime);
        
        // Appliquer le multiplicateur de sprint à la vitesse max si nécessaire
        float currentMaxSpeed = isSprinting ? maxSpeed * sprintMultiplier : maxSpeed;

        if (velocity.LengthSquared() > currentMaxSpeed * currentMaxSpeed)
            velocity = Vector2.Normalize(velocity) * currentMaxSpeed;

        velocity *= friction;

        if (velocity.LengthSquared() < minVelocity * minVelocity)
            velocity = Vector2.Zero;

        Position += velocity * deltaTime;
        
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
        if (isSprinting && canSprint)
        {
            // Réduire l'endurance de sprint
            currentSprintDuration -= sprintDepletionRate * deltaTime;
            
            // Vérifier si l'endurance est épuisée
            if (currentSprintDuration <= 0)
            {
                currentSprintDuration = 0;
                canSprint = false;
                isSprinting = false;
            }
        }
        // Si le joueur ne sprinte pas, récupérer de l'endurance
        else if (!isSprinting)
        {
            // Augmenter progressivement l'endurance de sprint
            currentSprintDuration += sprintRecoveryRate * deltaTime;
            
            // Limiter à la durée maximale
            if (currentSprintDuration >= maxSprintDuration)
            {
                currentSprintDuration = maxSprintDuration;
                canSprint = true;
            }
        }
    }
    
    // Méthode pour mettre à jour les timers de bonus
    private void UpdateBonusTimers(float deltaTime)
    {
        List<BonusType> expiredBonuses = new List<BonusType>();
        
        foreach (var bonus in activeBonuses)
        {
            BonusType bonusType = bonus.Key;
            float remaining = bonus.Value - deltaTime;
            
            if (remaining <= 0)
            {
                expiredBonuses.Add(bonusType);
            }
            else
            {
                activeBonuses[bonusType] = remaining;
            }
        }
        
        // Supprimer les bonus expirés
        foreach (var expired in expiredBonuses)
        {
            activeBonuses.Remove(expired);
        }
    }

    public void UpdateAngle(Vector2 mousePosition)
    {
        AngleToMouse = MathF.Atan2(mousePosition.Y - Position.Y, mousePosition.X - Position.X) * (180f / MathF.PI);
    }

    public bool HasActiveBonus(BonusType type)
    {
        return activeBonuses.ContainsKey(type);
    }
    
    public List<BonusType> GetActiveBonuses()
    {
        return new List<BonusType>(activeBonuses.Keys);
    }
    
    public float GetBonusRemainingTime(BonusType type)
    {
        if (activeBonuses.ContainsKey(type))
        {
            return activeBonuses[type];
        }
        return 0f;
    }

    public void EnableInstaKill(float duration)
    {
        activeBonuses[BonusType.InstaKill] = duration;
    }

    public void EnableDoublePoints(float duration)
    {
        activeBonuses[BonusType.DoublePoints] = duration;
    }

    public void DisableBonus(BonusType type)
    {
        if (activeBonuses.ContainsKey(type))
        {
            activeBonuses.Remove(type);
        }
    }
    
    public void DisableAllBonuses()
    {
        activeBonuses.Clear();
    }

    public void SetSprinting(bool sprinting)
    {
        // Ne permettre le sprint que s'il reste de l'endurance et qu'il est autorisé
        if (sprinting && currentSprintDuration > 0 && canSprint)
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
        return currentSprintDuration / maxSprintDuration;
    }

    private void UpdateInvincibility(float deltaTime)
    {
        if (isInvincible)
        {
            currentInvincibilityTime -= deltaTime;
            if (currentInvincibilityTime <= 0)
            {
                isInvincible = false;
            }
        }
    }
    
    private void UpdateHealthRegeneration(float deltaTime)
    {
        // Si la santé est déjà au maximum, pas besoin de régénérer
        if (currentHealth >= maxHealth)
        {
            isRegenerating = false;
            currentRegenerationDelay = 0f;
            return;
        }
        
        // Si le joueur vient d'être touché (est invincible), réinitialiser le délai de régénération
        if (isInvincible)
        {
            currentRegenerationDelay = 0f;
            isRegenerating = false;
            return;
        }
        
        // Si le joueur n'est pas en train de régénérer, attendre le délai avant de commencer
        if (!isRegenerating)
        {
            currentRegenerationDelay += deltaTime;
            
            // Commencer la régénération si le délai est écoulé
            if (currentRegenerationDelay >= healthRegenerationDelay)
            {
                isRegenerating = true;
                regenerationAccumulator = 0f;
            }
        }
        // Si le joueur est en train de régénérer, augmenter sa santé progressivement
        else
        {
            // Accumuler la régénération en fonction du temps écoulé
            regenerationAccumulator += regenerationRate * deltaTime;
            
            // Si l'accumulateur dépasse 1, ajouter un point de vie
            if (regenerationAccumulator >= 1.0f)
            {
                int pointsToAdd = (int)regenerationAccumulator;
                regenerationAccumulator -= pointsToAdd;
                
                currentHealth += pointsToAdd;
                
                // Limiter la santé au maximum
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                    isRegenerating = false; // Arrêter la régénération quand la santé est pleine
                }
            }
        }
    }
    
    public void TakeDamage(int damage)
    {
        // Si le joueur est invincible, ne pas prendre de dégâts
        if (isInvincible)
            return;
            
        currentHealth -= damage;
        
        // Activer l'invincibilité temporaire
        isInvincible = true;
        currentInvincibilityTime = invincibilityTime;
        
        // Réinitialiser le délai de régénération
        currentRegenerationDelay = 0f;
        isRegenerating = false;
        
        if (currentHealth <= 0)
        {
            // Le joueur est mort
            currentHealth = 0;
            // Logique de mort du joueur à implémenter
        }
    }
    
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    public float GetHealthRatio()
    {
        return (float)currentHealth / maxHealth;
    }
    
    public bool IsInvincible()
    {
        return isInvincible;
    }

    private void UpdateAnimationState()
    {
        if (velocity.LengthSquared() < minVelocity * minVelocity + 800.0f)
        {
            CurrentAnimationState = AnimationState.Idle;
        }
        else if (isSprinting)
        {
            CurrentAnimationState = AnimationState.Running;
        }
        else
        {
            CurrentAnimationState = AnimationState.Walking;
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
}
