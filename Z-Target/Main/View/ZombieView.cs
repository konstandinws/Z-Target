using Raylib_cs;
using System.Numerics;

public class ZombieView : IView
{

  public void Draw(){
  foreach(Zombie zombie in ZombieController.zombiesList){
      Raylib.DrawCircleV(zombie.Position, zombie.radius, Color.Green);
  }
  foreach(Zombie zombie in ZombieController.zombiesList){ // je fais une autre boucle pour placer la barre de vie
    DrawHealthBar(zombie);                                // pour que les zombies soient dessine avant les barres de vie
  }
  }
  private void DrawHealthBar(Zombie zombie){ // fonction degeulasse pour placer la barre de vie
    Raylib.DrawRectangleV(new Vector2(zombie.Position.X - (zombie.radius*1.5f)/2,zombie.Position.Y - zombie.radius * 1.2f), new Vector2((zombie.Health/100) * (zombie.radius *1.5f), 5), Color.Red);
  }
}

