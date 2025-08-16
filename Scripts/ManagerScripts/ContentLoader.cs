using Godot;
using System;
using System.Collections.Generic;

public static class ContentLoader
{
	public static List<string> UIAudioFileNames = new List<string>() {"UI_Click", "Page_Turn", "Card_Select", "Card_Cancel", "UI_Save", "UI_Load"};
	public static List<string> PhaseAudioFileNames = new List<string>() {"EncounterStart", "CombatStart", "BattleLost", "BattleWon"};
	public static List<string> ClashAudioFileNames = new List<string>() {"Clash_Slash", "Clash_Pierce", "Clash_Blunt", "Clash_Block", "Clash_Evade", "Clash_Damage"};


	//Locating files
	public static List<string> GetTxtFilesContaining(string SearchTerm, Node Root = null) //Returns a list with the filepath of every TXT within Content and its subfolders containing a given string
	{
		try
		{
			if (DirAccess.Open("user://").DirExists("Content"))
			{
				List<string> DirectoryList = GetDirectories("user://Content"); //Gets all the folders in Content
				List<string> TxtFileList = new List<string>();
				foreach (string SearchDirectory in DirectoryList) { TxtFileList.AddRange(SearchFiles(SearchDirectory, ".txt")); } //Finds all text files in the found folders
				List<string> OutputTxtFileList = new List<string>();
				foreach (string SearchFile in TxtFileList)
				{
					if (FileAccess.FileExists(SearchFile))
					{
						using var DataFile = FileAccess.Open(SearchFile, FileAccess.ModeFlags.Read);
						string FileContents = DataFile.GetAsText();
						if (FileContents.Contains(SearchTerm)) { OutputTxtFileList.Add(SearchFile); } //Checks each text file for the given search term
					}
				}
				return OutputTxtFileList;
			}
			else
			{
				if (Root != null) { Root.GetChild(0).GetChild<Console>(1).WriteToConsole("Content folder missing"); }
				return new List<string>();
			}
		}
		catch (Exception e)
		{
			if (Root != null) { Root.GetChild(0).GetChild<Console>(1).WriteToConsole("Some error occured when trying to find the DonTxtFiles: " + e); }
			return new List<string>();
		}
	}
	
	public static List<string> GetPngFiles(Node Root = null) //Returns a list with the filepath of every PNG within Content and its subfolders
	{
		try
		{
			if(DirAccess.Open("user://").DirExists("Content"))
			{
				List<string> DirectoryList = GetDirectories("user://Content"); //Gets all folders in Content
				List<string> PngFileList = new List<string>();
				foreach(string SearchDirectory in DirectoryList) {PngFileList.AddRange(SearchFiles(SearchDirectory, ".png"));} //Gets all png files in the folders found
				
				return PngFileList;
			}
			else
			{
				if(Root != null) {Root.GetChild(0).GetChild<Console>(1).WriteToConsole("Content folder missing");}
				return new List<string>();
			}
		}
		catch (Exception e)
		{
			if(Root != null) {Root.GetChild(0).GetChild<Console>(1).WriteToConsole("Some error occured when trying to find the Png Files: "+e);}
			return new List<string>();
		}
	}

	public static List<string> GetAudioFilesContaining(string SearchTerm, Node Root = null) //Returns a list with the filepath of every TXT within Content and its subfolders containing a given string
	{
		try
		{
			if (DirAccess.Open("user://").DirExists("Content"))
			{
				List<string> DirectoryList = GetDirectories("user://Content"); //Gets all the folders in Content
				List<string> AudioFileList = new List<string>();
				foreach (string SearchDirectory in DirectoryList) { AudioFileList.AddRange(SearchFiles(SearchDirectory, ".ogg")); AudioFileList.AddRange(SearchFiles(SearchDirectory, ".wav")); } //Finds all ogg and wav files in the found folders
				List<string> OutputAudioFileList = new List<string>();
				foreach (string SearchFile in AudioFileList)
				{
					if (SearchFile.Contains(SearchTerm)) { OutputAudioFileList.Add(SearchFile); } //Checks each ogg filename for the given search term
				}

				return OutputAudioFileList;
			}
			else
			{
				if (Root != null) { Root.GetChild(0).GetChild<Console>(1).WriteToConsole("Content folder missing"); }
				return new List<string>();
			}
		}
		catch (Exception e)
		{
			if (Root != null) { Root.GetChild(0).GetChild<Console>(1).WriteToConsole("Some error occured when trying to find the Ogg Files: " + e); }
			return new List<string>();
		}
	}

	//Stuff for non-music audio files
	public static List<string> GetAudioFilesFromTermList(List<string> SearchTermList, Node Root = null)
	{
		try
		{
			if (DirAccess.Open("user://").DirExists("Content"))
			{
				List<string> DirectoryList = GetDirectories("user://Content"); //Gets all the folders in Content
				List<string> AudioFileList = new List<string>();
				foreach (string SearchDirectory in DirectoryList) { AudioFileList.AddRange(SearchFiles(SearchDirectory, ".ogg")); AudioFileList.AddRange(SearchFiles(SearchDirectory, ".wav")); } //Finds all ogg and wav files in the found folders
				List<string> OutputAudioFileList = new List<string>();
				foreach (string SearchFile in AudioFileList)
				{
					foreach (string SearchTerm in SearchTermList) //Checks each ogg filename for the given search term
					{
						if (SearchFile.Contains(SearchTerm))
						{
							OutputAudioFileList.Add(SearchFile);
							break;
						} 
					}
				}

				return OutputAudioFileList;
			}
			else
			{
				if (Root != null) { Root.GetChild(0).GetChild<Console>(1).WriteToConsole("Content folder missing"); }
				return new List<string>();
			}
		}
		catch (Exception e)
		{
			if (Root != null) { Root.GetChild(0).GetChild<Console>(1).WriteToConsole("Some error occured when trying to find the Ogg Files: " + e); }
			return new List<string>();
		}
	}


	
	private static List<string> GetDirectories(string SearchDirectory) //Returns a list with the paths all subfolders of a given folder, including folders within folders
	{
		List<string> Output = new List<String>() { SearchDirectory };
		string[] SubDirs = SearchDir(SearchDirectory);
		foreach (string SubDirectory in SubDirs)
		{
			Output.AddRange(GetDirectories(SearchDirectory + "/" + SubDirectory));
		}
		return Output;
	}
	private static string[] SearchDir(string SearchDirectory) //Returns the subfolders of a given folder
	{
		using var Dir = DirAccess.Open(SearchDirectory);
		if(Dir != null) {return Dir.GetDirectories();}
		else {return null;}
	}
	private static List<string> SearchFiles(string SearchDirectory, string Extension) //Returns a list with the filepaths of all files containing a given string in their name (intended to be the extension) in a given folder
	{
		List<string> Output = new List<string>();
		var Dir = DirAccess.Open(SearchDirectory);
		string[] FileArray = Dir.GetFiles();
		foreach(string FileName in FileArray) {if(FileName.Contains(Extension)) {Output.Add(SearchDirectory+"/"+FileName);}}
		
		return Output;
	}
	
	
	//Loading files
	public static List<Image> PreloadImages(List<string> PngFilePaths, Node Root = null) //Returns a list with all images corresponding to the given filepaths in the list
	{
		List<Image> OutputList = new List<Image>();
		foreach(string FilePath in PngFilePaths)
		{
			try
				{
					var ImgBytes = FileAccess.GetFileAsBytes(FilePath);
					Image Img = new Image();
					Img.LoadPngFromBuffer(ImgBytes);
					OutputList.Add(Img);
				}
			catch
				{
					if(Root != null) {Root.GetChild(0).GetChild<Console>(1).WriteToConsole("An error occured when loading the file at the path "+FilePath);}
					return null;
				}
		}
		return OutputList;
	}
	
	public static Character[] LoadPlayerTeam(string FilePath) //Loads the characters from a DonTxt File with 5 characters and returns it as an array
	{
		Character[] OutputArray = new Character[5]{new Character(), new Character(), new Character(), new Character(), new Character()};
		string WholeDonTxt = DataStorageManager.ParseDonTxt(FilePath, "CHARACTER");
		if(DataStorageManager.GetNumberOfDonTxtEntries("CHARACTER", WholeDonTxt) == 5)
		{
			for(int Index = 0; Index < 5; Index++)
			{
				string IndividualDonTxt = DataStorageManager.GetDonTxtEntryFromIndex("CHARACTER", WholeDonTxt, Index);
				Character WorkingCharacter = new Character();
				WorkingCharacter.SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(IndividualDonTxt));
				OutputArray[Index] = WorkingCharacter;
			}
		}
		return OutputArray;
	}
	
	public static List<CombatCard> LoadCombatCards(List<string> DonTxtFilePaths) //Returns a list of all combat cards from the given paths
	{
		List<CombatCard> OutputList = new List<CombatCard>();
		foreach(string FilePath in DonTxtFilePaths)
		{
			string WholeDonTxt = DataStorageManager.ParseDonTxt(FilePath, "COMBATCARD");
			for(int Index = 0; Index < DataStorageManager.GetNumberOfDonTxtEntries("COMBATCARD", WholeDonTxt); Index++)
			{
				string IndividualDonTxt = DataStorageManager.GetDonTxtEntryFromIndex("COMBATCARD", WholeDonTxt, Index);
				CombatCard WorkingCard = new CombatCard();
				WorkingCard.SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(IndividualDonTxt));
				OutputList.Add(WorkingCard);
			}
		}
		
		return OutputList;
	}
	public static List<CharacterCard> LoadCharacterCards(List<string> DonTxtFilePaths) //Returns a list of all character cards from the given paths
	{
		List<CharacterCard> OutputList = new List<CharacterCard>();
		foreach(string FilePath in DonTxtFilePaths)
		{
			string WholeDonTxt = DataStorageManager.ParseDonTxt(FilePath, "CHARACTERCARD");
			for(int Index = 0; Index < DataStorageManager.GetNumberOfDonTxtEntries("CHARACTERCARD", WholeDonTxt); Index++)
			{
				string IndividualDonTxt = DataStorageManager.GetDonTxtEntryFromIndex("CHARACTERCARD", WholeDonTxt, Index);
				CharacterCard WorkingCard = new CharacterCard();
				WorkingCard.SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(IndividualDonTxt));
				OutputList.Add(WorkingCard);
			}
		}
		
		return OutputList;
	}
	public static List<Battle> LoadBattles(List<string> DonTxtFilePaths) //Returns a list of all battles from the given paths
	{
		List<Battle> OutputList = new List<Battle>();
		foreach(string FilePath in DonTxtFilePaths)
		{
			string WholeDonTxt = DataStorageManager.ParseDonTxt(FilePath, "BATTLE");
			for(int Index = 0; Index < DataStorageManager.GetNumberOfDonTxtEntries("BATTLE", WholeDonTxt); Index++)
			{
				string IndividualDonTxt = DataStorageManager.GetDonTxtEntryFromIndex("BATTLE", WholeDonTxt, Index);
				Battle WorkingBattle = new Battle();
				WorkingBattle.SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(IndividualDonTxt));
				OutputList.Add(WorkingBattle);
			}
		}
		
		return OutputList;
	}
	public static List<StatusEffect> LoadStatusEffects(List<string> DonTxtFilePaths) //Returns a list of all status effects from the given paths
	{
		List<StatusEffect> OutputList = new List<StatusEffect>();
		foreach(string FilePath in DonTxtFilePaths)
		{
			string WholeDonTxt = DataStorageManager.ParseDonTxt(FilePath, "STATUSEFFECT");
			for(int Index = 0; Index < DataStorageManager.GetNumberOfDonTxtEntries("STATUSEFFECT", WholeDonTxt); Index++)
			{
				string IndividualDonTxt = DataStorageManager.GetDonTxtEntryFromIndex("STATUSEFFECT", WholeDonTxt, Index);
				StatusEffect WorkingStatusEffect = new StatusEffect();
				WorkingStatusEffect.SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(IndividualDonTxt));
				OutputList.Add(WorkingStatusEffect);
			}
		}
		
		return OutputList;
	}
	
	public static List<AudioStream> LoadAudioFiles(List<string> AudioFilePaths)
	{
		List<AudioStream> OutputList = new List<AudioStream>();
		foreach (string Path in AudioFilePaths)
		{
			if (Path.Contains(".ogg"))
			{
				AudioStreamOggVorbis OggStream = AudioStreamOggVorbis.LoadFromFile(Path);

				OggStream.ResourceName = Path.GetFile().GetBaseName().Replace("BATTLE_", "").Replace("MENU_", "");
				OutputList.Add(OggStream);
			}
			else if (Path.Contains(".wav"))
			{
				AudioStreamWav WavStream = new AudioStreamWav();

				var WavFile = FileAccess.Open(Path, FileAccess.ModeFlags.Read);
				var WavAudioData = WavFile.GetBuffer((int)WavFile.GetLength());

				WavStream.Format = AudioStreamWav.FormatEnum.Format16Bits;
				WavStream.Stereo = true;
				WavStream.ResourceName = Path.GetFile().GetBaseName().Replace("BATTLE_", "").Replace("MENU_", "");
				WavStream.Data = WavAudioData;
				OutputList.Add(WavStream);
			}
		}
		
		return OutputList;
	}
}
