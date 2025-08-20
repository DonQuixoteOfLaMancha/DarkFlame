using Godot;
using System;

public partial class Passive : Node2D
{
	public string Trigger = "";
	public string[] Condition = new string[4]; //EXPLAIN FORMAT 0 - __, 1 - __, 2 - __, 3 - __
	public string[] Effect = new string[4]; //EXPLAIN FORMAT 0 - __, 1 - __, 2 - __, 3 - __
	public string DescOverride = ""; //Manually written description, can only be set by directly modifying the .txt file
	
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
		if(DescOverride == "" || DescOverride == " " || DescOverride == null) {GetChild<Label>(2).Text = InformationConversionUtils.ConditionalToText(Trigger, Condition, Effect);}
		else {GetChild<Label>(2).Text = DescOverride;}
	}
	
	//Stuff for storing and loading data
	public string ConvertToDonTxt()
	{
		string[,] PropertiesData = new string[4,2];
		
		PropertiesData[0,0] = "Trigger";
		PropertiesData[1,0] = "Condition"; PropertiesData[2,0] = "Effect";
		PropertiesData[3,0] = "DescOverride";
		
		PropertiesData[0,1] = Trigger;
		PropertiesData[1,1] = "["+Condition[0]+","+Condition[1]+","+Condition[2]+","+Condition[3]+"]";
		PropertiesData[2,1] = "["+Effect[0]+","+Effect[1]+","+Effect[2]+","+Effect[3]+"]";
		PropertiesData[3,1] = DescOverride;
		
		string DonTxtForm = DataStorageManager.ConvertToDonTxt("PASSIVE", PropertiesData);
		
		return DonTxtForm;
	}
	public void SetFromPropertiesData(string[,] PropertiesData)
	{
		for(int Index = 0; Index<PropertiesData.GetLength(0); Index++)
		{
			switch(PropertiesData[Index,0])
			{
				case "Trigger": Trigger = PropertiesData[Index,1]; break;
				case "Condition": Condition = StringToArray(PropertiesData[Index,1]); break;
				case "Effect": Effect = StringToArray(PropertiesData[Index,1]); break;
				case "DescOverride": DescOverride = PropertiesData[Index,1]; break;
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
