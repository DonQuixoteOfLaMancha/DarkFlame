using Godot;
using System;
using System.Collections.Generic;

public partial class StartUpScreen : Node2D
{
	double TimeTrack = 1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(!DirAccess.Open("user://").DirExists("Content"))
		{SetUpDefaultUserFolder();}
		
		string[,] SettingPropertiesData = DataStorageManager.ConvertToPropertiesData(DataStorageManager.ParseDonTxt("user://Content/Settings.txt", "SETTINGS"));
		for(int Index = 0; Index<SettingPropertiesData.GetLength(0); Index++)
		{
			switch(SettingPropertiesData[Index, 0])
			{
				case "Master":  AudioServer.SetBusVolumeDb(0, Mathf.LinearToDb(Convert.ToSingle(SettingPropertiesData[Index, 1]))); break;
				case "SFX": AudioServer.SetBusVolumeDb(1, Mathf.LinearToDb(Convert.ToSingle(SettingPropertiesData[Index, 1]))); break;
				case "Music": AudioServer.SetBusVolumeDb(2, Mathf.LinearToDb(Convert.ToSingle(SettingPropertiesData[Index, 1]))); break;
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		TimeTrack -= delta;
		if(TimeTrack <= 0)
		{
			GetTree().ChangeSceneToFile("res://Scenes/GameManager.tscn");
		}
	}
	
	private void SetUpDefaultUserFolder() //Copies a default content folder in to the user directory for when it's missing
	{
		string FolderPath = OS.GetExecutablePath();
		if(FolderPath.Contains("DarkFlame.console.exe")) {FolderPath = FolderPath.Replace("DarkFlame.console.exe", "");}
		else if(FolderPath.Contains("DarkFlame.exe")) {FolderPath = FolderPath.Replace("DarkFlame.exe", "");}
		else if(FolderPath.Contains("Godot_v4.3-stable_mono_win64.exe") || FolderPath.Contains("Godot_v4.3-stable_mono_win64_console.exe")) {FolderPath = "res://";}
		List<string> Directories = GetDirectories(FolderPath+"DefaultUserFolder/Content");
		List<string> FileList = new List<string>();
		foreach(string SearchDirectory in Directories)
		{
			FileList.AddRange(SearchFiles(SearchDirectory));
			DirAccess.MakeDirRecursiveAbsolute(SearchDirectory.Replace(FolderPath+"DefaultUserFolder","user://"));
		}
		
		for(int Index = 0; Index<FileList.Count; Index++)
		{DirAccess.CopyAbsolute(FileList[Index], FileList[Index].Replace(FolderPath+"DefaultUserFolder", "user://"));}
	}
	
	private static List<string> GetDirectories(string SearchDirectory) //Returns a list with the paths all subfolders of a given folder, including folders within folders
	{
		List<string> Output = new List<String>(){SearchDirectory};
		string[] SubDirs = SearchDir(SearchDirectory);
		foreach(string SubDirectory in SubDirs)
		{
			Output.AddRange(GetDirectories(SearchDirectory+"/"+SubDirectory));
		}
		return Output;
	}
	private static string[] SearchDir(string SearchDirectory) //Returns the subfolders of a given folder
	{
		using var Dir = DirAccess.Open(SearchDirectory);
		if(Dir != null) {return Dir.GetDirectories();}
		else {return new string[0];}
	}
	private static List<string> SearchFiles(string SearchDirectory) //Returns a list with the filepaths of all files in a given folder
	{
		List<string> Output = new List<string>();
		var Dir = DirAccess.Open(SearchDirectory);
		if(Dir != null) 
		{
			string[] FileArray = Dir.GetFiles();
			foreach(string FileName in FileArray) {if(!FileName.Contains(".import")){Output.Add(SearchDirectory+"/"+FileName);}}
		}
		
		return Output;
	}
}
