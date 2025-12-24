using Godot;
using System;

public partial class BorisScript : Node3D, Interactable
{
	AnimationPlayer borisAP;
	[Export] AudioStreamPlayer3D sound;
	string[] lines =
	{
		"res://Models/Characters/Boris/Voicelines/items.mp3"
		//"res://Models/Characters/Boris/Voicelines/speech_one.wav",
		//"res://Models/Characters/Boris/Voicelines/speech_two.wav",
		//"res://Models/Characters/Boris/Voicelines/speech_three.wav",
	};
	int line = 0;

	public override void _Ready()
	{
		borisAP = GetNode<AnimationPlayer>("AnimationPlayer");
		borisAP.Play("Hunchback");

	}

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
		playerone.equipItem(playerone.Items.all, true);
	}

}
