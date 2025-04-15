using Raylib_cs;
using System.Numerics;
public class BulletController{

    public List<Bullet> BulletList;
    Player player;

    public BulletController(Player player){
        this.player = player;
        BulletList = new List<Bullet>();
    }

    public void Update(float dt){
        // if (BulletList.Count() > 0){
        //     foreach(Bullet bullet in BulletList){
        //         bullet.Move(dt);
        //         KillBullet(bullet);
        //         System.Console.WriteLine(bullet.Position);
        //     }
        // }
    for (int i = BulletList.Count - 1; i >= 0; i--)
    {
        Bullet bullet = BulletList[i];
        bullet.Move(dt);

        if (IsOutOfBounds(bullet.Position))
        {
            BulletList.RemoveAt(i);
        }
    }
    }

    private bool IsOutOfBounds(Vector2 pos)
    {
        return pos.X < 0 || pos.X > GameConfig.ScreenWidth || pos.Y < 0 || pos.Y > GameConfig.ScreenHeight;
    }

    // public void ShootBullet(Vector2 mouseWorldPos){
    //     Vector2 bulletDirection = Vector2.Normalize(mouseWorldPos - player.Position);
    //     BulletList.Add(new Bullet(player.Position, bulletDirection));
    // }
    public void ShootBullet(Vector2 targetPos, int damage = 10)
    {
        Vector2 direction = Vector2.Normalize(targetPos - player.Position);
        BulletList.Add(new Bullet(player.Position, direction, damage));
    }

    // void KillBullet(Bullet bullet){
    //     if(bullet.Position.X > GameConfig.ScreenWidth || bullet.Position.X < 0){
    //         BulletList.Remove(bullet);
    //     }
    //     if(bullet.Position.Y > GameConfig.ScreenHeight || bullet.Position.Y < 0){
    //         BulletList.Remove(bullet);
    //     }
    // }
}