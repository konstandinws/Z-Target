using Raylib_cs;
using System.Numerics;

class BulletView{

 public void Draw(List<Bullet> bulletList){
    if (bulletList.Count() > 0){
        foreach(Bullet bullet in bulletList){
            Raylib.DrawCircleV(bullet.Position,5,Color.Black);
        }
    }
  }
}