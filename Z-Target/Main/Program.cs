// using System.Security.Cryptography.X509Certificates;
// using Raylib_cs;

// class Program
// {
//     static void Main(string[] args)
//     {

//         Raylib.InitWindow(GameConfig.ScreenWidth, GameConfig.ScreenHeight, "test");
//         Raylib.SetTargetFPS(GameConfig.TargetFPS);

//         GameController gameController = new GameController();

//         while (!Raylib.WindowShouldClose())
//         {
//             float deltaTime = Raylib.GetFrameTime();
            
            
//             gameController.update(deltaTime);
//             gameController.Draw();

//         }

//         Raylib.CloseWindow();
//     }
// }
using Raylib_cs;
using System;
using System.Numerics;

class Program
{
    static void Main()
    {
        const int screenWidth = 800;
        const int screenHeight = 600;

        Raylib.InitWindow(screenWidth, screenHeight, "Max Ammo - Sprite Stack Rotating");
        Raylib.SetTargetFPS(60);

        const int sliceCount = 65;
        int sliceOffset = 8; // Plus grand pour mieux voir les tranches
        const float scale = 10.0f;  // Facteur d’agrandissement
        const float rotationSpeed = 1f;
        Vector2 position = new Vector2(screenWidth / 2, screenHeight / 2);
        int hauteur = 100;

        Texture2D[] slices = new Texture2D[sliceCount];
        for (int i = 1; i < sliceCount; i++)
        {
            slices[i] = Raylib.LoadTexture($"zombie/Zombie-{i}.png");
        }

        float angle = 0f;

        while (!Raylib.WindowShouldClose())
        {
            angle += rotationSpeed;

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DarkGray);

            int centerX = screenWidth / 2;
            int centerY = screenHeight / 2 + 250 +hauteur; // Déplacé 100 pixels plus bas

            if (Raylib.IsKeyDown(KeyboardKey.W))
            {
                sliceOffset -= 1 ; // Déplacement vers le haut
            }
            if (Raylib.IsKeyDown(KeyboardKey.S))
            {
                sliceOffset += 1; // Déplacement vers le bas
            }
            if (Raylib.IsKeyDown(KeyboardKey.Space))
            {
                sliceOffset = 8; // Réinitialisation de l'offset
            }
            if (Raylib.IsKeyDown(KeyboardKey.Up))
            {
                hauteur -= 1;
            }
            if (Raylib.IsKeyDown(KeyboardKey.Down))
            {
                hauteur += 1;
            }

            for (int i = 0; i < sliceCount; i++)
            {
                int offsetY = i * - sliceOffset;

                Texture2D slice = slices[i];

                // Calcul du centre de la tranche pour la rotation
                int sliceCenterX = centerX;
                int sliceCenterY = centerY + offsetY;

                // Calcul du rectangle de destination pour DrawTextureEx
                Rectangle sourceRec = new Rectangle(0, 0,slice.Width, slice.Height); // Source rectangle pour la texture
                Rectangle destRec = new Rectangle(sliceCenterX, sliceCenterY, slice.Width * scale, slice.Height * scale);
                Vector2 origin = new Vector2(slice.Width * scale / 2, slice.Height * scale / 2); // Origine au centre de la tranche

                Raylib.DrawTexturePro(slice, sourceRec, destRec, origin, angle, Color.White);
            }

            Raylib.DrawText("Sprite Stack - Rotation + Scale", 20, 20, 20, Color.LightGray);
            Raylib.DrawText($"Slice Offset: {sliceOffset}", 20, 40, 20, Color.LightGray);
            Raylib.EndDrawing();
        }

        foreach (Texture2D slice in slices)
            Raylib.UnloadTexture(slice);

        Raylib.CloseWindow();
    }
}