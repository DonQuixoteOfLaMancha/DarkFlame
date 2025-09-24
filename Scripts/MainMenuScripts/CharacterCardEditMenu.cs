using Godot;
using System;

public partial class CharacterCardEditMenu : Node2D
{
	int DonTxtIndex = 0;
	string FilePath = "user://Content/DefaultContent/DonTxtFiles/CharacterCards.txt";
	
	int PassiveIndex = 0;
	
	int[] CosmeticSpriteIndices = [0,0,0,0];
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateCosmeticSprites();
		GetChild<CharacterCard>(2).Refresh();
		UpdatePassive();
		UpdateCharacter();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	//CIF
	private void LoadCard(string DonTxt)
	{
		GetChild<CharacterCard>(2).SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(DonTxt));
		GetChild<CharacterCard>(2).Refresh();
		GetChild<Character>(3).LoadCardSprites();
		UpdateCharacter();
		PassiveIndex = 0;
		UpdatePassive();
	}
	
	private void UpdatePassive()
	{
		if(GetChild<CharacterCard>(2).PassiveList.Count == 0)
		{
			GetChild(1).GetChild(4).GetChild(7).GetChild<Node2D>(0).Hide();
			GetChild(1).GetChild(4).GetChild(7).GetChild<Node2D>(1).Hide();
			GetChild(1).GetChild(4).GetChild(7).GetChild<Node2D>(3).Hide();
		}
		else
		{
			GetChild(1).GetChild(4).GetChild(7).GetChild<Node2D>(0).Show();
			GetChild(1).GetChild(4).GetChild(7).GetChild<Node2D>(1).Show();
			GetChild(1).GetChild(4).GetChild(7).GetChild<Node2D>(3).Show();
			GetChild(1).GetChild(4).GetChild(7).GetChild(0).GetChild<TextEdit>(2).Text = GetChild<CharacterCard>(2).PassiveList[PassiveIndex].Trigger;
			for(int Index = 0; Index<4; Index++)
			{
				GetChild(1).GetChild(4).GetChild(7).GetChild(0).GetChild(0).GetChild<TextEdit>(Index).Text = GetChild<CharacterCard>(2).PassiveList[PassiveIndex].Condition[Index];
				GetChild(1).GetChild(4).GetChild(7).GetChild(0).GetChild(1).GetChild<TextEdit>(Index).Text = GetChild<CharacterCard>(2).PassiveList[PassiveIndex].Effect[Index];
			}
			GetChild(1).GetChild(4).GetChild(7).GetChild(1).GetChild(1).GetChild<Label>(2).Text = Convert.ToString(PassiveIndex);
		}
	}
	
	private void UpdateCharacter()
	{
		GetChild<Character>(3).CharCard = GetChild<CharacterCard>(2);
		GetChild<Character>(3).UpdateSpriteState();
		UpdateSpritePositions();
	}
	
	private void UpdateSpritePositions()
	{
		GetChild(1).GetChild(3).GetChild(2).GetChild(0).GetChild<TextEdit>(0).Text = Convert.ToString(GetChild<CharacterCard>(2).BackHairOrigins[GetChild<Character>(3).SpriteState, 0]);
		GetChild(1).GetChild(3).GetChild(2).GetChild(0).GetChild<TextEdit>(1).Text = Convert.ToString(GetChild<CharacterCard>(2).BackHairOrigins[GetChild<Character>(3).SpriteState, 1]);
		GetChild(1).GetChild(3).GetChild(2).GetChild(1).GetChild<TextEdit>(0).Text = Convert.ToString(GetChild<CharacterCard>(2).FrontHairOrigins[GetChild<Character>(3).SpriteState, 0]);
		GetChild(1).GetChild(3).GetChild(2).GetChild(1).GetChild<TextEdit>(1).Text = Convert.ToString(GetChild<CharacterCard>(2).FrontHairOrigins[GetChild<Character>(3).SpriteState, 1]);
		GetChild(1).GetChild(3).GetChild(2).GetChild(2).GetChild<TextEdit>(0).Text = Convert.ToString(GetChild<CharacterCard>(2).MouthOrigins[GetChild<Character>(3).SpriteState, 0]);
		GetChild(1).GetChild(3).GetChild(2).GetChild(2).GetChild<TextEdit>(1).Text = Convert.ToString(GetChild<CharacterCard>(2).MouthOrigins[GetChild<Character>(3).SpriteState, 1]);
		GetChild(1).GetChild(3).GetChild(2).GetChild(3).GetChild<TextEdit>(0).Text = Convert.ToString(GetChild<CharacterCard>(2).EyeOrigins[GetChild<Character>(3).SpriteState, 0]);
		GetChild(1).GetChild(3).GetChild(2).GetChild(3).GetChild<TextEdit>(1).Text = Convert.ToString(GetChild<CharacterCard>(2).EyeOrigins[GetChild<Character>(3).SpriteState, 1]);
	}
	
	private void UpdateCosmeticSprites()
	{
		for(int Index = 0; Index<4; Index++)
		{
			GetChild(1).GetChild(3).GetChild(1).GetChild(Index).GetChild(1).GetChild<Label>(2).Text = Convert.ToString(CosmeticSpriteIndices[Index]);
		}
		GetChild<Character>(3).CosmeticAttributes = CosmeticSpriteIndices;
		GetChild<Character>(3).UpdateCosmeticAttributes();
	}
	
	
	// Connection Inputs
	private void _File_Selected(string SelectedFilePath)
	{
		FilePath = SelectedFilePath;
		DonTxtIndex = 0;
		GetChild(1).GetChild(2).GetChild(3).GetChild<Label>(2).Text = "0";
	}
	
	private void _DonTxtIndex_Pressed(bool LeftRight)
	{
		if(LeftRight && DonTxtIndex>0) {DonTxtIndex--;}
		else if (!LeftRight && DonTxtIndex < DataStorageManager.GetNumberOfDonTxtEntries("CHARACTERCARD", DataStorageManager.ParseDonTxt(FilePath, "CHARACTERCARD"))-1) {DonTxtIndex++;}
		GetChild(1).GetChild(2).GetChild(3).GetChild<Label>(2).Text = Convert.ToString(DonTxtIndex);
	}
	
	private void _LoadCard_Pressed()
	{
		if(DataStorageManager.GetNumberOfDonTxtEntries("CHARACTERCARD", DataStorageManager.ParseDonTxt(FilePath, "CHARACTERCARD")) > 0)
		{
			LoadCard(DataStorageManager.GetDonTxtEntryFromIndex("CHARACTERCARD",DataStorageManager.ParseDonTxt(FilePath, "CHARACTERCARD"), DonTxtIndex));
		}
	}
	private void _SaveCard_Pressed()
	{DataStorageManager.StoreDonTxt(FilePath, "CHARACTERCARD", GetChild<CharacterCard>(2).ConvertToDonTxt(), "Name", GetChild<CharacterCard>(2).Name);}
	
	
	private void _SpriteStateChange_Pressed(bool LeftRight)
	{
		if(LeftRight && GetChild<Character>(3).SpriteState>0) {GetChild<Character>(3).SpriteState--;}
		else if (!LeftRight && GetChild<Character>(3).SpriteState < 8) {GetChild<Character>(3).SpriteState++;}
		GetChild(1).GetChild(3).GetChild(0).GetChild(1).GetChild<Label>(2).Text = Convert.ToString(GetChild<Character>(3).SpriteState);
		UpdateCharacter();
	}
	
	private void _CosmeticSpriteChange_Pressed(bool LeftRight, int Index)
	{
		if(LeftRight && CosmeticSpriteIndices[Index]>0) {CosmeticSpriteIndices[Index]--;}
		else if (!LeftRight && CosmeticSpriteIndices[Index] < 15) {CosmeticSpriteIndices[Index]++;}
		UpdateCosmeticSprites();
	}
	
	private void _CosmeticSpriteCoordinate_Changed(int XY, int Index)
	{
		if(GetChild(1).GetChild(3).GetChild(2).GetChild(Index).GetChild<TextEdit>(XY).Text == "")
		{GetChild(1).GetChild(3).GetChild(2).GetChild(Index).GetChild<TextEdit>(XY).Text = "0";}
		
		try
		{
			int Coordinate = Convert.ToInt32(GetChild(1).GetChild(3).GetChild(2).GetChild(Index).GetChild<TextEdit>(XY).Text);
			switch(Index)
			{
				case 0:
					GetChild<CharacterCard>(2).BackHairOrigins[GetChild<Character>(3).SpriteState, XY] = Coordinate;
					break;
				case 1:
					GetChild<CharacterCard>(2).FrontHairOrigins[GetChild<Character>(3).SpriteState, XY] = Coordinate;
					break;
				case 2:
					GetChild<CharacterCard>(2).MouthOrigins[GetChild<Character>(3).SpriteState, XY] = Coordinate;
					break;
				case 3:
					GetChild<CharacterCard>(2).EyeOrigins[GetChild<Character>(3).SpriteState, XY] = Coordinate;
					break;
			}
			GetChild<Character>(3).CharCard = GetChild<CharacterCard>(2);
			GetChild<Character>(3).UpdateSpriteState();
		}
		catch
		{}
	}
	
	private void _Name_Changed()
	{
		string InputName = GetChild(1).GetChild(4).GetChild<TextEdit>(0).Text;
		if(!(InputName.Contains("=") || InputName.Contains("@") || InputName.Contains("<") || InputName.Contains(">") || InputName.Contains(":") || InputName.Contains("#") || InputName.Contains("[") || InputName.Contains("]") || InputName.Contains(',')))
		{
			GetChild<CharacterCard>(2).Name = InputName;
			GetChild<CharacterCard>(2).Refresh();
		}
		else
		{
			GetChild(1).GetChild(4).GetChild<TextEdit>(0).Text = GetChild<CharacterCard>(2).Name;
		}
	}
	private void _Health_Changed()
	{
		try
		{
			GetChild<CharacterCard>(2).Health = Convert.ToInt32(GetChild(1).GetChild(4).GetChild<TextEdit>(1).Text);
		}
		catch
		{
			GetChild(1).GetChild(4).GetChild<TextEdit>(1).Text = "0";
			GetChild<CharacterCard>(2).Health = 0;
		}
		GetChild<CharacterCard>(2).Refresh();
	}
	private void _StaggerHealth_Changed()
	{
		try
		{
			GetChild<CharacterCard>(2).StaggerHealth = Convert.ToInt32(GetChild(1).GetChild(4).GetChild<TextEdit>(2).Text);
		}
		catch
		{
			GetChild(1).GetChild(4).GetChild<TextEdit>(2).Text = "0";
			GetChild<CharacterCard>(2).StaggerHealth = 0;
		}
		GetChild<CharacterCard>(2).Refresh();
	}
	
	private void _Speed_Changed(bool MinMax, int IncDecValue)
	{
		if(MinMax && GetChild<CharacterCard>(2).MinSpeed + IncDecValue <= GetChild<CharacterCard>(2).MaxSpeed)
		{
			GetChild<CharacterCard>(2).MinSpeed += IncDecValue;
			GetChild<CharacterCard>(2).Refresh();
		}
		else if(!MinMax && GetChild<CharacterCard>(2).MaxSpeed + IncDecValue >= GetChild<CharacterCard>(2).MinSpeed)
		{
			GetChild<CharacterCard>(2).MaxSpeed += IncDecValue;
			GetChild<CharacterCard>(2).Refresh();
		}
	}
	private void _StrengthGrade_Changed(int IncDecValue)
	{
		GetChild<CharacterCard>(2).StrengthGrade += IncDecValue;
		GetChild<CharacterCard>(2).Refresh();
	}
	private void _Resistances_Changed(int Index, double IncDecValue)
	{
		switch(Index)
		{
			case 0:
				GetChild<CharacterCard>(2).SlashRes += IncDecValue;
				break;
			case 1:
				GetChild<CharacterCard>(2).PierceRes += IncDecValue;
				break;
			case 2:
				GetChild<CharacterCard>(2).BluntRes += IncDecValue;
				break;
			case 3:
				GetChild<CharacterCard>(2).SlashStagRes += IncDecValue;
				break;
			case 4:
				GetChild<CharacterCard>(2).PierceStagRes += IncDecValue;
				break;
			case 5:
				GetChild<CharacterCard>(2).BluntStagRes += IncDecValue;
				break;
		}
		GetChild<CharacterCard>(2).Refresh();
	}
	
	private void _SpriteSheet_Selected(string SelectedFilePath)
	{
		try {GetChild<CharacterCard>(2).SpriteSheetSource = SelectedFilePath; GetChild<CharacterCard>(2).Refresh(); GetChild<Character>(3).LoadCardSprites();}
		catch {}
	}
	
	//Passive connections
	private void _AddPassive_Pressed()
	{
		GetChild<CharacterCard>(2).PassiveList.Add(new Passive());
		PassiveIndex = GetChild<CharacterCard>(2).PassiveList.Count-1;
		UpdatePassive();
	}
	private void _RemovePassive_Pressed()
	{
		if(PassiveIndex<GetChild < CharacterCard>(2).PassiveList.Count)
		{
			GetChild<CharacterCard>(2).PassiveList.RemoveAt(PassiveIndex);
			if(PassiveIndex==GetChild<CharacterCard>(2).PassiveList.Count) {PassiveIndex--;}
			UpdatePassive();
		}
	}
	private void _ChangePassive_Pressed(int IncDecValue)
	{
		if(PassiveIndex+IncDecValue > -1 && PassiveIndex+IncDecValue < GetChild<CharacterCard>(2).PassiveList.Count)
		{PassiveIndex += IncDecValue; UpdatePassive();}
	}
	private void _PassiveElement_Changed(int CondEffe, int Index)
	{
		switch(CondEffe)
		{
			case 0: //Trigger
				GetChild<CharacterCard>(2).PassiveList[PassiveIndex].Trigger = GetChild(1).GetChild(4).GetChild(7).GetChild(0).GetChild<TextEdit>(2).Text;
				break;
			case 1: //Condition
				GetChild<CharacterCard>(2).PassiveList[PassiveIndex].Condition[Index] = GetChild(1).GetChild(4).GetChild(7).GetChild(0).GetChild(0).GetChild<TextEdit>(Index).Text;
				break;
			case 2: //Effect
				GetChild<CharacterCard>(2).PassiveList[PassiveIndex].Effect[Index] = GetChild(1).GetChild(4).GetChild(7).GetChild(0).GetChild(1).GetChild<TextEdit>(Index).Text;
				break;
		}
	}
}
