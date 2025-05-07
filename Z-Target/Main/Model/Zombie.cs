using Raylib_cs;
using System.Numerics;
using System;
public class Zombie
{   
    public float Health;
    public float MaxHealth;
    public Vector2 Position;
    public float Speed = 150;
    public bool IsAlive;
    public int radius = 27; 
    public float dropBonusChance = 0.05f;

    public Zombie(){
        MaxHealth = 150;
        Health = MaxHealth;
        Position = new Vector2(Raylib.GetRandomValue(0, GameConfig.ScreenWidth), -200);
        IsAlive = true;
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

    private BonusType? Die()
    {
        IsAlive = false;

        // Probabilité de drop d'un bonus (20 % par exemple)
       
        if (GetRandomFloat() < dropBonusChance)
        {
            // Random bonus (par exemple MaxAmmo ou InstaKill)
            BonusType bonusType = GetRandomBonusType();
            return bonusType;
        }

        return null;  // Pas de bonus
    }

    private float GetRandomFloat()
    {
        Random rnd = new Random();
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

        Random rnd = new Random();
        int index = rnd.Next(possibleBonuses.Length);
        return possibleBonuses[index];
    }
}
