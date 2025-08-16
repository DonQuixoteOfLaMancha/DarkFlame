using Godot;
using System;
using System.Collections.Generic;

public partial class CharPageSelect : Node2D
{
	int PageNum = 0;
	int MaxPage = 0;
	int Filter = 0;
	string SearchTerm = "";
	public List<CharacterCard> FullCharacterCardList = new List<CharacterCard>();
	List<CharacterCard> FilteredCharacterCardList = new List<CharacterCard>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetChild(2).GetChild(0).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[0];
		GetChild(2).GetChild(1).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[1];
		GetChild(2).GetChild(2).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[2];
		GetChild(2).GetChild(3).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[3];
		GetChild(2).GetChild(4).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[4];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	// CIF
	public void Refresh(List<CharacterCard> CharacterCardList)
	{
		FullCharacterCardList = CharacterCardList;
		Filter = 0;
		PageNum = 0;
		UpdateFilter();
	}
	
	public void UpdateFilter()
	{
		FilteredCharacterCardList.Clear();
		for(int Index = 0; Index<FullCharacterCardList.Count; Index++)
		{
			string TargetCardName = FullCharacterCardList[Index].Name;
			if((FullCharacterCardList[Index].StrengthGrade == Filter || Filter == 0) && TargetCardName.ToLower().Contains(SearchTerm.ToLower())) 
			{FilteredCharacterCardList.Add(FullCharacterCardList[Index]);}
		}
		MaxPage = FilteredCharacterCardList.Count/9;
		PageNum = 0;
		SetPage();
	}
	
	public void SetPage()
	{
		GetChild(1).GetChild(1).GetChild<Label>(2).Text = Convert.ToString(PageNum);
		int Offset = PageNum*9;
		for(int Index = 0; Index<9; Index++)
		{
			CharacterCard Leaf = GetChild(1).GetChild(0).GetChild<CharacterCard>(Index);
			if(Offset+Index<FilteredCharacterCardList.Count)
			{
				Leaf.Show();
				Leaf.CopyCharCard(FilteredCharacterCardList[Offset+Index]);
				Leaf.Refresh();
			}
			else {Leaf.Hide();}
		}
	}
	
	public void UpdateSelected()
	{
		GetChild<CharacterCard>(4).CopyCharCard(GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].CharCard);
		GetChild<CharacterCard>(4).Refresh();
		GetChild<PassiveContainer>(5).UpdatePassives(GetChild<CharacterCard>(4).PassiveList);
	}
	
	// Connection Inputs
	private void _Card_Selected(int Index)
	{
		Index = (9*PageNum)+Index;
		if(FilteredCharacterCardList[Index] != null)
		{
			GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].CharCard = FilteredCharacterCardList[Index];
		}
		GetParent<CharacterMenu>().UpdateSelectedChar();
		UpdateSelected();
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
