using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterCard : Node2D
{
	public string Name = "Washed-Up Grade 9 Fixer";
	
	private string spritesheetsource;
	public string SpriteSheetSource
	{
		get {return spritesheetsource;}
		set
		{
			if(value != spritesheetsource)
			{
				spritesheetsource = value;
				if(((SceneTree)Engine.GetMainLoop()).Root.GetChild(0).GetChildCount() > 1)
				{
					if(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(value)) != -1)
					{SpriteIdleImage = ResourceLoadUtils.LoadTexture2DFromImage(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImages[((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(value))], ((SceneTree)Engine.GetMainLoop()).Root, XSize: 1792, YSize: 1792);}
					else{SpriteIdleImage = ResourceLoadUtils.LoadTexture2D(value, null, XSize: 1792, YSize: 1792);}
					if(GetChildCount() > 0) {Refresh();}
				}
			}
		}
	}
	public Texture2D SpriteIdleImage;
	
	//First index corresponds to sprite state, second index corresponds to coords
	//Sprite states go from 0-7: Idle, Moving, Damage, Slash, Pierce, Blunt, Block, Evade, Dead
	//If the coordinates are (1793,1793), the relevant sprite will not be displayed
	public int[,] FrontHairOrigins = new int[9,2] {{258,160}, {202,228}, {490,188}, {0,0}, {0,0}, {0,0}, {0,0}, {0,0}, {1792,1792}};
	public int[,] BackHairOrigins = new int[9,2] {{258,160}, {202,228}, {490,188}, {0,0}, {0,0}, {0,0}, {0,0}, {0,0}, {1792,1792}};
	public int[,] MouthOrigins = new int [9,2] {{520,510}, {464,578}, {752,538}, {0,0}, {0,0}, {0,0}, {0,0}, {0,0}, {1792,1792}};
	public int[,] EyeOrigins = new int[9,2] {{500,430}, {444,498}, {732,458}, {0,0}, {0,0}, {0,0}, {0,0}, {0,0}, {1792,1792}};
	
	public int Health = 30; public int StaggerHealth = 15;
	public double SlashRes = 1.0; public double PierceRes = 1.5; public double BluntRes = 2.0;
	public double SlashStagRes = 1.0; public double PierceStagRes = 1.5; public double BluntStagRes = 2.0;
	public int MinSpeed = 1; public int MaxSpeed = 4;
	public List<Passive> PassiveList = new List<Passive>();
	
	public int StrengthGrade = 1;
	
	public CharacterCard()
	{
		SpriteSheetSource = "user://Content/DefaultContent/Images/CharacterCardSprites/Grade9Fixer.png";
	}
	
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//CIF
	public void Refresh()
	{
		//Resistances Section
		GetChild(5).GetChild<Label>(0).Text = Convert.ToString(SlashRes);
		if(GetChild(5).GetChild<Label>(0).Text.Length == 1) {GetChild(5).GetChild<Label>(0).Text += ".0";}
		else if (GetChild(5).GetChild<Label>(0).Text.Length > 3 && SlashRes >= 0) {GetChild(5).GetChild<Label>(0).Text = GetChild(5).GetChild<Label>(0).Text.Substring(0,3);}
		else if (GetChild(5).GetChild<Label>(0).Text.Length > 4) {GetChild(5).GetChild<Label>(0).Text = GetChild(5).GetChild<Label>(0).Text.Substring(0,4);}
		GetChild(5).GetChild<Label>(1).Text = Convert.ToString(PierceRes);
		if(GetChild(5).GetChild<Label>(1).Text.Length == 1) {GetChild(5).GetChild<Label>(1).Text += ".0";}
		else if (GetChild(5).GetChild<Label>(1).Text.Length > 3 && PierceRes >= 0) {GetChild(5).GetChild<Label>(1).Text = GetChild(5).GetChild<Label>(1).Text.Substring(0,3);}
		else if (GetChild(5).GetChild<Label>(1).Text.Length > 4) {GetChild(5).GetChild<Label>(1).Text = GetChild(5).GetChild<Label>(1).Text.Substring(0,4);}
		GetChild(5).GetChild<Label>(2).Text = Convert.ToString(BluntRes);
		if(GetChild(5).GetChild<Label>(2).Text.Length == 1) {GetChild(5).GetChild<Label>(2).Text += ".0";}
		else if (GetChild(5).GetChild<Label>(2).Text.Length > 3 && BluntRes >= 0) {GetChild(5).GetChild<Label>(2).Text = GetChild(5).GetChild<Label>(2).Text.Substring(0,3);}
		else if (GetChild(5).GetChild<Label>(2).Text.Length > 4) {GetChild(5).GetChild<Label>(2).Text = GetChild(5).GetChild<Label>(2).Text.Substring(0,4);}
		GetChild(5).GetChild<Label>(3).Text = Convert.ToString(SlashStagRes); 
		if(GetChild(5).GetChild<Label>(3).Text.Length == 1) {GetChild(5).GetChild<Label>(3).Text += ".0";}
		else if (GetChild(5).GetChild<Label>(3).Text.Length > 3 && SlashStagRes >= 0) {GetChild(5).GetChild<Label>(3).Text = GetChild(5).GetChild<Label>(3).Text.Substring(0,3);}
		else if (GetChild(5).GetChild<Label>(3).Text.Length > 4) {GetChild(5).GetChild<Label>(3).Text = GetChild(5).GetChild<Label>(3).Text.Substring(0,4);}
		GetChild(5).GetChild<Label>(4).Text = Convert.ToString(PierceStagRes);
		if(GetChild(5).GetChild<Label>(4).Text.Length == 1) {GetChild(5).GetChild<Label>(4).Text += ".0";}
		else if (GetChild(5).GetChild<Label>(4).Text.Length > 3 && PierceStagRes >= 0) {GetChild(5).GetChild<Label>(4).Text = GetChild(5).GetChild<Label>(4).Text.Substring(0,3);}
		else if (GetChild(5).GetChild<Label>(4).Text.Length > 4) {GetChild(5).GetChild<Label>(4).Text = GetChild(5).GetChild<Label>(4).Text.Substring(0,4);}
		GetChild(5).GetChild<Label>(5).Text = Convert.ToString(BluntStagRes);
		if(GetChild(5).GetChild<Label>(5).Text.Length == 1) {GetChild(5).GetChild<Label>(5).Text += ".0";}
		else if (GetChild(5).GetChild<Label>(5).Text.Length > 3 && BluntStagRes >= 0) {GetChild(5).GetChild<Label>(5).Text = GetChild(5).GetChild<Label>(5).Text.Substring(0,3);}
		else if (GetChild(5).GetChild<Label>(5).Text.Length > 4) {GetChild(5).GetChild<Label>(5).Text = GetChild(5).GetChild<Label>(5).Text.Substring(0,4);}
		
		//Info Section
		GetChild(6).GetChild<Label>(0).Text = Name;
		GetChild(6).GetChild<Label>(1).Text = "HP: "+Convert.ToString(Health);
		GetChild(6).GetChild<Label>(2).Text = "STAGGER: "+Convert.ToString(StaggerHealth);
		GetChild(6).GetChild<Label>(3).Text = Convert.ToString(MinSpeed)+"~"+Convert.ToString(MaxSpeed);
		
		//Strength Grade Background Colouring
		GetChild<Label>(2).Hide();
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
				GetChild<Label>(2).Text = $"{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}";
				GetChild<Label>(2).Show();
				break;
		}
		
		//Sprite Loading
		if(SpriteIdleImage == null && ((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(SpriteSheetSource)) != -1)
		{SpriteIdleImage = ResourceLoadUtils.LoadTexture2DFromImage(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImages[((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(SpriteSheetSource))], ((SceneTree)Engine.GetMainLoop()).Root, XSize: 1792, YSize: 1792);}
		else if(SpriteIdleImage == null) {SpriteIdleImage = ResourceLoadUtils.LoadTexture2D(SpriteSheetSource, ((SceneTree)Engine.GetMainLoop()).Root, XSize: 1792, YSize: 1792);}
		GetChild<Sprite2D>(3).Texture = SpriteIdleImage;
	}
	
	public void CopyCharCard(CharacterCard InputCard)
	{
		Name = InputCard.Name;
		SpriteSheetSource = InputCard.SpriteSheetSource;
		BackHairOrigins = InputCard.BackHairOrigins; FrontHairOrigins = InputCard.BackHairOrigins;
		EyeOrigins = InputCard.BackHairOrigins; MouthOrigins = InputCard.BackHairOrigins;
		Health = InputCard.Health; StaggerHealth = InputCard.StaggerHealth;
		SlashRes = InputCard.SlashRes; PierceRes = InputCard.PierceRes; BluntRes = InputCard.BluntRes;
		SlashStagRes = InputCard.SlashStagRes; PierceStagRes = InputCard.PierceStagRes; BluntStagRes = InputCard.BluntStagRes;
		MinSpeed = InputCard.MinSpeed; MaxSpeed = InputCard.MaxSpeed;
		PassiveList = InputCard.PassiveList;
		StrengthGrade = InputCard.StrengthGrade;
	}
	
	//Stuff for storing and loading data
	public string ConvertToDonTxt()
	{
		string[,] PropertiesData = new string[18,2];
		
		PropertiesData[0,0] = "Name"; PropertiesData[1,0] = "SpriteSheetSource";
		PropertiesData[2,0] = "BackHairOrigins"; PropertiesData[3,0] = "EyeOrigins";
		PropertiesData[4,0] = "MouthOrigins"; PropertiesData[5,0] = "FrontHairOrigins";
		PropertiesData[6,0] = "Health"; PropertiesData[7,0] = "StaggerHealth";
		PropertiesData[8,0] = "SlashRes"; PropertiesData[9,0] = "PierceRes"; PropertiesData[10,0] = "BluntRes";
		PropertiesData[11,0] = "SlashStagRes"; PropertiesData[12,0] = "PierceStagRes"; PropertiesData[13,0] = "BluntStagRes";
		PropertiesData[14,0] = "MinSpeed"; PropertiesData[15,0] = "MaxSpeed"; PropertiesData[16,0] = "StrengthGrade";
		PropertiesData[17,0] = "Passives";
		
		PropertiesData[0,1] = Name; PropertiesData[1,1] = SpriteSheetSource;
		PropertiesData[2,1] = SpritePositionsToString(BackHairOrigins); PropertiesData[3,1] = SpritePositionsToString(EyeOrigins);
		PropertiesData[4,1] = SpritePositionsToString(MouthOrigins); PropertiesData[5,1] = SpritePositionsToString(FrontHairOrigins);
		PropertiesData[6,1] = Convert.ToString(Health); PropertiesData[7,1] = Convert.ToString(StaggerHealth);
		PropertiesData[8,1] = Convert.ToString(SlashRes); PropertiesData[9,1] = Convert.ToString(PierceRes); PropertiesData[10,1] = Convert.ToString(BluntRes);
		PropertiesData[11,1] = Convert.ToString(SlashStagRes); PropertiesData[12,1] = Convert.ToString(PierceStagRes); PropertiesData[13,1] = Convert.ToString(BluntStagRes);
		PropertiesData[14,1] = Convert.ToString(MinSpeed); PropertiesData[15,1] = Convert.ToString(MaxSpeed); PropertiesData[16,1] = Convert.ToString(StrengthGrade);
		
		string PassiveTxt = "[";
		foreach(Passive ConvPassive in PassiveList)
		{PassiveTxt += ConvPassive.ConvertToDonTxt()+",";}
		string NewPassiveTxt = PassiveTxt.Substring(0,PassiveTxt.Length-1);
		if(NewPassiveTxt.Length > 0) {NewPassiveTxt += "]";}
		PropertiesData[17,1] = NewPassiveTxt;
		
		string DonTxtForm = DataStorageManager.ConvertToDonTxt("CHARACTERCARD", PropertiesData);
		
		return DonTxtForm;
	}
	public void SetFromPropertiesData(string[,] PropertiesData)
	{
		for(int Index = 0; Index<PropertiesData.GetLength(0); Index++)
		{
			switch(PropertiesData[Index,0])
			{
				case "Name": Name = PropertiesData[Index,1]; break;
				case "SpriteSheetSource": SpriteSheetSource = PropertiesData[Index,1]; break;
				case "BackHairOrigins": BackHairOrigins = StringToSpritePositions(PropertiesData[Index,1]); break;
				case "EyeOrigins": EyeOrigins = StringToSpritePositions(PropertiesData[Index,1]); break;
				case "MouthOrigins": MouthOrigins = StringToSpritePositions(PropertiesData[Index,1]); break;
				case "FrontHairOrigins": FrontHairOrigins = StringToSpritePositions(PropertiesData[Index,1]); break;
				case "Health": Health = Convert.ToInt32(PropertiesData[Index,1]); break;
				case "StaggerHealth": StaggerHealth = Convert.ToInt32(PropertiesData[Index,1]); break;
				case "SlashRes": SlashRes = Convert.ToDouble(PropertiesData[Index,1]); break;
				case "PierceRes": PierceRes = Convert.ToDouble(PropertiesData[Index,1]); break;
				case "BluntRes": BluntRes = Convert.ToDouble(PropertiesData[Index,1]); break;
				case "SlashStagRes": SlashStagRes = Convert.ToDouble(PropertiesData[Index,1]); break;
				case "PierceStagRes": PierceStagRes = Convert.ToDouble(PropertiesData[Index,1]); break;
				case "BluntStagRes": BluntStagRes = Convert.ToDouble(PropertiesData[Index,1]); break;
				case "MinSpeed": MinSpeed = Convert.ToInt32(PropertiesData[Index,1]); break;
				case "MaxSpeed": MaxSpeed = Convert.ToInt32(PropertiesData[Index,1]); break;
				case "Passives": PassiveList = StringListToPassiveList(StringToList(PropertiesData[Index,1])); break;
				case "StrengthGrade": StrengthGrade = Convert.ToInt32(PropertiesData[Index,1]); break;
			}
		}
	}
	
	private List<Passive> StringListToPassiveList(List<string> InputList)
	{
		List<Passive> OutputList = new List<Passive>();
		foreach(string PassiveDonTxt in InputList)
		{
			if(PassiveDonTxt != null && PassiveDonTxt != "" && PassiveDonTxt != " ")
			{
				Passive WorkingPassive  = new Passive();
				WorkingPassive.SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(PassiveDonTxt));
				OutputList.Add(WorkingPassive);
			}
		}
		return OutputList;
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
	
	private string SpritePositionsToString(int[,] InputArray)
	{
		string OutputString = "[";
		for(int IndexOne = 0; IndexOne<InputArray.GetLength(0); IndexOne++)
		{
			OutputString += "[";
			for(int IndexTwo = 0; IndexTwo<InputArray.GetLength(1); IndexTwo++)
			{
				OutputString += Convert.ToString(InputArray[IndexOne,IndexTwo]);
				if(IndexTwo < InputArray.GetLength(1)-1) {OutputString += ",";}
			}
			OutputString += "]";
		}
		OutputString += "]";
		return OutputString;
	}
	private int[,] StringToSpritePositions(string InputString)
	{
		int[,] OutputArray = new int[9,2]; 
		int OuterIndex = 0;
		int InnerIndex = 0;
		string WorkingString = "";
		for(int Index = 0; Index<InputString.Length; Index++)
		{
			if(InputString[Index] == ',') {try{OutputArray[OuterIndex,InnerIndex] = Convert.ToInt32(WorkingString);} catch{} InnerIndex++; WorkingString = "";}
			else if(InputString[Index] == ']') {try{OutputArray[OuterIndex,InnerIndex] = Convert.ToInt32(WorkingString);} catch{} OuterIndex++; InnerIndex = 0; WorkingString = "";}
			else if(InputString[Index] != '[') {WorkingString += InputString[Index];}
		}
		return OutputArray;
	}
}
