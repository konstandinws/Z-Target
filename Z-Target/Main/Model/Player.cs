using Raylib_cs;
using System.Numerics;

public class Player
{
    public Vector2 Position { get; set; } = new Vector2(GameConfig.ScreenWidth / 2, GameConfig.ScreenHeight / 2);
    public Vector2 Size { get; set; } = new Vector2(50, 50);
    public Color Color { get; set; } = Color.Blue;
    public Vector2 Direction = Vector2.Zero;
    public float AngleToMouse;

    private Vector2 velocity = Vector2.Zero;
    private Vector2 acceleration = Vector2.Zero;

    private float accelerationSpeed = 10000f;
    private float maxSpeed = 300f;
    private float friction = 0.85f; // augmenter fait effet patinoire

    private float minVelocity = 0.01f; // vitesse minimale avant de s'arreter
    
    public void Move(float deltaTime)
    {
        acceleration = Vector2.Zero;
        acceleration = Direction * accelerationSpeed;
        
        velocity += acceleration * deltaTime;

        if (velocity.LengthSquared() > maxSpeed * maxSpeed) // velocity vecteur et maxSpeed scalaire et on regarde le carre pour eviter la racine carre couteuse en perfomances
            velocity = Vector2.Normalize(velocity) * maxSpeed; // si le vecteur depasse on le normalise 

        velocity *= friction;

        if (velocity.LengthSquared() < minVelocity * minVelocity) // si la vitesse est inferieure a la vitesse minimale on l'arrete
            velocity = Vector2.Zero;

        Position += velocity * deltaTime;
    }

    public void UpdateAngle(Vector2 mousePosition)
    {
        AngleToMouse = MathF.Atan2(mousePosition.Y - Position.Y, mousePosition.X - Position.X) * (180f / MathF.PI);
    }
}
