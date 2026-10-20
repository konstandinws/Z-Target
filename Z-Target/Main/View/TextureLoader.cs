using Raylib_cs;

// Charge chaque image une seule fois et la réutilise ensuite
// (évite de recharger toutes les textures à chaque nouvelle partie)
public class TextureLoader
{
    private readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

    public Texture2D Load(string path)
    {
        if (!textures.TryGetValue(path, out Texture2D texture))
        {
            texture = Raylib.LoadTexture(path);
            textures[path] = texture;
        }
        return texture;
    }

    // Charge toutes les images .png d'un dossier, triées par leur numéro (ex: walk_0, walk_1, ..., walk_19).
    // Le nombre d'images de l'animation est donc celui du dossier, sans valeur écrite en dur.
    public List<Texture2D> LoadAnimation(string folder)
    {
        List<string> files = Directory.GetFiles(folder, "*.png")
            .OrderBy(GetFrameNumber)
            .ToList();

        if (files.Count == 0)
            throw new FileNotFoundException($"No images found in folder {folder}");

        return files.Select(Load).ToList();
    }

    // "survivor-walk_12.png" -> 12
    private static int GetFrameNumber(string path)
    {
        string name = Path.GetFileNameWithoutExtension(path);
        string number = name.Substring(name.LastIndexOf('_') + 1);
        return int.TryParse(number, out int frame) ? frame : 0;
    }

    public void UnloadAll()
    {
        foreach (Texture2D texture in textures.Values)
        {
            Raylib.UnloadTexture(texture);
        }
        textures.Clear();
    }
}
