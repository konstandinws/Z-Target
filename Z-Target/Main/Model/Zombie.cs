using Raylib_cs;
using System.Numerics;
using System;
public class Zombie : IDamageable
{   
    public float Health;
    public float MaxHealth;
    public Vector2 Position {get; set;}
    public float Speed = 150;
    public bool IsAlive => Health > 0;
    public int Radius = 27; 
    public float HitBoxRadius => Radius;
    public float DropBonusChance = 0.60f;
    static Random rnd = new Random();

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

    public BonusType? HandleDamage(int damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            return Die();
        }
        return null;
    }

    public void TakeDamage(int damageAmount)
    {
        BonusType? bonus = HandleDamage(damageAmount);
        if (bonus.HasValue) 
        {
            BonusController.Instance.CreateBonus(this.Position, bonus.Value); // couplage degeulasse utiliser des Evenements pour la creation de bonus
        }
    }

    private BonusType? Die()
    {
        // Probabilité de drop d'un bonus (20 % par exemple)
        if (GetRandomFloat() < DropBonusChance)
        {
            // Random bonus (par exemple MaxAmmo ou InstaKill)
            BonusType bonusType = GetRandomBonusType();
            return bonusType;
        }

        return null;  // Pas de bonus
    }

    private float GetRandomFloat()
    {
        return (float)rnd.NextDouble();  // Entre 0.0 et 1.0
    }
    private BonusType GetRandomBonusType()
    {
        BonusType[] possibleBonuses = new BonusType[]
        {
            BonusType.MaxAmmo,
            BonusType.InstaKill,
            BonusType.DoublePoints,
            BonusType.Nuke,
        };

        int index = rnd.Next(possibleBonuses.Length);
        return possibleBonuses[index];
    }
}
