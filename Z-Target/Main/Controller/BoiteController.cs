using System.Numerics;
using Raylib_cs;

public class BoiteController
{
    private Boite Box;
    private Player Player;
    private float InteractionRange = 40f;
    private float LastInteractionTime = 0f;
    private const float INTERACTION_COOLDOWN = 0.5f;

    public BoiteController(Boite box, Player player)
    {
        this.Box = box;
        this.Player = player;
    }

    public void Update(float dt)
    {
        float distance = Vector2.Distance(Player.Position, Box.Position);
        float currentTime = (float)Raylib.GetTime();

        if (distance < InteractionRange && Raylib.IsKeyPressed(KeyboardKey.F) && !Box.IsOpen && currentTime - LastInteractionTime >= INTERACTION_COOLDOWN)
        {
            Console.WriteLine("Open box");
            if (Box.CanOpen(Player))
            {
                Box.Open(Player);
                LastInteractionTime = currentTime;
            }
        }
        if(Box.IsOpen){
            Box.TimeToDisappear(dt);
            if(Raylib.IsKeyPressed(KeyboardKey.F) && currentTime - LastInteractionTime >= INTERACTION_COOLDOWN)
            {
                Console.WriteLine("Accept weapon");
                Box.AcceptWeapon(Player);
                Box.Close();
                LastInteractionTime = currentTime;
            }
            if (Box.WeaponTimeToDisappear <= 0)
            {
                Console.WriteLine("Close box");
                Box.Close();
            }
        }
    }
}