using Godot;
using System;

public partial class Stamina : Node2D
{
	private bool active = false;
	public bool Active
	{
		get {return active;}
		set
		{
			active = value;
			GetChild<Node2D>(1).Visible = active;
		}
	}
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for(int Index = 0; Index<GetParent().GetChildCount(); Index++)
		{
			GetParent().GetChild<Node2D>(Index).SetPosition(new Vector2(0, Index*45));
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
