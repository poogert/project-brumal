using Godot;
using System;

public partial class PickaxeScript : Node3D
{
	

	public override void _Process(double delta)
	{
		if ( Input.IsActionJustPressed("leftclick") )
		{
			GD.Print("Holding");
		}

	}

}
