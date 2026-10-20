using Raylib_cs;
using System.Numerics;
using System;
public class Zombie : IDamageable
{
    public float Health;
    public float MaxHealth;
    public Vector2 Position {get; set;}
    public float Speed;
    public bool IsAlive => Health > 0;
    public int Radius = GameConfig.Zombie.Radius;
    public float HitBoxRadius => Radius;

    public Zombie(Vector2 position, float health, float speed){
        MaxHealth = health;
        Health = health;
        Position = position;
        Speed = speed;
    }

    public void Move(float dt, Vector2 target) // je met target et pas joueur pour le Quick Revive je met target au loin en gros
    {
        Vector2 dir = Vector2.Normalize(target - Position);
        Position += dir * Speed * dt;
    }

    // Le zombie ne fait que perdre de la vie : ce qui se passe à sa mort
    // (points, bonus, manche) est géré via l'événement ZombieController.ZombieKilled
    public void TakeDamage(int damageAmount)
    {
        Health -= damageAmount;
    }
}
