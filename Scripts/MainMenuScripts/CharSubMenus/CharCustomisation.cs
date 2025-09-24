using Godot;
using System;
using System.Text.RegularExpressions;

public partial class CharCustomisation : Node2D
{
	int[] CosmeticSpriteIndices = [0,0,0,0];
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	// CIF
	public void UpdateCharacter()
	{
		Character SelectedChar = GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter];
		CosmeticSpriteIndices = SelectedChar.CosmeticAttributes;
		for(int Index = 0; Index<4; Index++)
		{
			GetChild(0).GetChild(Index).GetChild(1).GetChild<Label>(2).Text = Convert.ToString(CosmeticSpriteIndices[Index]);
		}
		GetChild(1).GetChild(0).GetChild<TextEdit>(0).Text = Convert.ToString(SelectedChar.HairRed);
		GetChild(1).GetChild(0).GetChild<ProgressBar>(1).Value = SelectedChar.HairRed;
		GetChild(1).GetChild(1).GetChild<TextEdit>(0).Text = Convert.ToString(SelectedChar.HairGreen);
		GetChild(1).GetChild(1).GetChild<ProgressBar>(1).Value = SelectedChar.HairGreen;
		GetChild(1).GetChild(2).GetChild<TextEdit>(0).Text = Convert.ToString(SelectedChar.HairBlue);
		GetChild(1).GetChild(2).GetChild<ProgressBar>(1).Value = SelectedChar.HairBlue;
		GetChild(2).GetChild<TextEdit>(0).Text = SelectedChar.Name;
	}
	
	private void UpdateCosmeticSprites()
	{
		GetParent<CharacterMenu>().UpdatePlayerTeam();
		GetParent<CharacterMenu>().UpdateSelectedChar();
		
		for(int Index = 0; Index<4; Index++)
		{
			GetChild(0).GetChild(Index).GetChild(1).GetChild<Label>(2).Text = Convert.ToString(CosmeticSpriteIndices[Index]);
		}
	}
	
	
	// Connection Inputs
	private void _CosmeticSpriteChange_Pressed(bool LeftRight, int Index)
	{
		if(LeftRight && CosmeticSpriteIndices[Index]>0) {CosmeticSpriteIndices[Index]--;}
		else if (!LeftRight && CosmeticSpriteIndices[Index] < 15) {CosmeticSpriteIndices[Index]++;}
		GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].CosmeticAttributes = CosmeticSpriteIndices;
		UpdateCosmeticSprites();
	}
	
	private void _HairColour_Changed(int Index)
	{
		if(Regex.IsMatch(GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text,"^[0-9]+$"))
		{
			if(Convert.ToInt32(GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text)>255)
			{GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text = "255";}
			
			switch(Index)
			{
				case 0:
					GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].SetHairColour(InputRed: Convert.ToInt32(GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text));
					GetChild(1).GetChild(Index).GetChild<ProgressBar>(1).SetValue(GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].HairRed);
					break;
				case 1:
					GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].SetHairColour(InputGreen: Convert.ToInt32(GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text));
					GetChild(1).GetChild(Index).GetChild<ProgressBar>(1).Value = GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].HairGreen;
					break;
				case 2:
					GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].SetHairColour(InputBlue: Convert.ToInt32(GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text));
					GetChild(1).GetChild(Index).GetChild<ProgressBar>(1).Value = GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].HairBlue;
					break;
			}
		}
		else
		{
			switch(Index)
			{
				case 0: GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text = Convert.ToString(GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].HairRed); break;
				case 1: GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text = Convert.ToString(GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].HairGreen); break;
				case 2: GetChild(1).GetChild(Index).GetChild<TextEdit>(0).Text = Convert.ToString(GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].HairBlue); break;
			}
		}
	}
	private void _Name_Changed()
	{
		string InputName = GetChild(2).GetChild<TextEdit>(0).Text;
		if(InputName.Contains('=') || InputName.Contains('@') || InputName.Contains('<') || InputName.Contains('>') || InputName.Contains(':') || InputName.Contains('#') || InputName.Contains('[') || InputName.Contains(']') || InputName.Contains(','))
		{
			GetChild(2).GetChild<TextEdit>(0).Text = GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].Name;
		}
		else
		{
			GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[GetParent<CharacterMenu>().SelectedCharacter].Name = InputName;
			try{DisplayChar.GetChild<Label>(5).Text = GetParent().GetParent().GetParent().GetParent<GameManager>().PlayerTeam[SelectedCharacter].Name;} catch{}
		}
	}
}
