using System.Numerics;
using Raylib_cs;

public class BoiteController
{
    private Boite box;
    private Player player;
    private float interactionRange = 40f;
    private float lastInteractionTime = 0f;
    private const float INTERACTION_COOLDOWN = 0.5f;

    public BoiteController(Boite box, Player player)
    {
        this.box = box;
        this.player = player;
    }

    public void Update(float dt)
    {
        float distance = Vector2.Distance(player.Position, box.Position);
        float currentTime = (float)Raylib.GetTime();

        if (distance < interactionRange && Raylib.IsKeyPressed(KeyboardKey.F) && !box.IsOpen && currentTime - lastInteractionTime >= INTERACTION_COOLDOWN)
        {
            Console.WriteLine("Open box");
            if (box.CanOpen(player))
            {
                box.Open(player);
                lastInteractionTime = currentTime;
            }
        }
        if(box.IsOpen){
            box.TimeToDisappear(dt);
            if(Raylib.IsKeyPressed(KeyboardKey.F) && currentTime - lastInteractionTime >= INTERACTION_COOLDOWN)
            {
                Console.WriteLine("Accept weapon");
                box.AcceptWeapon(player);
                box.Close();
                lastInteractionTime = currentTime;
            }
            if (box.WeaponTimeToDisappear <= 0)
            {
                Console.WriteLine("Close box");
                box.Close();
            }
        }
    }
}