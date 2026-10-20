// Structure du fichier config.jsonc.
// "required" : si une valeur manque dans le fichier, le jeu s'arrête au démarrage avec un message clair,
// au lieu de démarrer silencieusement avec une valeur à 0.

public class GameSettings
{
    public required WindowSettings Window { get; init; }
    public required MapSettings Map { get; init; }
    public required PlayerSettings Player { get; init; }
    public required KnifeSettings Knife { get; init; }
    public required ZombieSettings Zombie { get; init; }
    public required RoundSettings Rounds { get; init; }
    public required BonusSettings Bonus { get; init; }
    public required MysteryBoxSettings MysteryBox { get; init; }
    public required BulletSettings Bullet { get; init; }
    public required AssetSettings Assets { get; init; }
}

public class WindowSettings
{
    public required string Title { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required int TargetFPS { get; init; }
}

public class MapSettings
{
    public required int Size { get; init; }
    public required int PlayAreaSize { get; init; }

    // Marge entre le bord de la carte et la zone jouable
    public int PlayAreaOffset => (Size - PlayAreaSize) / 2;
    public float Center => Size / 2f;
}

public class PlayerSettings
{
    public required float Size { get; init; }
    public required int StartingScore { get; init; }
    public required string StartingWeapon { get; init; }
    public required int MaxHealth { get; init; }
    public required float Acceleration { get; init; }
    public required float MaxSpeed { get; init; }
    public required float SprintMultiplier { get; init; }
    public required float Friction { get; init; }
    public required float MaxSprintDuration { get; init; }
    public required float SprintDepletionRate { get; init; }
    public required float SprintRecoveryRate { get; init; }
    public required float InvincibilityTime { get; init; }
    public required float HealthRegenerationDelay { get; init; }
    public required float HealthRegenerationRate { get; init; }
}

public class KnifeSettings
{
    public required float Range { get; init; }
    public required int Damage { get; init; }
    public required float Cooldown { get; init; }
    public required float HalfAngle { get; init; }
    public required float AttackDuration { get; init; }
    public required int Points { get; init; }
}

public class ZombieSettings
{
    public required int Radius { get; init; }
    public required float SeparationRadius { get; init; }
    public required float SpawnDistance { get; init; }
    public required int ContactDamage { get; init; }
    public required float PlayerKnockback { get; init; }
}

public class RoundSettings
{
    public required int MaxZombiesOnScreen { get; init; }
    public required float SpawnCooldown { get; init; }
    public required float TransitionDuration { get; init; }
    public required int BaseZombieCount { get; init; }
    public required float ZombiesAddedPerRound { get; init; }
    public required float BaseZombieHealth { get; init; }
    public required float HealthAddedPerRound { get; init; }
    public required float BaseZombieSpeed { get; init; }
    public required float SpeedAddedPerRound { get; init; }
    public required float MaxZombieSpeed { get; init; }
}

public class BonusSettings
{
    public required float DropChance { get; init; }
    public required float Lifetime { get; init; }
    public required float EffectDuration { get; init; }
    public required float PickupRadius { get; init; }
}

public class MysteryBoxSettings
{
    public required float PositionX { get; init; }
    public required float PositionY { get; init; }
    public required int Price { get; init; }
    public required float WeaponDisplayDuration { get; init; }
    public required float InteractionRange { get; init; }
    public required float InteractionCooldown { get; init; }
    public required List<string> Weapons { get; init; }
}

public class BulletSettings
{
    public required float Radius { get; init; }
    public required float MaxDistanceFromPlayer { get; init; }
}

public class AssetSettings
{
    public required string Map { get; init; }
    public required string MysteryBox { get; init; }
    public required BonusAssets Bonus { get; init; }
    public required PlayerAssets Player { get; init; }
    public required ZombieAssets Zombie { get; init; }
}

public class BonusAssets
{
    public required string MaxAmmo { get; init; }
    public required string InstaKill { get; init; }
    public required string DoublePoints { get; init; }
    public required string Nuke { get; init; }
}

public class PlayerAssets
{
    public required string FeetIdle { get; init; }
    public required string FeetWalk { get; init; }
    public required string FeetRun { get; init; }
    public required string Knife { get; init; }
    public required WeaponAssets Rifle { get; init; }
    public required WeaponAssets Handgun { get; init; }
    public required WeaponAssets Shotgun { get; init; }
}

public class WeaponAssets
{
    public required string Idle { get; init; }
    public required string Shoot { get; init; }
    public required string Reload { get; init; }
}

public class ZombieAssets
{
    public required string Move { get; init; }
}
