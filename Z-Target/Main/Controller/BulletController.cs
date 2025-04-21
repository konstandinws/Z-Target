using Raylib_cs;
using System.Numerics;
public class BulletController{

    public static BulletController Instance { get; private set; } // singleton
    public List<Bullet> BulletList = new List<Bullet>();
    Player player;

    public BulletController(Player player){
        this.player = player;
        if (Instance != null) // verification pour le singleton
            throw new Exception("Une seule instance de BulletController est autorisée.");
        Instance = this; // pour le singleton 
    }

    public void Update(float dt){
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

    public void ShootBullet(Vector2 targetPos, int damage = 50)
    {
        Vector2 direction = Vector2.Normalize(targetPos - player.Position);
        BulletList.Add(new Bullet(player.Position, direction, damage));
    }
}