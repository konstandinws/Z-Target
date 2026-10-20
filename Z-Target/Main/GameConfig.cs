using System.Numerics;

public static class GameConfig // classe static pour ne pas avoir d'instance de GameConfig et appeler les variables statiques directement
{
    public const string WindowTitle = "Z-Target";
    public const int ScreenWidth = 1200;
    public const int ScreenHeight = 800;
    public const int TargetFPS = 60;

    public const int MAP_SIZE = 3200;
    public const int PLAY_AREA_SIZE = 2000;
    // Marge entre le bord de la carte et la zone jouable (600 px de chaque côté)
    public const int PLAY_AREA_OFFSET = (MAP_SIZE - PLAY_AREA_SIZE) / 2;

    public static readonly Vector2 MapCenter = new Vector2(MAP_SIZE / 2f, MAP_SIZE / 2f);
}
