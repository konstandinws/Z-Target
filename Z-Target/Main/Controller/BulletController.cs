using Raylib_cs;
using System.Numerics;
public class BulletController{

    public List<Bullet> BulletList = new List<Bullet>();
    private Player Player;

    public BulletController(Player player){
        this.Player = player;
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
        return Vector2.Distance(pos, Player.Position) > GameConfig.Bullet.MaxDistanceFromPlayer;
    }

    // direction doit être normalisée
    public void ShootBullet(Vector2 direction, float speed, int damage, int points)
    {
        BulletList.Add(new Bullet(Player.Position, direction, speed, damage, points));
    }
}
