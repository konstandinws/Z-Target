using Raylib_cs;
using System.Numerics;
public class BulletController{

    public static BulletController Instance { get; private set; } // singleton
    public List<Bullet> BulletList = new List<Bullet>();
    Player Player;

    public BulletController(Player player){
        this.Player = player;
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
        if (Vector2.Distance(pos, Player.Position) > 1500)
        {
            return true;
        }
        return false;
    }

    public void ShootBullet(Vector2 targetPos, int damage, int points)
    {
        Vector2 direction = Vector2.Normalize(targetPos - Player.Position);
        BulletList.Add(new Bullet(Player.Position, direction, damage, points));
        
    }
}