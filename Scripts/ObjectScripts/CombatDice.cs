using Godot;
using System;

public partial class CombatDice : Node2D
{
	public int MinRoll = 1;
	public int MaxRoll = 1;
	public int DiceType = 0; //Slash, Pierce, Blunt, Block, Evade (+ special ones: Red, White, Black, and Pale)
	public string ConditionalTrigger = "";
	public string[] Condition = new string[4]; //EXPLAIN FORMAT 0 - __, 1 - __, 2 - __, 3 - __
	public string[] Effect = new string[4]; //EXPLAIN FORMAT 0 - __, 1 - __, 2 - __, 3 - __
	public string ConditionalDescOverride = ""; //Manually written description for condition, can only be set by directly modifying the .txt file
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	// CIF
	public void Refresh()
	{
		if(DiceType < 9) {GetChild<Sprite2D>(1).Texture = ((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).DiceTextures[DiceType];}
		else {GetChild<Sprite2D>(1).Texture = null;}
		
		GetChild<Label>(2).Text = Convert.ToString(MinRoll)+"~"+Convert.ToString(MaxRoll);
		
		if(ConditionalDescOverride == "" || ConditionalDescOverride == " ") {GetChild<Label>(3).Text = InformationConversionUtils.ConditionalToText(ConditionalTrigger, Condition, Effect);}
		else {GetChild<Label>(3).Text = ConditionalDescOverride;}
	}
	public void CopyDice(CombatDice InputDice)
	{
		MinRoll = InputDice.MinRoll; MaxRoll = InputDice.MaxRoll; DiceType = InputDice.DiceType;
		ConditionalTrigger = InputDice.ConditionalTrigger; Condition = InputDice.Condition; Effect = InputDice.Effect;
		ConditionalDescOverride = InputDice.ConditionalDescOverride;
	}
	
	//Stuff for storing and loading data
	public string ConvertToDonTxt()
	{
		string[,] PropertiesData = new string[7,2];
		
		PropertiesData[0,0] = "MinRoll"; PropertiesData[1,0] = "MaxRoll"; PropertiesData[2,0] = "DiceType";
		PropertiesData[3,0] = "ConditionalTrigger"; PropertiesData[4,0] = "Condition"; PropertiesData[5,0] = "Effect";
		PropertiesData[6,0] = "ConditionalDescOverride";
		
		PropertiesData[0,1] = Convert.ToString(MinRoll); PropertiesData[1,1] = Convert.ToString(MaxRoll);
		PropertiesData[2,1] = Convert.ToString(DiceType); PropertiesData[3,1] = ConditionalTrigger;
		PropertiesData[4,1] = "["+Condition[0]+","+Condition[1]+","+Condition[2]+","+Condition[3]+"]";
		PropertiesData[5,1] = "["+Effect[0]+","+Effect[1]+","+Effect[2]+","+Effect[3]+"]";
		PropertiesData[6,1] = ConditionalDescOverride;
		
		string DonTxtForm = DataStorageManager.ConvertToDonTxt("COMBATDICE", PropertiesData);
		
		return DonTxtForm;
	}
	public void SetFromPropertiesData(string[,] PropertiesData)
	{
		for(int Index = 0; Index<PropertiesData.GetLength(0); Index++)
		{
			switch(PropertiesData[Index,0])
			{
				case "MinRoll": MinRoll = Convert.ToInt32(PropertiesData[Index,1]); break;
				case "MaxRoll": MaxRoll = Convert.ToInt32(PropertiesData[Index,1]); break;
				case "DiceType": DiceType = Convert.ToInt32(PropertiesData[Index,1]); break;
				case "ConditionalTrigger": ConditionalTrigger = PropertiesData[Index,1]; break;
				case "Condition": Condition = StringToArray(PropertiesData[Index,1]); break;
				case "Effect": Effect = StringToArray(PropertiesData[Index,1]); break;
				case "ConditionalDescOverride": ConditionalDescOverride = PropertiesData[Index,1]; break;
			}
		}
	}
	
	private string[] StringToArray(string InputString)
	{
		string[] OutputArray = new string[4];
		string WorkingString = "";
		int Index = 0;
		foreach(char ConvChar in InputString)
		{
			if(ConvChar == '[') {continue;}
			if(ConvChar == ',' || ConvChar == ']') {OutputArray[Index] = WorkingString; Index++; WorkingString = ""; continue;}
			WorkingString += ConvChar;
		}
		return OutputArray;
	}
}
