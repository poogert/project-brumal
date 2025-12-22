using Godot;
using System;

public partial class PickaxeScript : Node3D
{
	
	const double HOLD_TIME = .5; 
	double holdingTimer = 0;
	
	private Tween swingTween;
	private Vector3 originalLocalPosition;
	private bool isHolding = false;

	public override void _Ready()
	{
		originalLocalPosition = Position;
	}

	public override void _Process(double delta)
	{

		if (Input.IsActionPressed("leftclick"))
		{
			holdingTimer += delta;
			
			if (!isHolding)
			{
				isHolding = true;
				PlayWindupTween();
			}

			if (holdingTimer >= HOLD_TIME)
			{
				Mine();
				PlayStrikeTween();
				holdingTimer = 0.0;
				
			}
		}
		else
		{
			if (isHolding) 
			{ 
				ResetSwingTween();
				isHolding = false;
			}

			holdingTimer = 0.0;
		}

	}

	public void Mine()
	{

		GD.Print("Attempting mining");

		if (PlayerData.collider == null) 
		{ 
			GD.Print("raycast isnt grabbing anything");
			return;
		}

		Node nodeobject = PlayerData.collider.GetParent().GetParent();
		Mineable mineable = findObject(nodeobject);

		if (mineable != null) 
		{ 
			mineable.Mine(); 
			GD.Print("success");

		} else
		{
			GD.Print("the thing youre mining is wrong -> " + PlayerData.collider.GetParent().GetParent().Name);
			GD.Print("failed");
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
		if (node == null)
			return;

		string indent = new string(' ', depth * 2);

		GD.Print($"{indent}- {node.Name} ({node.GetType().Name})");
		if (node is Mineable mineable) GD.Print("is mineable");

		foreach (Node child in node.GetChildren())
		{
			PrintTree(child, depth + 1);
		}
	}

	private void PlayWindupTween()
	{
		swingTween?.Kill();
		swingTween = CreateTween();

		Vector3 windupOffset = new Vector3(0, 0, 0.7f);

		swingTween.TweenProperty(
			this,
			"position",
			originalLocalPosition + windupOffset,
			0.5f
		).SetTrans(Tween.TransitionType.Quad)
		.SetEase(Tween.EaseType.Out);
	}

	private void PlayStrikeTween()
	{
		swingTween?.Kill();
		swingTween = CreateTween();

		Vector3 strikeOffset = new Vector3(0, 0, -1f);

		// Fast strike forward
		swingTween.TweenProperty(
			this,
			"position",
			originalLocalPosition + strikeOffset,
			0.03f
		).SetTrans(Tween.TransitionType.Quad)
		.SetEase(Tween.EaseType.Out);

		// Return to rest
		swingTween.TweenProperty(
			this,
			"position",
			originalLocalPosition,
			0.15f
		).SetTrans(Tween.TransitionType.Quad)
		.SetEase(Tween.EaseType.In);
	}

	private void ResetSwingTween()
	{
		// Kill any active tween
		swingTween?.Kill();
		swingTween = null;

		// Hard reset transform
		Position = originalLocalPosition;
	}


}
