using Godot;
using System;
using System.Collections.Generic;

public partial class Battle : Node
{
	public string Name = "Defoko";
	public string imgpath;
	public string ImgPath
	{
		get {return imgpath;}
		set
		{
			if(value != imgpath)
			{
				imgpath = value;
				try{Icon = ImageTexture.CreateFromImage(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImages[((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(value))]);}
				catch{Icon = ResourceLoadUtils.LoadTexture2D(value, null);}
			}
		}
	}
	public string Author = "DonQui";
	public string Description = "The default battle, used to ensure there is a usable battle at all times, even if the DonTxts are all deleted.";
	
	public int DiffGrade = 1;
	public List<Encounter> Encounters = new List<Encounter>(1){new Encounter()};
	
	public Texture2D Icon;
	
	public Battle()
	{
		ImgPath = "user://Content/DefaultContent/Images/BattleIcons/Defoko.png";
	}
	
	//Stuff for storing and loading data
	public string ConvertToDonTxt()
	{
		string[,] PropertiesData = new string[7,2];
		
		PropertiesData[0,0] = "Name"; PropertiesData[1,0] = "ImgPath"; PropertiesData[2,0] = "Author";
		PropertiesData[3,0] = "Description"; PropertiesData[4,0] = "DiffGrade";
		PropertiesData[5,0] = "Encounters";
		
		PropertiesData[0,1] = Name; PropertiesData[1,1] = ImgPath; PropertiesData[2,1] = Author;
		PropertiesData[3,1] = Description; PropertiesData[4,1] = Convert.ToString(DiffGrade);
		PropertiesData[5,1] = EncountersToDonTxt();
		
		string DonTxtForm = DataStorageManager.ConvertToDonTxt("BATTLE", PropertiesData);
		
		return DonTxtForm;
	}
	public void SetFromPropertiesData(string[,] PropertiesData)
	{
		for(int Index = 0; Index<PropertiesData.GetLength(0); Index++)
		{
			switch(PropertiesData[Index,0])
			{
				case "Name": Name = PropertiesData[Index,1]; break;
				case "ImgPath": ImgPath = PropertiesData[Index,1]; break;
				case "Author": Author = PropertiesData[Index,1]; break;
				case "Description": Description = PropertiesData[Index,1]; break;
				case "DiffGrade": DiffGrade = Convert.ToInt32(PropertiesData[Index,1]); break;
				case "Encounters": Encounters = StringListToEncountersList(StringToList(PropertiesData[Index,1])); break;
			}
		}
	}
	
	
	private string EncountersToDonTxt()
	{
		string EncountersTxt = "[";
		for(int Index = 0; Index<Encounters.Count; Index++)
		{
			EncountersTxt += Encounters[Index].ConvertToDonTxt();
			if(Index<Encounters.Count-1) {EncountersTxt += ",";}
		}
		EncountersTxt += "]";
		
		return EncountersTxt;
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
	private List<Encounter> StringListToEncountersList(List<string> InputList)
	{
		List<Encounter> OutputList = new List<Encounter>();
		foreach(string ConvString in InputList)
		{
			Encounter WorkingEncounter = new Encounter();
			WorkingEncounter.SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(ConvString));
			OutputList.Add(WorkingEncounter);
		}
		return OutputList;
	}
}


public partial class Encounter : Node
{
	public Character[] Enemies = new Character[5] {new Character(), new Character(), new Character(), new Character(), new Character()};
	public string backgroundimgpath;
	public string BackGroundImgPath
	{
		get {return backgroundimgpath;}
		set
		{
			if(value != backgroundimgpath)
			{
				backgroundimgpath = value;
				try{BackGroundImage = ImageTexture.CreateFromImage(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImages[((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(value))]);}
				catch{BackGroundImage = ResourceLoadUtils.LoadTexture2D(value, null);}
			}
		}
	}
	public string TargetingType = "Random";
	
	public Texture2D BackGroundImage;
	
	
	public Encounter()
	{
		BackGroundImgPath = "user://Content/DefaultContent/Images/BattleIcons/Defoko.png";
	}
	
	
	//Stuff for storing and loading data
	
	public string ConvertToDonTxt()
	{
		string[,] PropertiesData = new string[3,2];
		
		PropertiesData[0,0] = "Enemies"; PropertiesData[1,0] = "BackGroundImgPath"; PropertiesData[2,0] = "TargetingType";
		
		PropertiesData[0,1] = "["+Enemies[0].ConvertToDonTxt()+","+Enemies[1].ConvertToDonTxt()+","+Enemies[2].ConvertToDonTxt()+","+Enemies[3].ConvertToDonTxt()+","+Enemies[4].ConvertToDonTxt()+"]";
		PropertiesData[1,1] = BackGroundImgPath; PropertiesData[2,1] = TargetingType;
		
		string DonTxtForm = DataStorageManager.ConvertToDonTxt("ENCOUNTER", PropertiesData);
		
		return DonTxtForm;
	}
	public void SetFromPropertiesData(string[,] PropertiesData)
	{
		for(int Index = 0; Index<PropertiesData.GetLength(0); Index++)
		{
			switch(PropertiesData[Index,0])
			{
				case "Enemies": Enemies = StringToCharacterArray(PropertiesData[Index,1]); break;
				case "BackGroundImgPath": BackGroundImgPath = PropertiesData[Index,1]; break;
				case "TargetingType": TargetingType = PropertiesData[Index,1]; break;
			}
		}
	}
	
	
	private string[] StringToArray(string InputString)
	{
		List<string> OutputList = new List<string>();
		string WorkingString = "";
		int Depth = -1;
		foreach(char ConvChar in InputString)
		{
			if(ConvChar == '[') {Depth++; if(Depth==0){continue;}}
			if(ConvChar == ']') {Depth--;}
			if((ConvChar == ',' && Depth == 0) || (Depth == -1 && WorkingString != "")) {OutputList.Add(WorkingString); WorkingString = ""; continue;}
			WorkingString += ConvChar;
		}

		return OutputList.ToArray();
	}
	private Character[] StringToCharacterArray(string InputString)
	{
		Character[] OutputArray = new Character[5];
		string[] Middleman = StringToArray(InputString);
		for(int Index = 0; Index<Middleman.Length && Index < 5; Index++)
		{
			Character WorkingCharacter = new Character();
			WorkingCharacter.SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(Middleman[Index]));
			OutputArray[Index] = WorkingCharacter;
		}
		for(int Index = 0; Index < 5; Index++) {if(OutputArray[Index] == null) {OutputArray[Index] = new Character();}}
		
		return OutputArray;
	}
	
}
