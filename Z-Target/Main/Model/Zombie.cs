using Raylib_cs;
using System.Numerics;
public class Zombie
{   
    public float Health;
    public Vector2 Position;
    public float Speed = 150;
    public bool IsAlive;
    public int radius = 27; 
    public float Damage = 35f;
    private Vector2 knockbackVelocity = Vector2.Zero;
    private const float KNOCKBACK_DECAY = 0.9f;

    public Zombie(){
        Health = 100;
        Position = new Vector2(Raylib.GetRandomValue(0, GameConfig.ScreenWidth), -200);
        IsAlive = true;
    }

    public void Move(float dt, Vector2 target)
    {
        // Appliquer le knockback
        Position += knockbackVelocity * dt;
        knockbackVelocity *= KNOCKBACK_DECAY;

        // Si le knockback est suffisamment faible, on reprend le mouvement normal
        if (knockbackVelocity.LengthSquared() < 1f)
        {
            Vector2 dir = Vector2.Normalize(target - Position);
            Position += dir * Speed * dt;
        }
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            IsAlive = false;
        }
    }

    public void KnockBack(Vector2 fromPosition, float force)
    {
        Vector2 direction = Vector2.Normalize(Position - fromPosition);
        knockbackVelocity = direction * force;
    }
}
