using Godot;
using System;

public partial class Stitcher : Node3D
{

	
	
	//Area3D StitcherArea;
	[Export] AnimationTree StitcherAT;
	[Export] AnimationPlayer StitcherAP;
	Boolean End = false;
	String prevAnim;
	AnimationNodeStateMachinePlayback stateMachine;


	
	[Export] Area3D StitcherArea;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		stateMachine = (AnimationNodeStateMachinePlayback)StitcherAT.Get("parameters/playback");

	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		if(StitcherArea.OverlapsBody(GetTree().CurrentScene.GetNode("Player")))
		{	
			GD.Print("Player is in the area");
			StitcherAT.Set("parameters/StateMachine/conditions/Area3D", true);
			
			
		}
		
		
	}
	public  void _on_animation_tree_animation_finished(String anim_name)
	{
		if (anim_name == "PeekCaught")
		{
			this.QueueFree();
		}
	}
}
