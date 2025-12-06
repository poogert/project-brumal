using Godot;
using System;

public partial class InteractableBox : Node3D, Interactable
{
	// Called when the node enters the scene tree for the first time.

	[Export] StaticBody3D mesh;
	[Export] AudioStreamPlayer3D sound;

	public override void _Ready()
	{

	}

	public void PlaySound()
	{
		if (!sound.Playing) 
		{ 
			sound.Play(); 
			GD.Print("b b b break it down...");
		} else
		{
			sound.Stop(); 
			GD.Print("music off");
		}

	}

	public void Interact()
	{

		PlaySound();	
		

	}

}
