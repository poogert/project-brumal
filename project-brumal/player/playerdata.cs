using Godot;
public partial class PlayerData : Node
{
    // player info
    public static Vector3 player_position;
    public static Vector3 look_direction;
    public static float rot_horizontal;
    public static float rot_vertical; 

	// player resources
    public static int flare_count = 5;
    public static float calcium_fuel = 100;
    public static int sprint = 100;
    
	// states
	public static string currentState = "";
    public static string currentItem = "";


    // raycast
    public static (bool colliding, Vector3? point) ray;
    public static Node collider;

    
}
