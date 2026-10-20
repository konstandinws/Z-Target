// Crée une arme à partir de son nom (celui utilisé dans config.jsonc).
// Pour ajouter une nouvelle arme au jeu : créer sa classe dans Model/Weapons/ et l'ajouter ici.
public static class WeaponFactory
{
    private static readonly Dictionary<string, Func<Weapon>> Weapons = new Dictionary<string, Func<Weapon>>
    {
        ["AK47"] = () => new AK47(),
        ["AWP"] = () => new AWP(),
        ["FAMAS"] = () => new FAMAS(),
        ["GALIL"] = () => new GALIL(),
        ["GLOCK"] = () => new GLOCK(),
        ["M1911"] = () => new M1911(),
        ["M4"] = () => new M4(),
        ["MP40"] = () => new MP40(),
        ["MP5"] = () => new MP5(),
        ["OLYMPIA"] = () => new OLYMPIA(),
        ["PYTHON"] = () => new PYTHON(),
        ["RAYGUN"] = () => new RAYGUN(),
        ["RPK"] = () => new RPK(),
        ["SPAS12"] = () => new SPAS12(),
        ["UMP45"] = () => new UMP45(),
    };

    public static Weapon Create(string name)
    {
        if (!Weapons.TryGetValue(name, out Func<Weapon>? create))
            throw new ArgumentException($"Unknown weapon \"{name}\". Available: {string.Join(", ", Weapons.Keys)}");

        return create();
    }
}
