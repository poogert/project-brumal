using Godot;
using System;

public partial class CalciumLamp : Node3D
{
	// ==========================
	// References
	// ==========================
	private SpotLight3D _light;

	[Export] private Timer FuelTimer;

	// ==========================
	// Configuration
	// ==========================
	[Export] public float FlickerIntensity = 0.3f;
	[Export] public double FlickerThreshold = 0.6;

	private const float DefaultSpotAngle = 52.72f;
	private const float DefaultRange = 301f;

	// ==========================
	// State
	// ==========================
	private bool isOn = false;
	private bool flickerEnabled = true;
	private bool isFlashing = false;

	// ==========================
	// Lifecycle
	// ==========================
	public override void _Ready()
	{
		_light = GetNode<SpotLight3D>("light");
		FuelTimer.Timeout += OnFuelTimerTimeout;

		TurnLightOff();
	}

	public override void _Process(double delta)
	{
		HandleInput();
		UpdateFlash((float)delta);
		UpdateLightVisuals();
	}

	// ==========================
	// Input
	// ==========================
	private void HandleInput()
	{
		if (Input.IsActionJustPressed("leftclick"))
		{
			if (isOn) TurnLightOff();
			else TurnLightOn();
		}

		if (Input.IsActionJustPressed("special"))
		{
			StartFlash();
		}
	}

	// ==========================
	// Light State
	// ==========================
	private void TurnLightOn()
	{
		if (isOn || PlayerData.calcium_fuel <= 0) return;

		isOn = true;
		FuelTimer.Start();
	}

	private void TurnLightOff()
	{
		if (!isOn) return;

		isOn = false;
		FuelTimer.Stop();
		_light.LightEnergy = 0;
	}

	// ==========================
	// Flicker
	// ==========================
	private float CalculateFlicker()
	{
		double r1 = GD.Randfn(0.0, 1.0);
		double r2 = GD.Randfn(0.0, 1.0);

		return (r1 > FlickerThreshold) ? (float)(2 + r2 * FlickerIntensity) : 2f;
	}

	// ==========================
	// Flash Ability
	// ==========================
	private void StartFlash()
	{
		if (isFlashing || PlayerData.calcium_fuel < 20) return;

		isFlashing = true;
		flickerEnabled = false;

		TurnLightOn();
		PlayerData.calcium_fuel -= 20f;

		_light.LightEnergy = 3000f;
		_light.SpotAngle = 179f;
		_light.SpotRange = 100f;
	}

	private void UpdateFlash(float delta)
	{
		if (!isFlashing) return;

		float time = 6f;

		_light.LightEnergy = Mathf.Lerp(_light.LightEnergy, 1f, time * delta);
		_light.SpotAngle = Mathf.Lerp(_light.SpotAngle, DefaultSpotAngle, time * delta);
		_light.SpotRange = Mathf.Lerp(_light.SpotRange, DefaultRange, time * delta);

		if (_light.LightEnergy <= 2f) EndFlash();
		
	}

	private void EndFlash()
	{
		_light.LightEnergy = 0;
		_light.SpotAngle = DefaultSpotAngle;
		_light.SpotRange = DefaultRange;

		flickerEnabled = true;
		isFlashing = false;
	}

	// ==========================
	// Visual Update
	// ==========================
	private void UpdateLightVisuals()
	{
		if (isFlashing) return;

		if (!isOn)
		{
			_light.LightEnergy = 0;
			return;
		}

		if (flickerEnabled)
		{
			_light.LightEnergy = CalculateFlicker();
		}
		else
		{
			_light.LightEnergy = 6f;
		}
	}

	// ==========================
	// Fuel Drain
	// ==========================
	private void OnFuelTimerTimeout()
	{
		if (!isOn) return;

		PlayerData.calcium_fuel = Mathf.Max(0, PlayerData.calcium_fuel - 1f);

		if (PlayerData.calcium_fuel <= 0)
		{
			TurnLightOff();
		}
	}
}
