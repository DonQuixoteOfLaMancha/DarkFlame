using Godot;
using System;

public partial class CombatCardEditMenu : Node2D
{
	int DonTxtIndex = 0;
	string FilePath = "user://Content/DefaultContent/DonTxtFiles/CombatCards.txt";
	
	int DiceIndex = 0;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetChild<CombatCard>(1).Refresh();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//CIF
	private void LoadCard(string DonTxt)
	{
		GetChild<CombatCard>(1).SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(DonTxt));
		GetChild<CombatCard>(1).Refresh();
		DiceIndex = 0;
		UpdateConditional();
		UpdateDice();
	}
	
	private void UpdateConditional()
	{
		GetChild(0).GetChild(3).GetChild(1).GetChild<TextEdit>(2).Text = GetChild<CombatCard>(1).ConditionalTrigger;
		for(int Index = 0; Index<4; Index++)
		{
			GetChild(0).GetChild(3).GetChild(1).GetChild(0).GetChild<TextEdit>(Index).Text = GetChild<CombatCard>(1).Condition[Index];
			GetChild(0).GetChild(3).GetChild(1).GetChild(1).GetChild<TextEdit>(Index).Text = GetChild<CombatCard>(1).Effect[Index];
		}
	}
	private void UpdateDice()
	{
		if(GetChild<CombatCard>(1).Dice.Count == 0)
		{
			GetChild(0).GetChild(3).GetChild(2).GetChild<Node2D>(0).Hide();
			GetChild(0).GetChild(3).GetChild(2).GetChild<Node2D>(1).Hide();
			GetChild(0).GetChild(3).GetChild(2).GetChild<Node2D>(3).Hide();
			GetChild(0).GetChild(3).GetChild(2).GetChild<Node2D>(4).Hide();
			GetChild(0).GetChild(3).GetChild(2).GetChild<Node2D>(5).Hide();
			GetChild(0).GetChild(3).GetChild(2).GetChild<Node2D>(7).Hide();
		}
		else
		{
			GetChild(0).GetChild(3).GetChild(2).GetChild<Node2D>(0).Show();
			GetChild(0).GetChild(3).GetChild(2).GetChild<Node2D>(1).Show();
			GetChild(0).GetChild(3).GetChild(2).GetChild<Node2D>(3).Show();
			GetChild(0).GetChild(3).GetChild(2).GetChild<Node2D>(4).Show();
			GetChild(0).GetChild(3).GetChild(2).GetChild<Node2D>(5).Show();
			GetChild(0).GetChild(3).GetChild(2).GetChild<Node2D>(7).Show();
			
			GetChild(0).GetChild(3).GetChild(2).GetChild(0).GetChild<TextEdit>(2).Text = GetChild<CombatCard>(1).Dice[DiceIndex].ConditionalTrigger;
			for(int Index = 0; Index<4; Index++)
			{
				GetChild(0).GetChild(3).GetChild(2).GetChild(0).GetChild(0).GetChild<TextEdit>(Index).Text = GetChild<CombatCard>(1).Dice[DiceIndex].Condition[Index];
				GetChild(0).GetChild(3).GetChild(2).GetChild(0).GetChild(1).GetChild<TextEdit>(Index).Text = GetChild<CombatCard>(1).Dice[DiceIndex].Effect[Index];
			}
			GetChild(0).GetChild(3).GetChild(2).GetChild(1).GetChild(1).GetChild<Label>(2).Text = Convert.ToString(DiceIndex);
			
			GetChild(0).GetChild(3).GetChild(2).GetChild<CombatDice>(7).CopyDice(GetChild<CombatCard>(1).Dice[DiceIndex]);
			GetChild(0).GetChild(3).GetChild(2).GetChild<CombatDice>(7).Refresh();
		}
	}
	
	
	// Connection Inputs
	private void _File_Selected(string SelectedFilePath)
	{
		FilePath = SelectedFilePath;
		DonTxtIndex = 0;
		GetChild(0).GetChild(2).GetChild(3).GetChild<Label>(2).Text = "0";
	}
	
	private void _DonTxtIndex_Pressed(bool LeftRight)
	{
		if(LeftRight && DonTxtIndex>0) {DonTxtIndex--;}
		else if (!LeftRight && DonTxtIndex < DataStorageManager.GetNumberOfDonTxtEntries("COMBATCARD", DataStorageManager.ParseDonTxt(FilePath, "COMBATCARD"))-1) {DonTxtIndex++;}
		GetChild(0).GetChild(2).GetChild(3).GetChild<Label>(2).Text = Convert.ToString(DonTxtIndex);
	}
	
	private void _LoadCard_Pressed()
	{
		if(DataStorageManager.GetNumberOfDonTxtEntries("COMBATCARD", DataStorageManager.ParseDonTxt(FilePath, "COMBATCARD")) > 0)
		{
			LoadCard(DataStorageManager.GetDonTxtEntryFromIndex("COMBATCARD",DataStorageManager.ParseDonTxt(FilePath, "COMBATCARD"), DonTxtIndex));
		}
	}
	public void _Save_Pressed()
	{DataStorageManager.StoreDonTxt(FilePath, "COMBATCARD", GetChild<CombatCard>(1).ConvertToDonTxt(), "Name", GetChild<CombatCard>(1).Name);}
	
	
	private void _Name_Changed()
	{
		{
		string InputName = GetChild(0).GetChild(3).GetChild<TextEdit>(3).Text;
		if(!(InputName.Contains("=") || InputName.Contains("@") || InputName.Contains("<") || InputName.Contains(">") || InputName.Contains(":") || InputName.Contains("#") || InputName.Contains("[") || InputName.Contains("]") || InputName.Contains(',')))
		{
			GetChild<CombatCard>(1).Name = InputName;
			GetChild<CombatCard>(1).Refresh();
		}
		else
		{
			GetChild(0).GetChild(3).GetChild<TextEdit>(3).Text = GetChild<CombatCard>(1).Name;
		}
	}
	}
	private void _StaminaCost_Changed()
	{
		try
		{
			GetChild<CombatCard>(1).StaminaCost = Convert.ToInt32(GetChild(0).GetChild(3).GetChild<TextEdit>(4).Text);
		}
		catch
		{
			GetChild(0).GetChild(3).GetChild<TextEdit>(4).Text = "0";
			GetChild<CombatCard>(1).StaminaCost = 0;
		}
		GetChild<CombatCard>(1).Refresh();
	}
	
	private void _PageArt_Selected(string SelectedFilePath)
	{
		try {GetChild<CombatCard>(1).CardArtPath = SelectedFilePath; GetChild<CombatCard>(1).Refresh();}
		catch {}
	}
	
	private void _StrengthGrade_Changed(int IncDecValue)
	{
		GetChild<CombatCard>(1).StrengthGrade += IncDecValue;
		GetChild<CombatCard>(1).Refresh();
	}
	
	private void _CardConditionalElement_Changed(int CondEffe, int Index)
	{
		switch(CondEffe)
		{
			case 0: //Trigger
				GetChild<CombatCard>(1).ConditionalTrigger = GetChild(0).GetChild(3).GetChild(1).GetChild<TextEdit>(2).Text;
				break;
			case 1: //Condition
				GetChild<CombatCard>(1).Condition[Index] = GetChild(0).GetChild(3).GetChild(1).GetChild(0).GetChild<TextEdit>(Index).Text;
				break;
			case 2: //Effect
				GetChild<CombatCard>(1).Effect[Index] = GetChild(0).GetChild(3).GetChild(1).GetChild(1).GetChild<TextEdit>(Index).Text;
				break;
		}
		GetChild<CombatCard>(1).Refresh(); 
	}
	
	
	//Dice connections
	private void _AddDice_Pressed()
	{
		GetChild<CombatCard>(1).Dice.Add(new CombatDice());
		DiceIndex = GetChild<CombatCard>(1).Dice.Count-1;
		GetChild<CombatCard>(1).Refresh();
		UpdateDice();
	}
	private void _RemoveDice_Pressed()
	{
		GetChild<CombatCard>(1).Dice.RemoveAt(DiceIndex);
		if(DiceIndex==GetChild<CombatCard>(1).Dice.Count) {DiceIndex--;}
		GetChild<CombatCard>(1).Refresh();
		UpdateDice();
	}
	private void _ChangeDice_Pressed(int IncDecValue)
	{
		if(DiceIndex+IncDecValue > -1 && DiceIndex+IncDecValue < GetChild<CombatCard>(1).Dice.Count)
		{DiceIndex += IncDecValue; UpdateDice();}
	}
	
	private void _ChangeDiceType_Pressed(int IncDecValue)
	{
		if(GetChild<CombatCard>(1).Dice[DiceIndex].DiceType + IncDecValue >= 0 && GetChild<CombatCard>(1).Dice[DiceIndex].DiceType + IncDecValue <= 4)
		{
			GetChild<CombatCard>(1).Dice[DiceIndex].DiceType += IncDecValue;
			GetChild<CombatCard>(1).Refresh();
			GetChild(0).GetChild(3).GetChild(2).GetChild<CombatDice>(7).CopyDice(GetChild<CombatCard>(1).Dice[DiceIndex]);
			GetChild(0).GetChild(3).GetChild(2).GetChild<CombatDice>(7).Refresh();
		}
	}
	private void _DiceRoll_Changed(bool MinMax, int IncDecValue)
	{
		if(MinMax && GetChild<CombatCard>(1).Dice[DiceIndex].MinRoll + IncDecValue <= GetChild<CombatCard>(1).Dice[DiceIndex].MaxRoll && GetChild<CombatCard>(1).Dice[DiceIndex].MinRoll + IncDecValue >= 0)
		{
			GetChild<CombatCard>(1).Dice[DiceIndex].MinRoll += IncDecValue;
			GetChild<CombatCard>(1).Refresh();
			GetChild(0).GetChild(3).GetChild(2).GetChild<CombatDice>(7).CopyDice(GetChild<CombatCard>(1).Dice[DiceIndex]);
			GetChild(0).GetChild(3).GetChild(2).GetChild<CombatDice>(7).Refresh();
		}
		else if(!MinMax && GetChild<CombatCard>(1).Dice[DiceIndex].MaxRoll + IncDecValue >= GetChild<CombatCard>(1).Dice[DiceIndex].MinRoll)
		{
			GetChild<CombatCard>(1).Dice[DiceIndex].MaxRoll += IncDecValue;
			GetChild<CombatCard>(1).Refresh();
			GetChild(0).GetChild(3).GetChild(2).GetChild<CombatDice>(7).CopyDice(GetChild<CombatCard>(1).Dice[DiceIndex]);
			GetChild(0).GetChild(3).GetChild(2).GetChild<CombatDice>(7).Refresh();
		}
	}
	
	private void _DiceConditionalElement_Changed(int CondEffe, int Index)
	{
		switch(CondEffe)
		{
			case 0: //Trigger
				GetChild<CombatCard>(1).Dice[DiceIndex].ConditionalTrigger = GetChild(0).GetChild(3).GetChild(2).GetChild(0).GetChild<TextEdit>(2).Text;
				break;
			case 1: //Condition
				GetChild<CombatCard>(1).Dice[DiceIndex].Condition[Index] = GetChild(0).GetChild(3).GetChild(2).GetChild(0).GetChild(0).GetChild<TextEdit>(Index).Text;
				break;
			case 2: //Effect
				GetChild<CombatCard>(1).Dice[DiceIndex].Effect[Index] = GetChild(0).GetChild(3).GetChild(2).GetChild(0).GetChild(1).GetChild<TextEdit>(Index).Text;
				break;
		}
		GetChild<CombatCard>(1).Refresh();
		GetChild(0).GetChild(3).GetChild(2).GetChild<CombatDice>(7).CopyDice(GetChild<CombatCard>(1).Dice[DiceIndex]);
		GetChild(0).GetChild(3).GetChild(2).GetChild<CombatDice>(7).Refresh();
	}
}
