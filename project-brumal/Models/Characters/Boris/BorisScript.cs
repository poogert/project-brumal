using Godot;
using System;
using System.Threading.Tasks;

public partial class BorisScript : Node3D, Interactable
{
	AnimationPlayer borisAP;
	[Export] AnimationTree borisAT;
	[Export] AudioStreamPlayer3D sound;
	/*string[] lines =
	{
		"res://Models/Characters/Boris/Voicelines/items.mp3",
		"res://Models/Characters/Boris/Voicelines/speech_one.wav",
		"res://Models/Characters/Boris/Voicelines/speech_two.wav",
		"res://Models/Characters/Boris/Voicelines/speech_three.wav",
	};
		*/
	string[] lines =
	{
		"res://Models/Characters/Boris/Voicelines/NewVL/VoiceMeno1.wav",
		"res://Models/Characters/Boris/Voicelines/NewVL/VoiceMeno2.wav",
		"res://Models/Characters/Boris/Voicelines/NewVL/Voicemeno3.wav",
		"res://Models/Characters/Boris/Voicelines/NewVL/Voicemeno4.wav",
	};
	int line = 0;

	public override void _Ready()
	{
		borisAP = GetNode<AnimationPlayer>("AnimationPlayer");
		borisAP.Play("Hunchback");

	}

	public async Task PlaySound()
	{
		if (line >= lines.Length) line = 0;
		
		sound.Stream = ResourceLoader.Load<AudioStream>(lines[line]);
		GD.Print("sound : " + sound.Stream);
		sound.Play();
		line++;
		borisAT.Set("parameters/MouthBlend/add_amount", 1.0);
		GD.Print("mouthing");

		while (sound.IsPlaying())
		{
			 await Task.Delay(100); // Check every 100 milliseconds
		}
		GD.Print(" not mouthing");
		borisAT.Set("parameters/MouthBlend/add_amount", 0.0);

		
		
		

	
	}

	public void Interact()
	{
		PlaySound();
		playerone.equipItem(playerone.Items.all, true);
	}

}
