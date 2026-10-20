using System.Numerics;

public enum BonusType
{
    MaxAmmo,
    InstaKill,
    DoublePoints,
    Nuke,
    None,
}

public class Bonus
{
    public const float LIFETIME = 10f;        // temps pendant lequel le bonus reste au sol
    public const float EFFECT_DURATION = 10f; // durée de l'effet (pour InstaKill / DoublePoints)
    public const float RADIUS = 20f;          // taille affichée
    public const float PICKUP_RADIUS = 25f;   // distance de ramassage (un peu plus large que l'affichage)

    public Vector2 Position ;
    public BonusType Type ;

    public float TimeRemaining ;    // Temps avant disparition au sol
    public float BonusDuration ;    // Temps restant de l'effet actif

    public bool IsCollected ;
    public bool IsTimedBonus ;      // Est-ce un bonus à effet temporaire ? (InstaKill / DoublePoints)
    public bool IsEffectActive ;    // Est-ce que l'effet est en cours ? (après ramassage)

    public Bonus(Vector2 position, BonusType type)
    {
        Position = position;
        Type = type;
        TimeRemaining = LIFETIME;
        IsCollected = false;
        IsEffectActive = false;

        IsTimedBonus = (Type == BonusType.InstaKill || Type == BonusType.DoublePoints);
        BonusDuration = 0f;  // Commence à 0, sera activé lors du ramassage
    }

    public void Update(float deltaTime)
    {
        if (!IsCollected)
        {
            // Bonus est au sol → timer au sol diminue
            TimeRemaining -= deltaTime;
        }
        else if (IsTimedBonus && IsEffectActive)
        {
            // Bonus est ramassé → timer d'effet diminue (si c'est InstaKill / DoublePoints)
            BonusDuration -= deltaTime;
        }
    }

    public void Collect()
    {
        IsCollected = true;

        if (IsTimedBonus)
        {
            BonusDuration = EFFECT_DURATION;
            IsEffectActive = true;
        }
        else
        {
            // MaxAmmo / Nuke pas de timer actif
            BonusDuration = 0f;
            IsEffectActive = false;
        }
    }

    public bool IsExpired()
    {
        // Bonus disparaît du sol s'il a expiré ou ramassé
        return TimeRemaining <= 0f || IsCollected;
    }

    public bool IsEffectOver()
    {
        // Pour vérifier si l'effet actif instaKill ou doublePoints est terminé
        return IsTimedBonus && IsEffectActive && BonusDuration <= 0f;
    }
}
