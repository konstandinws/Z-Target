using Raylib_cs;
using System.Numerics;

class BulletView : IView{
 public void Draw(){
    List<Bullet> bulletList = BulletController.Instance.BulletList;
    if (BulletController.Instance.BulletList.Count() > 0){
        foreach(Bullet bullet in bulletList){
            Raylib.DrawCircleV(bullet.Position, bullet.Radius, bullet.Color);
        }
    }
  }
}