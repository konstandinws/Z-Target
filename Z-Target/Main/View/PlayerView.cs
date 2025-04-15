using Raylib_cs;
using System.Numerics;

public class PlayerView : IView{

    private Player player;

    public PlayerView(Player player){
        this.player = player;
    }

    public void Draw(){

        Pivoter();
    }

    public void Pivoter(){
        
        Rectangle rect = new Rectangle(player.Position.X, player.Position.Y, player.Size.X, player.Size.Y);
        // origine du point de pivot au centre du rectangle 
        Vector2 origin = player.Size / 2f;

        Raylib.DrawRectanglePro(rect, origin, player.AngleToMouse, player.Color);
    }
}