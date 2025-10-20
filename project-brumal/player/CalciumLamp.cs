using Godot;
using System;

public partial class CalciumLamp : Node3D
{

	bool isEquipped = false;

	SpotLight3D CLLight; // light reference
	[Export] public float FlickerIntensity = .3f;
	[Export] public double FlickerThreshHold = .6;
	bool light = false;

	// flashing variables
	bool flickerable = true;
	bool flashing = false;
	const float defaultSpotAngle = 52.72f;

	float lightFlicker = 0.0f;
	// initializing toggle boolean


	public override void _Ready()
	{
		CLLight = GetNode<SpotLight3D>("SpotLight3D");// initializing the light
	}



	// public async void flash(float intensity) {} 


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isEquipped)
		{
			if (flickerable)
			{

				// the random float is added to the base brightness of the light and the intensity of the flicker 
				// decided by the second value to the right of the randomFloat (the .2 in this case)
				lightFlicker = 0.0f;
				double flickerRan = GD.Randfn(0.0, 1.0);

				// this has to be here for the random number to update
				double flickerRan2 = GD.Randfn(0.0, 1.0);

				if (flickerRan > FlickerThreshHold)
				{
					lightFlicker = (float)(2 + (flickerRan2 * FlickerIntensity));
				}
				else
				{
					lightFlicker = 2;
				}

			}


			if (Input.IsActionJustPressed("leftclick"))
			{

				light = !light;

			}

			if (Input.IsActionJustPressed("special") && !flashing) // special use -> flash

			{

				flickerable = !flickerable;
				light = !light;

			}


			// if special is used. flickerable turns off. it stops flickering and initiates a flash+angle lerp
			if (!flickerable)
			{
				if (!flashing)
				{
					flashing = true;
					CLLight.LightEnergy = 1500f; // insane light intensity
					CLLight.SpotAngle = 179f; // flash all around the player
					CLLight.SpotRange = 60f;
				}

				if (CLLight.LightEnergy > 2f)
				{

					// since nothing else is touching these values it can be lerped with no issues
					CLLight.LightEnergy = Mathf.Lerp(CLLight.LightEnergy, 1f, 15f * (float)delta);
					CLLight.SpotAngle = Mathf.Lerp(CLLight.SpotAngle, defaultSpotAngle, 15f * (float)delta);
					CLLight.SpotRange = Mathf.Lerp(CLLight.SpotRange, 15, 15f * (float)delta);
				}
				else
				{

					// RULE. whenever lerp is called. 
					// you MUST set an exact value afterwards
					// if you do not reset the value...
					// float-point value drifting will occur (weird values)
					CLLight.SpotAngle = defaultSpotAngle;
					CLLight.LightEnergy = 0;
					CLLight.SpotRange = 15f;

					flickerable = !flickerable;
					light = !light;
					flashing = false;
				}

			}

			// if light AND not flashing, set lightflicker/off.
			if (light && !flashing)
			{

				CLLight.LightEnergy = lightFlicker;

			}
			else if (!light && !flashing)
			{

				CLLight.LightEnergy = 0;

			}

		}
	}
}
