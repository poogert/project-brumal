using Godot;
using System;

public partial class InteractableBox : Node3D, Interactable
{
	[Export] AudioStreamPlayer3D sound;

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
