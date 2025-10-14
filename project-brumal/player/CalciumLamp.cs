using Godot;
using System;

public partial class CalciumLamp : Node3D
{	
	
	SpotLight3D CLLight; // light reference
	[Export] public float FlickerIntensity = .3f;
	[Export] public double FlickerThreshHold = .6;
	new bool light = false;
	// initializing toggle boolean


	public override void _Ready()
	{
		CLLight = GetNode<SpotLight3D>("SpotLight3D");// initializing the light
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)

	{	
		
		float lightFlicker = 0.0f;
		double flickerRan = GD.Randfn(0.0, 1.0);
		double flickerRan2 = GD.Randfn(0.0, 1.0);
		// this has to be here for the random number to update, which helps make the flicker effect
		if (flickerRan > FlickerThreshHold)
		{
			lightFlicker = (float)(2 + (flickerRan2 * FlickerIntensity));
		}
		else
		{
			lightFlicker = 2;
		}
		

		// ^^ the random float is added to the base brightness of the light and the intensity of the flicker 
		// decided by the second value to the right of the randomFloat (the .2 in this case)
		//  I loveee furriesss - Edgaaaa 
		if( Input.IsActionJustPressed("FlashLight") ) 
		{
			light = !light;
			// simple toggle
		}
		if (light)
		{
			
			CLLight.LightEnergy = lightFlicker;
		
		} else
		{
			// turn off light 
			CLLight.LightEnergy = 0;
			
		}
	}
	
}
