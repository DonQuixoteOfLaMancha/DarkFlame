using Godot;
using System;
using System.Collections.Generic;

public partial class CombatCard : Node2D
{
	public string Name = "Gut Harvesting";
	private string cardartpath;
	public string CardArtPath
	{
		get {return cardartpath;}
		set
		{
			if(value != cardartpath)
			{
				cardartpath = value;
				try{CardArtImage = ResourceLoadUtils.LoadTexture2DFromImage(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImages[((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(value))], ((SceneTree)Engine.GetMainLoop()).Root);}
				catch{CardArtImage = ResourceLoadUtils.LoadTexture2D(value, null);}
			}
		}
	}
	Texture2D CardArtImage;
	
	public int StrengthGrade = 1;
	
	public int StaminaCost = 1;
	public string ConditionalTrigger = "";
	public string[] Condition = new string[4]; //EXPLAIN FORMAT 0 - __, 1 - __, 2 - __, 3 - __
	public string[] Effect = new string[4]; //EXPLAIN FORMAT 0 - __, 1 - __, 2 - __, 3 - __
	public string ConditionalDescOverride = ""; //Manually written description for condition, can only be set by directly modifying the .txt file
	
	public List<CombatDice> Dice = new List<CombatDice>(1){new CombatDice()};
	
	bool NewDice = false;
	
	public CombatCard()
	{
		CardArtPath = "CombatCardDefaultImagePath";
		Dice[0].MaxRoll = 2;
	}
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Refresh();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(NewDice)
		{
			for(int Index = 0; Index<Dice.Count; Index++)
			{
				CombatDice TargetDice = GetChild<CombatDice>(GetChildCount()-Dice.Count+Index);
				TargetDice.CopyDice(Dice[Index]);
				TargetDice.Refresh();
				TargetDice.Position = new Vector2(5,140+30*Index);
			}
			NewDice = false;
		}
	}
	
	// CIF
	public void CopyCombatCard(CombatCard InputCard)
	{
		Name = InputCard.Name;
		cardartpath = InputCard.CardArtPath;
		CardArtImage = InputCard.CardArtImage;
		StrengthGrade = InputCard.StrengthGrade;
		StaminaCost = InputCard.StaminaCost;
		ConditionalTrigger = InputCard.ConditionalTrigger;
		Condition = InputCard.Condition;
		Effect = InputCard.Effect;
		ConditionalDescOverride = InputCard.ConditionalDescOverride;
		Dice = InputCard.Dice;
	}
	
	public void Refresh()
	{
		GetChild<Label>(5).Text = Name;
		GetChild<Sprite2D>(4).Texture = CardArtImage;
		if(GetChild<Sprite2D>(4).Texture != null)
		{GetChild<Node2D>(4).Scale = new Vector2((float)200/GetChild<Sprite2D>(4).Texture.GetWidth(),(float)100/GetChild<Sprite2D>(4).Texture.GetHeight());}
		
		GetChild<Label>(3).Text = Convert.ToString(StaminaCost);
		if(ConditionalDescOverride == "" || ConditionalDescOverride == " ") {GetChild<Label>(6).Text = InformationConversionUtils.ConditionalToText(ConditionalTrigger, Condition, Effect);}
		else {GetChild<Label>(6).Text = ConditionalDescOverride;}
		
		GetChild<Label>(7).Hide();
		switch(StrengthGrade)
		{
			case 1: GetChild<Polygon2D>(0).Color = new Color("2e8799"); break;
			case 2: GetChild<Polygon2D>(0).Color = new Color("660a0a"); break;
			case 3: GetChild<Polygon2D>(0).Color = new Color("195e80"); break;
			case 4: GetChild<Polygon2D>(0).Color = new Color("080a99"); break;
			case 5: GetChild<Polygon2D>(0).Color = new Color("997a1f"); break;
			default:
				GetChild<Polygon2D>(0).Color = new Color("cccccc");
				string Num = Convert.ToString(StrengthGrade);
				GetChild<Label>(7).Text = $"{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}";
				GetChild<Label>(7).Show();
				break;
		}
		
		for(int Index = 9; Index<GetChildCount(); Index++) {GetChild(Index).QueueFree();}
		var DiceScene = GD.Load<PackedScene>("res://Scenes/ObjectScenes/CombatDice.tscn");
		for(int Index = 0; Index<Dice.Count; Index++)
		{
			var DiceInstance = DiceScene.Instantiate();
			AddChild(DiceInstance);
		}
		NewDice = true;
	}
	
	//Stuff for storing and loading data
	public string ConvertToDonTxt()
	{
		string[,] PropertiesData = new string[9,2];
		
		PropertiesData[0,0] = "Name"; PropertiesData[1,0] = "CardArtPath"; PropertiesData[2,0] = "StrengthGrade";
		PropertiesData[3,0] = "StaminaCost"; PropertiesData[4,0] = "ConditionalTrigger";
		PropertiesData[5,0] = "Condition"; PropertiesData[6,0] = "Effect"; PropertiesData[7,0] = "ConditionalDescOverride";
		PropertiesData[8,0] = "Dice";
		
		PropertiesData[0,1] = Name; PropertiesData[1,1] = CardArtPath; PropertiesData[2,1] = Convert.ToString(StrengthGrade);
		PropertiesData[3,1] = Convert.ToString(StaminaCost); PropertiesData[4,1] = ConditionalTrigger;
		PropertiesData[5,1] = "["+Condition[0]+","+Condition[1]+","+Condition[2]+","+Condition[3]+"]";
		PropertiesData[6,1] = "["+Effect[0]+","+Effect[1]+","+Effect[2]+","+Effect[3]+"]";
		PropertiesData[7,1] = ConditionalDescOverride;
		string DiceTxt = "[";
		foreach(CombatDice ConvDice in Dice)
		{DiceTxt += ConvDice.ConvertToDonTxt()+",";}
		string NewDiceTxt = DiceTxt.Substring(0,DiceTxt.Length-1);
		NewDiceTxt += "]";
		PropertiesData[8,1] = NewDiceTxt;
		
		string DonTxtForm = DataStorageManager.ConvertToDonTxt("COMBATCARD", PropertiesData);
		
		return DonTxtForm;
	}
	public void SetFromPropertiesData(string[,] PropertiesData)
	{
		for(int Index = 0; Index<PropertiesData.GetLength(0); Index++)
		{
			switch(PropertiesData[Index,0])
			{
				case "Name": Name = PropertiesData[Index,1]; break;
				case "CardArtPath": CardArtPath = PropertiesData[Index,1]; break;
				case "StrengthGrade": StrengthGrade = Convert.ToInt32(PropertiesData[Index,1]); break;
				case "StaminaCost": StaminaCost = Convert.ToInt32(PropertiesData[Index,1]); break;
				case "ConditionalTrigger": ConditionalTrigger = PropertiesData[Index,1]; break;
				case "Condition": Condition = StringToArray(PropertiesData[Index,1]); break;
				case "Effect": Effect = StringToArray(PropertiesData[Index,1]); break;
				case "ConditionalDescOverride": ConditionalDescOverride = PropertiesData[Index,1]; break;
				case "Dice": Dice = StringListToDiceList(StringToList(PropertiesData[Index,1])); StringToList(PropertiesData[Index,1]); break;
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
	private List<string> StringToList(string InputString)
	{
		List<string> OutputList = new List<string>();
		string WorkingString = "";
		int Depth = -1;
		foreach(char ConvChar in InputString)
		{
			if(ConvChar == '[') {Depth++; if(Depth==0){continue;}}
			if(ConvChar == ']') {Depth--;}
			if((ConvChar == ',' && Depth == 0) || Depth == -1) {OutputList.Add(WorkingString); WorkingString = ""; continue;}
			WorkingString += ConvChar;
		}

		return OutputList;
	}
	private List<CombatDice> StringListToDiceList(List<string> InputList)
	{
		List<CombatDice> OutputList = new List<CombatDice>();
		foreach(string ConvString in InputList)
		{
			CombatDice WorkingDice = new CombatDice();
			WorkingDice.SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(ConvString));
			OutputList.Add(WorkingDice);
		}
		
		return OutputList;
	}
}
