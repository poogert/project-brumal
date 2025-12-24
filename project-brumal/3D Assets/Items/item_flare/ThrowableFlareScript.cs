using Godot;
using System;

public partial class ThrowableFlareScript : Node3D
{
	[Export] Timer timer;
	[Export] OmniLight3D light;
	[Export] Node3D particles;
	
	private GpuParticles3D burn;
	private GpuParticles3D smoke;
	
	public override void _Ready()
	{
		timer.Timeout += flareDie;
		burn = particles.GetNode<GpuParticles3D>("burn");
		smoke = particles.GetNode<GpuParticles3D>("smoke");
	}

	
	public override void _Process(double delta)
	{
	}
	
	private void flareDie() 
	{
		Tween tween = CreateTween();
		tween.SetEase(Tween.EaseType.Out);
		tween.SetTrans(Tween.TransitionType.Cubic);

		tween.TweenProperty(light, "light_energy", 0f, 1.0f);
		tween.TweenProperty(burn, "amount_ratio", 0f, 1.0f);
		tween.TweenCallback(Callable.From(() => burn.Emitting = false));

		tween.TweenProperty(smoke, "amount_ratio", 0f, 5.0f);
		tween.TweenCallback(Callable.From(() => smoke.Emitting = false));
	}
}
