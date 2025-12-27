using Godot;
public partial class PlayerData : Node
{

    public const int MAX_STAMINA = 200;
    public const int INITIAL_FLARE_COUNT = 5;
    public const int INITIAL_CALCIUM_COUNT = 100;

    // player info
    public static Vector3 player_position;
    public static Vector3 camera_position;
    public static Vector3 look_direction;
    public static float rot_horizontal;
    public static float rot_vertical; 


	// player resources
    public static int flare_count = 5;
    public static float calcium_fuel = 100;
    public static int stamina = MAX_STAMINA;
    

	// states
	public static string CurrentState = "";
    public static string CurrentItem = "";
    public static bool IsInAir = false;
    public static bool IsBlocking = false;
    public static bool[] hotbarinventory = {false, false, false, false, false};
    

    // raycast
    public static (bool colliding, Vector3? point) ray;
    public static Node collider;
}
