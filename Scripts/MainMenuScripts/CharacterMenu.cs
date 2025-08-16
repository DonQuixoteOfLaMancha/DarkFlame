using Godot;
using System;

public partial class CharacterMenu : Node2D
{
	public int SelectedCharacter = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_Character_Selected(0);
		_SubMenuButton_Pressed(0);
		UpdatePlayerTeam();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	// CIF
	public void UpdatePlayerTeam()
	{
		for(int Index = 0; Index<5; Index++)
		{
			if(Index == SelectedCharacter) {continue;}
			GetChild(0).GetChild(2).GetChild(0).GetChild<Character>(Index).CopyCharacter(GetParent().GetParent().GetParent<GameManager>().PlayerTeam[Index]);
			GetChild(0).GetChild(2).GetChild(0).GetChild<Character>(Index).Refresh();
		}
		UpdateSelectedChar();
	}
	public void UpdateSelectedChar()
	{
		GetChild(0).GetChild(2).GetChild(0).GetChild<Character>(SelectedCharacter).CopyCharacter(GetParent().GetParent().GetParent<GameManager>().PlayerTeam[SelectedCharacter]);
		GetChild(0).GetChild(2).GetChild(0).GetChild<Character>(SelectedCharacter).Refresh();
		Character DisplayChar = GetChild(0).GetChild<Character>(3);
		DisplayChar.CopyCharacter(GetParent().GetParent().GetParent<GameManager>().PlayerTeam[SelectedCharacter]);
		DisplayChar.Refresh();
		GetChild<CombatPageSelect>(1).UpdateDeck();
		GetChild<CharPageSelect>(2).UpdateSelected();
		GetChild<CharCustomisation>(3).UpdateCharacter();;
		try{DisplayChar.GetChild<Label>(5).Text = GetParent().GetParent().GetParent<GameManager>().PlayerTeam[SelectedCharacter].Name;} catch{}
	}
	
	// Connection Inputs
	private void _SubMenuButton_Pressed(int SubMenu)
	{
		for(int Index = 0; Index<3; Index++)
		{if(Index == SubMenu) {GetChild<Node2D>(Index+1).Show(); GetChild(0).GetChild(1).GetChild(Index).GetChild<Node2D>(2).Show();}
						else {GetChild<Node2D>(Index+1).Hide(); GetChild(0).GetChild(1).GetChild(Index).GetChild<Node2D>(2).Hide();}}
	}
	
	private void _Character_Selected(int CharacterIndex)
	{
		SelectedCharacter = CharacterIndex;
		UpdateSelectedChar();
	}
	
}
