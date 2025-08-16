using Godot;
using System;
using System.Collections.Generic;

public partial class CombatPageSelect : Node2D
{
	int PageNum = 0;
	int MaxPage = 0;
	int Filter = 0;
	string SearchTerm = "";
	CombatCard[] Deck = new CombatCard[9];
	public List<CombatCard> FullCombatCardList = new List<CombatCard>();
	List<CombatCard> FilteredCombatCardList = new List<CombatCard>();
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetChild(2).GetChild(0).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[0];
		GetChild(2).GetChild(1).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[1];
		GetChild(2).GetChild(2).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[2];
		GetChild(2).GetChild(3).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[3];
		GetChild(2).GetChild(4).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[4];
		
		UpdateFilter();
		UpdateDeck();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//CIF
	public void Refresh(List<CombatCard> CombatCardList)
	{
		FullCombatCardList = CombatCardList;
		Filter = 0;
		PageNum = 0;
		UpdateFilter();
	}
	
	public void UpdateFilter()
	{
		FilteredCombatCardList.Clear();
		for(int Index = 0; Index<FullCombatCardList.Count; Index++)
		{
			string TargetCardName = FullCombatCardList[Index].Name;
			if((FullCombatCardList[Index].StrengthGrade == Filter || Filter == 0) && TargetCardName.ToLower().Contains(SearchTerm.ToLower())) //WHY ARE YOU NOT WORKING??
			{FilteredCombatCardList.Add(FullCombatCardList[Index]);}
		}
		MaxPage = FilteredCombatCardList.Count/9;
		PageNum = 0;
		SetPage();
	}
	
	public void SetPage()
	{
		GetChild(1).GetChild(1).GetChild<Label>(2).Text = Convert.ToString(PageNum);
		int Offset = PageNum*9;
		for(int Index = 0; Index<9; Index++)
		{
			CombatCard Leaf = GetChild(1).GetChild(0).GetChild<CombatCard>(Index);
			if(Offset+Index<FilteredCombatCardList.Count)
			{
				Leaf.Show();
				Leaf.CopyCombatCard(FilteredCombatCardList[Offset+Index]);
				Leaf.Refresh();
			}
			else {Leaf.Hide();}
		}
	}
	
	public void UpdateDeck()
	{
		Deck = GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].Deck; 
		int TotalStaminaCost = 0;
		int MaxStrengthGrade = 0;
		
		for(int Index = 0; Index<9; Index++)
		{
			CombatCard Leaf = GetChild(4).GetChild(0).GetChild<CombatCard>(Index);
			if(Deck[Index] != null)
			{
				Leaf.Show();
				Leaf.CopyCombatCard(Deck[Index]);
				Leaf.Refresh();
				
				TotalStaminaCost += Deck[Index].StaminaCost;
				if(Deck[Index].StrengthGrade > MaxStrengthGrade)
				{MaxStrengthGrade = Deck[Index].StrengthGrade;}
			}
			else {Leaf.Hide();}
		}
		GetChild(5).GetChild<Label>(0).Text = "Total Stamina\nCost: "+Convert.ToString(TotalStaminaCost);
		switch(MaxStrengthGrade)
		{
			case 1:
				GetChild(5).GetChild<Label>(1).Text = "Highest Strength\nGrade: Naraka (1)"; break;
			case 2:
				GetChild(5).GetChild<Label>(1).Text = "Highest Strength\nGrade: Preta (2)"; break;
			case 3:
				GetChild(5).GetChild<Label>(1).Text = "Highest Strength\nGrade: Tiryagyoni (3)"; break;
			case 4:
				GetChild(5).GetChild<Label>(1).Text = "Highest Strength\nGrade: Asura (4)"; break;
			case 5:
				GetChild(5).GetChild<Label>(1).Text = "Highest Strength\nGrade: Manushya (5)"; break;
			default:
				GetChild(5).GetChild<Label>(1).Text = "Highest Strength\nGrade: Deva ("+Convert.ToString(MaxStrengthGrade)+")"; break;
		}
	}
	
	// Connection Inputs
	private void _Card_Selected(int Index)
	{
		CombatCard[] Deck = GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].Deck;
		Index = (9*PageNum)+Index;
		for(int DeckIndex = 0; DeckIndex<9; DeckIndex++)
		{
			if(Deck[DeckIndex] == null)
			{
				Deck[DeckIndex] = FilteredCombatCardList[Index];
				break;
			}
		}
		UpdateDeck();
	}
	
	private void _Deck_Card_Removed(int Index)
	{
		CombatCard[] Deck = GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].Deck;
		while(Index<8) {Deck[Index] = Deck[Index+1]; Index++;}
		Deck[Index] = null;
		UpdateDeck();
	}
	
	private void _ClearDeck()
	{
		GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].Deck = new CombatCard[9];
		UpdateDeck();
	}
	
	private void _GradeFilter_Pressed(int Grade)
	{
		if(Grade != Filter) {Filter = Grade;}
		else {Filter = 0;}
		for(int Index = 0; Index<5; Index++)
		{
			if(Index+1 == Filter) {GetChild(2).GetChild(Index).GetChild<Node2D>(2).Show();}
			else {GetChild(2).GetChild(Index).GetChild<Node2D>(2).Hide();}
		}
		UpdateFilter();
	}
	
	private void _PageChange_Pressed(bool LeftRight)
	{
		if(LeftRight && PageNum>0) {PageNum--;}
		else if (!LeftRight && PageNum < MaxPage) {PageNum++;}
		SetPage();
	}
	
	private void _SearchInput_Changed()
	{
		if(SearchTerm.ToLower() != GetChild(3).GetChild<TextEdit>(1).Text.ToLower())
		{SearchTerm = GetChild(3).GetChild<TextEdit>(1).Text.ToLower(); UpdateFilter();}
	}
}
