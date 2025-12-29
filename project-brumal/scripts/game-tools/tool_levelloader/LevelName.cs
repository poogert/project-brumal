using Godot;
using System;

public partial class LevelName : Label3D
{
	public PackedScene level;
	public override void _Ready()
	{
		var parentnode = GetParent().GetParent(); // parent
		var parent = parentnode as SetLevelField; // accessible field from parent

		if (parent != null) 
		{ 
			level = parent.next_level;
			Text = "this level loads\n" + level.ResourcePath;
		} 
		else
		{
			Text = "level loader null\n pls import";
		}

	}
}
