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
    public static int stamina = 100;
    
	// states
	public static string CurrentState = "";
    public static string CurrentItem = "";
    public static bool IsInAir = false;
    public static bool[] hotbarinventory = {false, false, false, false, false};


    // raycast
    public static (bool colliding, Vector3? point) ray;
    public static Node collider;

    
}
