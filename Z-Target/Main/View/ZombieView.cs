using Raylib_cs;
using System.Numerics;

public class ZombieView
{
  public void Draw(){
    foreach(Zombie zombie in ZombieController.zombiesList){
        Raylib.DrawCircleV(zombie.Position,zombie.radius,Color.Green);
    }
  }
}

