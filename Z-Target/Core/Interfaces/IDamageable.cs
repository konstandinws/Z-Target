using Raylib_cs;
/*
on la rend touchable et blessable pour adapter le bullet controller qui a besoin de la position
c'est pour qu'on a la position et la gestion de la santé
*/
using System.Numerics;

interface IDamageable 
{
    bool IsAlive {get; set;}
    Vector2 Position {get; set;}
    int HitBoxRadius {get; set;}
    void TakeDamage();
}