using Raylib_cs;
using System.Numerics;
public class Zombie
{   
    public float Health;
    public Vector2 Position;
    public float Speed = 150;
    public bool IsAlive;
    public int radius = 27; 

    public Zombie(){
        Health = 100;
        Position = new Vector2(Raylib.GetRandomValue(0, GameConfig.ScreenWidth), -200);
        IsAlive = true;
    }

    public void Move(float dt, Vector2 target) // je met target et pas joueur pour le Quick Revive je met target au loin en gros
    {
        Vector2 dir = Vector2.Normalize(target - Position);
        Position += dir * Speed * dt;
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            IsAlive = false;
        }
    }
}
