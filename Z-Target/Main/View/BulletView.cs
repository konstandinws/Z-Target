using Raylib_cs;
using System.Numerics;

class BulletView : IView{
    private BulletController BulletController;

    public BulletView(BulletController bulletController){
        this.BulletController = bulletController;
    }

    public void Draw(){
        foreach(Bullet bullet in BulletController.BulletList){
            Raylib.DrawCircleV(bullet.Position, bullet.Radius, bullet.Color);
        }
    }
}