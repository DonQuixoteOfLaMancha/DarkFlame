using Godot;
using System;
using System.Collections.Generic;

public partial class StatusEffect : Node2D
{
	public string Name = "";
	public string Identifier = "";
	
	private string imagesource = "";
	public string ImageSource
	{
		get {return imagesource;}
		set
		{
			if(value != imagesource)
			{
				imagesource = value;
				if(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(value)) != -1)
				{IconImage = ResourceLoadUtils.LoadTexture2DFromImage(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImages[((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(value))], ((SceneTree)Engine.GetMainLoop()).Root);}
				else{IconImage = ResourceLoadUtils.LoadTexture2D(value, null);}
				if(GetChildCount() > 0) {Refresh();}
			}
		}
	}
	
	public Texture2D IconImage;
	
	public string Trigger = "";
	public string[] Condition = new string[4];
	public List<string[]> EffectList = new List<string[]>();
	public string Description = "";
	
	public bool AutoDecrement = true; //Controls whether it loses one count at the start of a new turn or not
	
	private int count = 0;
	public int Count
	{
		get {return count;}
		set
		{
			count = value;
			if(Count > 0)
			{
				if(GetChildCount() > 1) {GetChild<Label>(1).Text = Convert.ToString(count);}
			}
			else
			{
				if(GetChildCount() > 1)
				{
					GetParent().GetParent().GetParent<Character>().StatusEffects.RemoveAt(GetParent().GetParent().GetParent<Character>().StatusEffects.FindIndex(x => x.Equals(this)));
					QueueFree();
				}
			}
		}
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
		if(IconImage == null && ((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(ImageSource)) != -1)
		{IconImage = ResourceLoadUtils.LoadTexture2DFromImage(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImages[((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(ImageSource))], ((SceneTree)Engine.GetMainLoop()).Root);}
		else if(IconImage == null) {IconImage = ResourceLoadUtils.LoadTexture2D(ImageSource, ((SceneTree)Engine.GetMainLoop()).Root);}
		GetChild<Sprite2D>(0).Texture = IconImage;
		if(IconImage != null) {GetChild<Sprite2D>(0).Scale = new Vector2((float)100/IconImage.GetWidth(),(float)100/IconImage.GetHeight());}
		GetChild(2).GetChild<Label>(2).Text = Name;
		GetChild(2).GetChild<Label>(3).Text = Description;
	}
	
	public void CopyStatusEffect(StatusEffect InputStatusEffect)
	{
		Name = InputStatusEffect.Name;
		Identifier = InputStatusEffect.Identifier;
		imagesource = InputStatusEffect.ImageSource;
		IconImage = InputStatusEffect.IconImage;
		Trigger = InputStatusEffect.Trigger;
		Condition = InputStatusEffect.Condition;
		EffectList = InputStatusEffect.EffectList;
		Description = InputStatusEffect.Description;
		AutoDecrement = InputStatusEffect.AutoDecrement;
	}
	
	//Storage conversion stuff
	public void SetFromPropertiesData(string[,] PropertiesData)
	{
		for(int Index = 0; Index<PropertiesData.GetLength(0); Index++)
		{
			switch(PropertiesData[Index,0])
			{
				case "Name": Name = PropertiesData[Index,1]; break;
				case "Identifier": Identifier = PropertiesData[Index,1]; break;
				case "ImageSource": ImageSource = PropertiesData[Index,1]; break;
				case "Trigger": Trigger = PropertiesData[Index,1]; break;
				case "Condition": Condition = StringToArray(PropertiesData[Index,1]); break;
				case "EffectList": EffectList = StringToStringArrayList(PropertiesData[Index,1]); break;
				case "Description": Description = PropertiesData[Index, 1]; break;
				case "AutoDecrement": AutoDecrement = Convert.ToBoolean(PropertiesData[Index, 1]); break;
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
	
	private List<string[]> StringToStringArrayList(string InputString)
	{
		List<string[]> OutputList = new List<string[]>();
		
		int Depth = -1;
		string WorkingString = "";
		foreach(char ConvChar in InputString)
		{
			if(ConvChar == '[') {Depth++; continue;}
			if(ConvChar == ']' && Depth>0) {WorkingString += ConvChar;Depth--; continue;}
			if((ConvChar == ',' || ConvChar == ']') && Depth == 0) {OutputList.Add(StringToArray(WorkingString)); WorkingString = ""; continue;}
			WorkingString += ConvChar;
		}
		
		return OutputList;
	}
	
	//Only used in-dev to make it easier for me to make new Status effects
	public void Store(string Filepath)
	{DataStorageManager.StoreDonTxt(Filepath, "STATUSEFFECT", ConvertToDonTxt(), "Identifier", Identifier);}
	
	public string ConvertToDonTxt()
	{
		string[,] PropertiesData = new string[8,2];
		
		PropertiesData[0,0] = "Name"; PropertiesData[1,0] = "Identifier"; PropertiesData[2,0] = "ImageSource";
		PropertiesData[3,0] = "Trigger";
		PropertiesData[4,0] = "Condition";
		PropertiesData[5,0] = "EffectList";
		PropertiesData[6,0] = "Description";
		PropertiesData[7,0] = "AutoDecrement";
		
		
		PropertiesData[0,1] = Name; PropertiesData[1,1] = Identifier; PropertiesData[2,1] = ImageSource;
		PropertiesData[3,1] = Trigger;
		PropertiesData[4,1] = "["+Condition[0]+","+Condition[1]+","+Condition[2]+","+Condition[3]+"]";
		PropertiesData[5,1] = "[";
		foreach(string[] ConvEffect in EffectList)
		{PropertiesData[5,1] += "["+ConvEffect[0]+","+ConvEffect[1]+","+ConvEffect[2]+","+ConvEffect[3]+"],";}
		PropertiesData[5,1] = PropertiesData[5,1].Substring(0, PropertiesData[5,1].Length-1)+"]";
		
		PropertiesData[6,1] = Description;
		
		PropertiesData[7,1] = Convert.ToString(AutoDecrement);
		
		string DonTxtForm = DataStorageManager.ConvertToDonTxt("STATUSEFFECT", PropertiesData);
		
		return DonTxtForm;
	}
	
	
	
	//Connection Inputs
	private void _Hovered()
	{GetChild<Node2D>(2).Show();}
	private void _Unhovered()
	{GetChild<Node2D>(2).Hide();}
}
