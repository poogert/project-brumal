using Godot;
using System;

public partial class BorisScript : Node3D, Interactable
{
	
	[Export] AudioStreamPlayer3D sound;
	string[] lines =
	{
		"res://Models/Characters/Boris/Voicelines/speech_one.wav",
		"res://Models/Characters/Boris/Voicelines/speech_two.wav",
		"res://Models/Characters/Boris/Voicelines/speech_three.wav"
	};
	int line = 0;

	public void PlaySound()
	{
		if (line >= lines.Length) line = 0;

		sound.Stream = ResourceLoader.Load<AudioStream>(lines[line]);
		GD.Print("sound : " + sound.Stream);
		sound.Play();
		
		line++;

	}

	public void Interact()
	{
		PlaySound();
	}

}
