using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class PickaxeScript : Node3D
{
	
	AnimationTree animationTree;
	AnimationNodeStateMachinePlayback stateMachine;
	const double HOLD_TIME = .5; 
	double holdingTimer = 0;

	int defaultAnim = 1;
	private string[] Anim = { "Equip", "IdleReal", "Charge", "Hit", "Miss", "Sprint" };
	private bool isHolding = false;

	public override void _Ready()
    {
        animationTree = GetNode<AnimationTree>("AnimationTree");
        stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
		stateMachine.Start(Anim[0]);
	}

	public override void _Process(double delta)
	{
		//GD.Print(holdingTimer);
		if (PlayerData.CurrentState == "running" && isHolding == false && !(Input.IsActionPressed("leftclick")))
		{
			stateMachine.Travel(Anim[5]);
		}else if (!(PlayerData.CurrentState == "running") && isHolding == false && !(Input.IsActionPressed("leftclick"))){
			stateMachine.Travel(Anim[1]);
		}
		if (Input.IsActionPressed("leftclick") && !(stateMachine.GetCurrentNode() == Anim[4]) && !(stateMachine.GetCurrentNode() == Anim[3]))
		{
			holdingTimer += delta;
			
			if (!isHolding)
			{
				isHolding = true;
				//PlayWindupTween();
				stateMachine.Travel(Anim[2]);
			}

			
		}else
		{	
			if (holdingTimer >= HOLD_TIME)
			{
				Mine();
				if (Mine() == true)
				{
					stateMachine.Travel(Anim[3]);
				}
				else
				{
					stateMachine.Travel(Anim[4]);

				}
				
				//PlayStrikeTween();
				holdingTimer = 0.0;
				
			}else 	if (isHolding) 
			{ 
				//ResetSwingTween();
				stateMachine.Travel(Anim[1]);
				isHolding = false;
			}
			
			holdingTimer = 0.0;
		}
		
			
		

	}

	public bool Mine()
	{

		GD.Print("Attempting mining");

		if (PlayerData.collider == null) 
		{ 
			GD.Print("raycast isnt grabbing anything");
			return false;
		}

		Node nodeobject = PlayerData.collider.GetParent().GetParent();
		Mineable mineable = findObject(nodeobject);
			
		if (mineable != null) 
		{ 
			mineable.Mine(); 
			GD.Print("success");
			return true;

		} else
		{
			GD.Print("the thing youre mining is wrong -> " + nodeobject.Name);
			GD.Print("failed");
			return false;
		}
		
	}

	private Mineable findObject(Node target) 
	{
		if (target == null) return null;

		if (target is Mineable mineable) return mineable;

		return findObject(target.GetParent());
	}

	private void PrintTree(Node node, int depth = 0)
	{
		if (node == null) return;

		string indent = new string(' ', depth * 2);

		GD.Print($"{indent}- {node.Name} ({node.GetType().Name})");
		if (node is Mineable mineable) GD.Print("is mineable");

		foreach (Node child in node.GetChildren())
		{
			PrintTree(child, depth + 1);
		}
	}


}
