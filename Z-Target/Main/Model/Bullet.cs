// using System.Numerics;
// using Raylib_cs;

// public class Bullet
// {
//     public Vector2 Position;
//     public Vector2 Velocity;
//     public float Speed = 600f;
//     public float Radius = 5f;
//     public bool IsDead = false;

//     public Bullet(Vector2 startPos, Vector2 target)
//     {
//         Position = startPos;
//         Vector2 dir = Vector2.Normalize(target - startPos);
//         Velocity = dir * Speed;
//     }

//     public void Update(float dt)
//     {
//         Position += Velocity * dt;

//         // Supprimer si hors écran
//         if (Position.X < 0 || Position.X > 800 || Position.Y < 0 || Position.Y > 600)
//             IsDead = true;
//     }

//     public void Draw()
//     {
//         Raylib.DrawCircleV(Position, Radius, Color.Black);
//     }

//     public bool CollidesWith(Zombie zombie)
//     {
//         return Vector2.Distance(Position, zombie.Position) < Radius + zombie.Radius;
//     }
// }

using System.Numerics;
using Raylib_cs;

public class Bullet{

    public Vector2 Position;
    Vector2 Direction;
    public int Damage = 50;
    float bulletSpeed = 1600.0f;
    public float Radius = 5f;
    public Color Color = Color.Black; 
    public int points = 10;

   public Bullet(Vector2 position, Vector2 direction, int damage = 50, int points = 10)
    {
        Position = position;
        Direction = direction;
        Damage = damage;
        this.points = points;
    }

    public void Move(float dt){
        Position += Direction * bulletSpeed * dt;
    }
}
