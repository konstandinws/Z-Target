using System.Text.Json;

// Point d'accès unique à la configuration du jeu, chargée depuis config.jsonc au démarrage.
// En lecture seule : rien ne la modifie pendant la partie.
public static class GameConfig
{
    public const string FILE_PATH = "config.jsonc";

    private static GameSettings? settings;

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        ReadCommentHandling = JsonCommentHandling.Skip, // autorise les // commentaires dans le fichier
        AllowTrailingCommas = true,
    };

    public static GameSettings Settings =>
        settings ?? throw new InvalidOperationException("GameConfig.Load() must be called before using the config.");

    // Raccourcis vers chaque section (ex: GameConfig.Player.MaxSpeed)
    public static WindowSettings Window => Settings.Window;
    public static MapSettings Map => Settings.Map;
    public static PlayerSettings Player => Settings.Player;
    public static KnifeSettings Knife => Settings.Knife;
    public static ZombieSettings Zombie => Settings.Zombie;
    public static RoundSettings Rounds => Settings.Rounds;
    public static BonusSettings Bonus => Settings.Bonus;
    public static MysteryBoxSettings MysteryBox => Settings.MysteryBox;
    public static BulletSettings Bullet => Settings.Bullet;
    public static AssetSettings Assets => Settings.Assets;

    public static void Load(string path = FILE_PATH)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Config file not found: {Path.GetFullPath(path)}");

        try
        {
            settings = JsonSerializer.Deserialize<GameSettings>(File.ReadAllText(path), JsonOptions)
                ?? throw new InvalidDataException($"Config file is empty: {path}");
        }
        catch (JsonException e)
        {
            throw new InvalidDataException($"Invalid config file {path}: {e.Message}", e);
        }

        // Vérifie tout de suite que les armes citées existent (sinon l'erreur arriverait en pleine partie)
        WeaponFactory.Create(Player.StartingWeapon);
        foreach (string weapon in MysteryBox.Weapons)
            WeaponFactory.Create(weapon);
    }
}
