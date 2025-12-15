using Godot;
public partial class PlayerData : Node
{
    // player info
    public static Vector3 player_position;
    public static Vector3 look_direction;
    public static float rot_horizontal;
    public static float rot_vertical; 
    public static int flare_count = 5;
    public static float calcium_fuel = 100;

    // raycast
    public static (bool colliding, Vector3? point) ray;
    public static Node collider;
}
