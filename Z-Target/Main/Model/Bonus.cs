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
    public Vector2 Position { get; private set; }
    public BonusType Type { get; private set; }

    private const float Lifetime = 10f;       // Bonus reste au sol 10 secondes
    private const float EffectDuration = 10f; // Durée de l'effet (pour InstaKill / DoublePoints)

    public float TimeRemaining { get; private set; }    // Temps avant disparition au sol
    public float BonusDuration { get; private set; }    // Temps restant de l'effet actif

    public bool IsCollected { get; private set; }
    public bool IsTimedBonus { get; private set; }      // Est-ce un bonus à effet temporaire ? (InstaKill / DoublePoints)
    public bool IsEffectActive { get; private set; }    // Est-ce que l'effet est en cours ? (après ramassage)

    public Bonus(Vector2 position, BonusType type)
    {
        Position = position;
        Type = type;
        TimeRemaining = Lifetime;   // Tous les bonus restent au sol 10 sec
        IsCollected = false;
        IsEffectActive = false;

        IsTimedBonus = (type == BonusType.InstaKill || type == BonusType.DoublePoints);
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
            // Démarre l'effet de 10s après ramassage
            BonusDuration = EffectDuration;
            IsEffectActive = true;
        }
        else
        {
            // MaxAmmo / Nuke → effet instantané (pas de timer actif)
            BonusDuration = 0f;
            IsEffectActive = false;
        }
    }

    public bool IsExpired()
    {
        // Bonus disparaît du sol s’il a expiré OU a été ramassé
        return TimeRemaining <= 0f || IsCollected;
    }

    public bool IsEffectOver()
    {
        // Pour vérifier si l'effet actif (InstaKill / DoublePoints) est terminé
        return IsTimedBonus && IsEffectActive && BonusDuration <= 0f;
    }
}
