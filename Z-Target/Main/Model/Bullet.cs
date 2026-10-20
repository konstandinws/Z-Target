using System.Numerics;
using Raylib_cs;

public class Bullet{

    public Vector2 Position;
    Vector2 Direction;
    float Speed;
    public int Damage;
    public int points;
    public float Radius = 5f;
    public Color Color = Color.Black;

    // direction doit être normalisée ; speed, damage et points viennent de l'arme
    public Bullet(Vector2 position, Vector2 direction, float speed, int damage, int points)
    {
        Position = position;
        Direction = direction;
        Speed = speed;
        Damage = damage;
        this.points = points;
    }

    public void Move(float dt){
        Position += Direction * Speed * dt;
    }
}
