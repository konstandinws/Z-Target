using System.Numerics;
using Raylib_cs;

public class MysteryBoxController
{
    private MysteryBox Box;
    private Player Player;
    private float LastInteractionTime = 0f;

    public MysteryBoxController(MysteryBox box, Player player)
    {
        this.Box = box;
        this.Player = player;
    }

    public void Update(float dt)
    {
        float distance = Vector2.Distance(Player.Position, Box.Position);
        float currentTime = (float)Raylib.GetTime();

        if (distance < GameConfig.MysteryBox.InteractionRange && Raylib.IsKeyPressed(KeyboardKey.F) && !Box.IsOpen && currentTime - LastInteractionTime >= GameConfig.MysteryBox.InteractionCooldown)
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
            if(Raylib.IsKeyPressed(KeyboardKey.F) && currentTime - LastInteractionTime >= GameConfig.MysteryBox.InteractionCooldown)
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