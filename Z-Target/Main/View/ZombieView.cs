using Raylib_cs;
using System.Numerics;

public class ZombieView : IView
{
  public void Draw(){
    List<Zombie> zombiesList = ZombieController.zombiesList;
    zombiesList.Sort((a, b) => a.Position.Y.CompareTo(b.Position.Y)); // Tri par position Y
    foreach(Zombie zombie in zombiesList){
        Raylib.DrawCircleV(zombie.Position,zombie.radius,Color.Green);
        DrawHealthBar(zombie);
    }
  }

  public void DrawHealthBar(Zombie zombie){
    float health = zombie.Health;
    float maxHealth =zombie.MaxHealth;
    float length = (health/maxHealth) * zombie.radius*2;
    float height = 5;
    Vector2 size = new Vector2(length,height);
    Vector2 healthBarPos = new Vector2(zombie.Position.X - zombie.radius, zombie.Position.Y - (float)(zombie.radius * 1.5));
    Raylib.DrawRectangleV(healthBarPos,size,Color.Red);
  }
}

