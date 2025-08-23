using Godot;
using System;
using System.Collections.Generic;

public partial class PassiveContainer : Node2D
{
	int NewPassive = 0;
	List<Passive> Passives = new List<Passive>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(NewPassive == 2 || NewPassive == 3) {NewPassive--;}
		if(NewPassive == 1)
		{
			for(int Index = 0; Index<Passives.Count; Index++)
			{
				Passive TargetPassive = GetChild<Passive>(Index+1);
				TargetPassive.Trigger = Passives[Index].Trigger;
				TargetPassive.Condition = Passives[Index].Condition;
				TargetPassive.Effect = Passives[Index].Effect;
				TargetPassive.DescOverride = Passives[Index].DescOverride;
				TargetPassive.Refresh();
				TargetPassive.Position = new Vector2(0,60*Index);
			}
			GetChild<VScrollBar>(0).MaxValue = 60*(GetChildCount()-3);
			GetChild<VScrollBar>(0).Page = 60;
			GetChild<VScrollBar>(0).Value = 0;
			NewPassive = 0;
		}
	}
	
	//CIF
	public void UpdatePassives(List<Passive> PassivesList)
	{
		Passives = PassivesList;
		for(int Index = 1; Index<GetChildCount(); Index++) {GetChild(Index).QueueFree();}
		var PassiveScene = GD.Load<PackedScene>("res://Scenes/ObjectScenes/Passive.tscn");
		for(int Index = 0; Index<PassivesList.Count; Index++)
		{
			var PassiveInstance = PassiveScene.Instantiate();
			AddChild(PassiveInstance);
		}
		NewPassive = 3;
	}
	
	//Connection Inputs
	private void ScrollMoved(float ScrollValue)
	{
		for(int Index = 1; Index<GetChildCount(); Index++)
		{
			Passive TargetPassive = GetChild<Passive>(Index);
			Vector2 Position = new Vector2(0,60*(Index-1)-ScrollValue);
			TargetPassive.Position = Position;
			if(Position.Y < 0) {TargetPassive.Hide();}
			else {TargetPassive.Show();}
		}
	}
}
