using Godot;
using System;

public partial class WallLamp : Node3D
{
	[Export] private Switch _Switch;
	OmniLight3D OLight;
	
	public override void _Ready()
	{   
		// ok so im using signals and i dont really know how they work too well but basically i got it set up so
		// you can just go in the godot editor when in the actual level scene, if u click the light you can just select which
		// switch node u want it to respond to and it should worrk i think. (hopefully)

		_Switch.SwitchStatus += OnSwitchStatusReachedSignal;
		OLight = GetNode<OmniLight3D>("LightBulb/OmniLight3D");// initializing the light
		OLight.LightEnergy = 0.0f; // sets light as off as the switch is off by default aswell.
		
	}


	private void OnSwitchStatusReachedSignal(bool SwitchToggle)
	{

		if (SwitchToggle)
		{
			OLight.LightEnergy = 1.0f;
		}
		else
		{
			OLight.LightEnergy = 0f;
		}
	}
	
}
