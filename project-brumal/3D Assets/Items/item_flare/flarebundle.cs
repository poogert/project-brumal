using Godot;
using System;

public partial class flarebundle : Node3D, Interactable
{
	public void Interact()
	{
		PlayerData.flare_count += 10;
		GD.Print("player now has " + PlayerData.flare_count + " flares");
		this.QueueFree();
	}
}
