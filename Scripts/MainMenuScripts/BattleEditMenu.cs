using Godot;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public partial class BattleEditMenu : Node2D
{
	//Editing Menu attributes
	int DonTxtIndex = 0;
	string FilePath = "user://Content/DefaultContent/DonTxtFiles/Battles.txt";
	
	
	Battle WorkingBattle = new Battle();
	
	int EncounterIndex = 0;
	int CharacterIndex = 0;
	
	int SwitchMenuIndex = 0;
	
	//Char Cosmetic Attributes
	int[] CosmeticSpriteIndices = [0,0,0,0];
	
	//CharPage Selection Attributes
	int CharCardPageIndex = 0;
	int CharCardMaxPage = 0;
	public List<CharacterCard> CharacterCardList = new List<CharacterCard>();
	
	//Deck Attributes
	int CombatCardPageIndex = 0;
	int CombatCardMaxPage = 0;
	public List<CombatCard> CombatCardList = new List<CombatCard>();
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateEncounter();
		DisplaySubmenu();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//CIF
	private void LoadBattle(string DonTxt) //UNFINISHED
	{
		WorkingBattle.SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(DonTxt));
		EncounterIndex = 0;
		UpdateEncounter();
		GetChild(1).GetChild(8).GetChild<TextEdit>(0).Text = WorkingBattle.Name;
		
		switch(WorkingBattle.DiffGrade)
		{
			case 1: GetChild(1).GetChild(9).GetChild<Polygon2D>(3).Color = new Color("2e8799"); break;
			case 2: GetChild(1).GetChild(9).GetChild<Polygon2D>(3).Color = new Color("660a0a"); break;
			case 3: GetChild(1).GetChild(9).GetChild<Polygon2D>(3).Color = new Color("195e80"); break;
			case 4: GetChild(1).GetChild(9).GetChild<Polygon2D>(3).Color = new Color("080a99"); break;
			case 5: GetChild(1).GetChild(9).GetChild<Polygon2D>(3).Color = new Color("997a1f"); break;
			default:
				GetChild(1).GetChild(9).GetChild<Polygon2D>(3).Color = new Color("cccccc");
				break;
		}
		GetChild(1).GetChild(9).GetChild<Label>(4).Text = Convert.ToString(WorkingBattle.DiffGrade);
	}
	
	private void UpdateEncounter()
	{
		if(WorkingBattle.Encounters.Count == 0) {WorkingBattle.Encounters.Add(new Encounter());}
		
		GetChild(1).GetChild(4).GetChild<Label>(2).Text = Convert.ToString(EncounterIndex);
		
		UpdateBattleEncounterSubmenu();
		
		CharacterIndex = 0;
		UpdateCharacter();
	}
	private void UpdateCharacter()
	{
		GetChild(1).GetChild(5).GetChild<Label>(2).Text = Convert.ToString(CharacterIndex);
		
		GetChild<Character>(3).CopyCharacter(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex]);
		GetChild<Character>(3).Refresh();
		
		UpdateCharCosmeticSubmenu();
		UpdateCharCardSubmenu();
		UpdateCharDeckSubmenu();
	}
	
	private void _StrengthGrade_Changed(int IncDecValue)
	{
		WorkingBattle.DiffGrade += IncDecValue;
		switch(WorkingBattle.DiffGrade)
		{
			case 1: GetChild(1).GetChild(9).GetChild<Polygon2D>(3).Color = new Color("2e8799"); break;
			case 2: GetChild(1).GetChild(9).GetChild<Polygon2D>(3).Color = new Color("660a0a"); break;
			case 3: GetChild(1).GetChild(9).GetChild<Polygon2D>(3).Color = new Color("195e80"); break;
			case 4: GetChild(1).GetChild(9).GetChild<Polygon2D>(3).Color = new Color("080a99"); break;
			case 5: GetChild(1).GetChild(9).GetChild<Polygon2D>(3).Color = new Color("997a1f"); break;
			default:
				GetChild(1).GetChild(9).GetChild<Polygon2D>(3).Color = new Color("cccccc");
				break;
		}
		
		GetChild(1).GetChild(9).GetChild<Label>(4).Text = Convert.ToString(WorkingBattle.DiffGrade);
	}
	
	
	//Submenu methods
	//BattleEncounter Submenu
	private void UpdateBattleEncounterSubmenu()
	{
		GetChild(2).GetChild(0).GetChild(2).GetChild<Sprite2D>(1).Texture = WorkingBattle.Icon;
		if(WorkingBattle.Icon != null) {GetChild(2).GetChild(0).GetChild(2).GetChild<Sprite2D>(1).Scale = new Vector2((float)175/WorkingBattle.Icon.GetWidth(), (float)175/WorkingBattle.Icon.GetHeight());}
		GetChild(2).GetChild(0).GetChild(3).GetChild<Sprite2D>(1).Texture = WorkingBattle.Encounters[EncounterIndex].BackGroundImage;
		if(WorkingBattle.Encounters[EncounterIndex].BackGroundImage != null) {GetChild(2).GetChild(0).GetChild(3).GetChild<Sprite2D>(1).Scale = new Vector2((float)320/WorkingBattle.Encounters[EncounterIndex].BackGroundImage.GetWidth(), (float)180/WorkingBattle.Encounters[EncounterIndex].BackGroundImage.GetHeight());}
	}
	
	//CosmeticDetailSubmenu
	private void UpdateCharCosmeticSubmenu()
	{
		//Sprites
		CosmeticSpriteIndices = WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].CosmeticAttributes;
		for(int Index = 0; Index<4; Index++)
		{
			GetChild(2).GetChild(1).GetChild(0).GetChild(Index).GetChild(1).GetChild<Label>(2).Text = Convert.ToString(CosmeticSpriteIndices[Index]);
		}
		
		//Hair
		GetChild(2).GetChild(1).GetChild(1).GetChild(0).GetChild<TextEdit>(0).Text = Convert.ToString(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].HairRed);
		GetChild(2).GetChild(1).GetChild(1).GetChild(1).GetChild<TextEdit>(0).Text = Convert.ToString(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].HairGreen);
		GetChild(2).GetChild(1).GetChild(1).GetChild(2).GetChild<TextEdit>(0).Text = Convert.ToString(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].HairBlue);
		
		GetChild(2).GetChild(1).GetChild(1).GetChild(0).GetChild<ProgressBar>(1).SetValue(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].HairRed);
		GetChild(2).GetChild(1).GetChild(1).GetChild(1).GetChild<ProgressBar>(1).SetValue(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].HairGreen);
		GetChild(2).GetChild(1).GetChild(1).GetChild(2).GetChild<ProgressBar>(1).SetValue(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].HairBlue);
		
		//Name
		GetChild(2).GetChild(1).GetChild(2).GetChild<TextEdit>(0).Text = WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].Name;
		GetChild(3).GetChild<Label>(5).Text = WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].Name;
	}
	
	//CharCard Submenu
	private void UpdateCharCardSubmenu()
	{
		GetChild(2).GetChild(2).GetChild<CharacterCard>(1).CopyCharCard(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].CharCard);
		GetChild(2).GetChild(2).GetChild<CharacterCard>(1).Refresh();
	}
	
	public void UpdateCharacterCardList(List<CharacterCard> InputCharacterCardList)
	{
		if(InputCharacterCardList != null)
		{
			CharacterCardList = InputCharacterCardList;
			CharCardPageIndex = 0;
			CharCardMaxPage = CharacterCardList.Count/6;
			CharCardPageIndex = 0;
			CharCard_SetPage();
		}
	}
	
	private void CharCard_SetPage()
	{
		GetChild(2).GetChild(2).GetChild(0).GetChild(1).GetChild<Label>(2).Text = Convert.ToString(CharCardPageIndex);
		int Offset = CharCardPageIndex*6;
		for(int Index = 0; Index<6; Index++)
		{
			CharacterCard Leaf = GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetChild<CharacterCard>(Index);
			if(Offset+Index<CharacterCardList.Count)
			{
				Leaf.Show();
				Leaf.CopyCharCard(CharacterCardList[Offset+Index]);
				Leaf.Refresh();
			}
			else {Leaf.Hide();}
		}
	}
	
	//Deck submenu
	private void UpdateCharDeckSubmenu()//UNFINISHED
	{
		for(int Index = 0; Index<9; Index++)
		{
			CombatCard Leaf = GetChild(2).GetChild(3).GetChild(1).GetChild(0).GetChild<CombatCard>(Index);
			if(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].Deck[Index] != null)
			{
				Leaf.Show();
				Leaf.CopyCombatCard(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].Deck[Index]);
				Leaf.Refresh();
			}
			else {Leaf.Hide();}
		}
	}
	
	public void UpdateCombatCardList(List<CombatCard> InputCombatCardList)
	{
		CombatCardList = InputCombatCardList;
		CombatCardMaxPage = CombatCardList.Count/6;
		CombatCardPageIndex = 0;
		CombatCard_SetPage();
	}
	
	private void CombatCard_SetPage()
	{
		GetChild(2).GetChild(3).GetChild(0).GetChild(1).GetChild<Label>(2).Text = Convert.ToString(CombatCardPageIndex);
		int Offset = CombatCardPageIndex*6;
		for(int Index = 0; Index<6; Index++)
		{
			CombatCard Leaf = GetChild(2).GetChild(3).GetChild(0).GetChild(0).GetChild<CombatCard>(Index);
			if(Offset+Index<CombatCardList.Count)
			{
				Leaf.Show();
				Leaf.CopyCombatCard(CombatCardList[Offset+Index]);
				Leaf.Refresh();
			}
			else {Leaf.Hide();}
		}
	}
	
	
	//Connection Inputs
	private void _File_Selected(string SelectedFilePath)
	{
		FilePath = SelectedFilePath;
		DonTxtIndex = 0;
		GetChild(1).GetChild(2).GetChild(3).GetChild<Label>(2).Text = "0";
	}
	
	private void _DonTxtIndex_Pressed(bool LeftRight) //if leftright, decrease, else increase
	{
		if(LeftRight && DonTxtIndex>0) {DonTxtIndex--;}
		else if (!LeftRight && DonTxtIndex < DataStorageManager.GetNumberOfDonTxtEntries("BATTLE", DataStorageManager.ParseDonTxt(FilePath, "BATTLE"))-1) {DonTxtIndex++;}
		GetChild(1).GetChild(2).GetChild(3).GetChild<Label>(2).Text = Convert.ToString(DonTxtIndex);
	}
	
	private void _LoadBattle_Pressed()
	{
		if(DataStorageManager.GetNumberOfDonTxtEntries("BATTLE", DataStorageManager.ParseDonTxt(FilePath, "BATTLE")) > 0)
		{
			LoadBattle(DataStorageManager.GetDonTxtEntryFromIndex("BATTLE",DataStorageManager.ParseDonTxt(FilePath, "BATTLE"), DonTxtIndex));
		}
	}
	private void _SaveBattle_Pressed()
	{DataStorageManager.StoreDonTxt(FilePath, "BATTLE", WorkingBattle.ConvertToDonTxt(), "Name", WorkingBattle.Name);} 
	
	private void _BattleName_Changed()
	{
		string InputName = GetChild(1).GetChild(8).GetChild<TextEdit>(0).Text;
		if(InputName.Contains('=') || InputName.Contains('@') || InputName.Contains('<') || InputName.Contains('>') || InputName.Contains(':') || InputName.Contains('#') || InputName.Contains('[') || InputName.Contains(']') || InputName.Contains(','))
		{GetChild(1).GetChild(8).GetChild<TextEdit>(0).Text = WorkingBattle.Name;}
		else
		{WorkingBattle.Name = InputName;}
	}
	
	//Encounter and Char Related Inputs
	private void _SwitchEncounter_Pressed(bool IncDec) //if IncDec, increase, else decrease
	{
		if(IncDec && EncounterIndex < WorkingBattle.Encounters.Count-1) {EncounterIndex++; UpdateEncounter();}
		else if(!IncDec && EncounterIndex > 0) {EncounterIndex--; UpdateEncounter();}
	}
	private void _AddEncounter_Pressed()
	{
		WorkingBattle.Encounters.Add(new Encounter());
		EncounterIndex = WorkingBattle.Encounters.Count-1;
		UpdateEncounter();
	}
	private void _RemoveEncounter_Pressed()
	{
		if(WorkingBattle.Encounters.Count > 1) {WorkingBattle.Encounters.RemoveAt(EncounterIndex);}
		if(EncounterIndex >= WorkingBattle.Encounters.Count) {EncounterIndex = WorkingBattle.Encounters.Count-1;}
		UpdateEncounter();
	}
	
	private void _SwitchChar_Pressed(bool IncDec) //if IncDec, increase, else decrease
	{
		if(IncDec && CharacterIndex < 4) {CharacterIndex++; UpdateCharacter();}
		else if(!IncDec && CharacterIndex > 0) {CharacterIndex--; UpdateCharacter();}
	}
	
	//Switch Menu control
	private void _SwitchSubmenu_Pressed(bool IncDec)
	{
		if(IncDec && SwitchMenuIndex < 3) {SwitchMenuIndex++;}
		else if(!IncDec && SwitchMenuIndex > 0) {SwitchMenuIndex--;}
		DisplaySubmenu();
	}
	private void DisplaySubmenu()
	{
		GetChild(1).GetChild(3).GetChild<Label>(2).Text = Convert.ToString(SwitchMenuIndex);
		for(int Index = 0; Index<4; Index++)
		{
			if(Index == SwitchMenuIndex) {GetChild(2).GetChild<Node2D>(Index).Show();}
			else {GetChild(2).GetChild<Node2D>(Index).Hide();}
		}
	}
	
	
	//Battle Detail Submenu Inputs
	private void _IconFile_Selected(string SelectedFilePath)
	{
		WorkingBattle.ImgPath = SelectedFilePath;
		UpdateBattleEncounterSubmenu();
	}
	private void _EncounterBG_Selected(string SelectedFilePath)
	{
		WorkingBattle.Encounters[EncounterIndex].BackGroundImgPath = SelectedFilePath;
		UpdateBattleEncounterSubmenu();
	}
	
	//Char Cosmetic Submenu Inputs
	private void _CosmeticSpriteChange_Pressed(bool LeftRight, int Index)
	{
		if(LeftRight && CosmeticSpriteIndices[Index]>0) {CosmeticSpriteIndices[Index]--;}
		else if (!LeftRight && CosmeticSpriteIndices[Index] < 15) {CosmeticSpriteIndices[Index]++;}
		WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].CosmeticAttributes = CosmeticSpriteIndices;
		UpdateCharacter();
	}
	
	private void _HairColour_Changed(int Index)
	{
		if(Regex.IsMatch(GetChild(2).GetChild(1).GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text,"^[0-9]+$"))
		{
			if(Convert.ToInt32(GetChild(2).GetChild(1).GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text)>255)
			{GetChild(2).GetChild(1).GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text = "255";}
			
			switch(Index)
			{
				case 0:
					WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].SetHairColour(InputRed: Convert.ToInt32(GetChild(2).GetChild(1).GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text));
					GetChild(2).GetChild(1).GetChild(1).GetChild(Index).GetChild<ProgressBar>(1).SetValue(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].HairRed);
					break;
				case 1:
					WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].SetHairColour(InputGreen: Convert.ToInt32(GetChild(2).GetChild(1).GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text));
					GetChild(2).GetChild(1).GetChild(1).GetChild(Index).GetChild<ProgressBar>(1).Value = WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].HairGreen;
					break;
				case 2:
					WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].SetHairColour(InputBlue: Convert.ToInt32(GetChild(2).GetChild(1).GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text));
					GetChild(2).GetChild(1).GetChild(1).GetChild(Index).GetChild<ProgressBar>(1).Value = WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].HairBlue;
					break;
			}
		}
		else
		{
			switch(Index)
			{
				case 0: GetChild(2).GetChild(1).GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text = Convert.ToString(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].HairRed); break;
				case 1: GetChild(2).GetChild(1).GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text = Convert.ToString(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].HairGreen); break;
				case 2: GetChild(2).GetChild(1).GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text = Convert.ToString(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].HairBlue); break;
			}
		}
		GetChild<Character>(3).CopyCharacter(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex]);
		GetChild<Character>(3).Refresh();
	}
	private void _CharName_Changed()
	{
		string InputName = GetChild(2).GetChild(1).GetChild(2).GetChild<TextEdit>(0).Text;
		if(InputName.Contains('=') || InputName.Contains('@') || InputName.Contains('<') || InputName.Contains('>') || InputName.Contains(':') || InputName.Contains('#') || InputName.Contains('[') || InputName.Contains(']') || InputName.Contains(','))
		{
			GetChild(2).GetChild(1).GetChild(2).GetChild<TextEdit>(0).Text = WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].Name;
		}
		else
		{
			WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].Name = InputName;
			GetChild(3).GetChild<Label>(5).Text = InputName;
		}
	}
	
	//CharCard Selection Submenu Inputs
	private void _CharCard_Card_Selected(int Index)
	{
		Index = (6*CharCardPageIndex)+Index;
		if(Index<CharacterCardList.Count)
		{
			if(CharacterCardList[Index] != null)
			{
				WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].CharCard = CharacterCardList[Index];
				UpdateCharacter();
			}
		}
	}
	
	private void _CharCard_PageChange_Pressed(bool LeftRight)
	{
		if(LeftRight && CharCardPageIndex>0) {CharCardPageIndex--;}
		else if (!LeftRight && CharCardPageIndex < CharCardMaxPage) {CharCardPageIndex++;}
		CharCard_SetPage();
	}
	
	//Deck Selection Submenu Inputs
	private void _CombatCard_Card_Selected(int Index)
	{
		Index = (6*CombatCardPageIndex)+Index;
		for(int DeckIndex = 0; DeckIndex<9; DeckIndex++)
		{
			if(WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].Deck[DeckIndex] == null)
			{
				WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].Deck[DeckIndex] = CombatCardList[Index];
				break;
			}
		}
		UpdateCharDeckSubmenu();
	}
	
	private void _CombatCard_Deck_Card_Removed(int Index)
	{
		while(Index<8) {WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].Deck[Index] = WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].Deck[Index+1]; Index++;}
		WorkingBattle.Encounters[EncounterIndex].Enemies[CharacterIndex].Deck[Index] = null;
		UpdateCharDeckSubmenu();
	}
	
	private void _CombatCard_PageChange_Pressed(bool LeftRight)
	{
		if(LeftRight && CombatCardPageIndex>0) {CombatCardPageIndex--;}
		else if (!LeftRight && CombatCardPageIndex < CombatCardMaxPage) {CombatCardPageIndex++;}
		CombatCard_SetPage();
	}
}
