using Godot;
using System;

public partial class SpeedDice : Node2D
{
	Random Rand = new Random();
	private int hiddenValue;
	public int Value
	{
		get {return hiddenValue;}
		set
		{
			hiddenValue = value;
			GetChild<Label>(2).Text = Convert.ToString(Value);
		}
	}
	
	public Character Target;
	public CombatCard SlotCard;
	
	public double RollTimer = 0.0;
	int RollIndex = 0;
	public int OriginalPosX = 0;
	public int OriginalPosY = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(RollTimer > 0)
		{
			Scale = new Vector2(1, (float)Math.Cos(RollTimer*2));
			Position = new Vector2(OriginalPosX, OriginalPosY+400*(float)Math.Sin(RollTimer));
			GetChild<Node2D>(4).Scale = new Vector2(3, 3/(float)Math.Cos(RollTimer*2));
			GetChild<Node2D>(4).Position = new Vector2(-80, (-800-400*(float)Math.Sin(RollTimer))/(float)Math.Cos(RollTimer*2));
			RollTimer -= delta;
			Value = Rand.Next(SlotCard.Dice[RollIndex].MinRoll, SlotCard.Dice[RollIndex].MaxRoll+1);
			if(RollTimer <= 0)
			{
				RollTimer = 0;
				Scale = new Vector2(1, 1);
				Position = new Vector2(OriginalPosX, OriginalPosY);
				GetChild<Node2D>(4).Scale = new Vector2(3, 3);
				GetChild<Node2D>(4).Position = new Vector2(-80, -800);
			}
		}
	}
	
	//CIF
	public void AssignTarget(Character InputChar, CombatCard InputCard)
	{
		if(SlotCard != null) {GetParent().GetParent().GetParent<Character>().Stamina += SlotCard.StaminaCost;}
		Target = InputChar;
		SlotCard = InputCard;
		GetParent().GetParent().GetParent<Character>().Stamina -= SlotCard.StaminaCost;
		GetChild<CombatCard>(4).CopyCombatCard(SlotCard);
		GetChild<CombatCard>(4).Refresh();
	}
	
	public void Clear()
	{
		GetParent().GetParent().GetParent<Character>().Hand.Add(SlotCard);
		GetParent().GetParent().GetParent<Character>().Stamina += SlotCard.StaminaCost;
		Target = null;
		SlotCard = null;
		Unhovered();
	}
	
	public void RollIndexDice(int InputIndex)
	{
		if(InputIndex < SlotCard.Dice.Count && GetParent().GetParent().GetParent<Character>().StaggerHealth > 0)
		{
			RollTimer = 3.141;
			RollIndex = InputIndex;
		}
		else
		{
			Hide();
			Value = -1984; //Just picked a really low number
		}
	}
	
	//Connection Inputs
	private void Clicked()
	{
		GetParent().GetParent().GetParent().GetParent().GetParent().GetParent<BattleSceneManager>().SpeedDiceClicked(this);
	}
	
	private void Hovered()
	{
		if(SlotCard != null)
		{
			GetParent().GetParent().GetParent().GetParent().GetParent().GetParent<BattleSceneManager>().SpeedDiceHovered(this);
		}
		if(Target != null)
		{
			GetChild<Node2D>(5).Show();
			GetChild<Node2D>(5).SetGlobalPosition(Target.GlobalPosition+new Vector2(134, 134));
		}
	}
	private void Unhovered()
	{
		GetParent().GetParent().GetParent().GetParent().GetParent().GetParent<BattleSceneManager>().SpeedDiceUnhovered(this);
		GetChild<Node2D>(5).Hide();
	}
}
