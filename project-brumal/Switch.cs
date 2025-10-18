using Godot;
using System;

public partial class Switch : Node3D, Interactable
{

	//[Export] 
	

	public bool SwitchToggle =	false; // sets this as default when game starts

	[Signal] public delegate void SwitchStatusEventHandler(bool Status);
	
	Node3D SwitchHandle; 
	MeshInstance3D BrassMesh;
	Vector3 Off = new Vector3(0f, 0f, 0f); // switch off rotation position
	Vector3 On = new Vector3(90f, 0f, 0f); // switch on rotation position
	Vector3 targetRotation = new Vector3(90f, 0f, 0f); // target for the lerp function to go towards
	public override void _Ready()
	{
		SwitchHandle = GetNode<Node3D>("SwitchHandle");// initializing the Handle Node
		BrassMesh = GetNode<MeshInstance3D>("SwitchHousingBrass/BrassHousingMesh");// initializing the Brass Housing Mesh Model
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	Vector3 currentRotation = SwitchHandle.Rotation;
		
		if(SwitchToggle){
			targetRotation = Off;
		}else{
			targetRotation = On;
		}
		if (currentRotation != targetRotation){

			SwitchHandle.RotationDegrees = SwitchHandle.RotationDegrees.Lerp(targetRotation, .2f);
		}
		

	}
	// lets be fr u know what this checks VV
	public void Touching(){
		//GD.Print("XRAY");	
		
	}
	// i deadass was staring at the interact code for like a hour drooling like a retard figuring out how tf u made that work, 
	// then when i realized i was like OOHHHH brah i tried finding the interact method thang for so long.
	public void Interact()
	{
	SwitchToggle = !SwitchToggle;

	EmitSignal(SignalName.SwitchStatus, SwitchToggle);
	//GD.Print(SwitchToggle);	
	}
}
