using Godot;
using System;

public partial class OreScript : Node3D, Mineable
{
	
	int ORE_HP = 3;

	public void Mine()
	{
		ORE_HP -= 1;
		GD.Print("ORE_HP -> " + ORE_HP);
		if (ORE_HP <= 0)
		{
			PlayerData.calcium_fuel += 100;
			GD.Print("Player now has " + PlayerData.calcium_fuel + " fuel" );
			QueueFree();
		}
		
	}

}
