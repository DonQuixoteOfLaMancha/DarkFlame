using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node2D
{
	public Character[] PlayerTeam = new Character[5]{new Character(), new Character(), new Character(), new Character(), new Character()};
	
	public Texture2D[] DiceTextures = new Texture2D[9]{ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiceIconSheet.png",OriginX : 0, XSize : 95, YSize : 90),
														ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiceIconSheet.png",OriginX : 96, XSize : 95, YSize : 90),
														ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiceIconSheet.png",OriginX : 192, XSize : 95, YSize : 90),
														ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiceIconSheet.png",OriginX : 288, XSize : 95, YSize : 90),
														ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiceIconSheet.png",OriginX : 384, XSize : 95, YSize : 90),
														ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiceIconSheet.png",OriginX : 480, XSize : 95, YSize : 90),
														ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiceIconSheet.png",OriginX : 576, XSize : 95, YSize : 90),
														ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiceIconSheet.png",OriginX : 672, XSize : 95, YSize : 90),
														ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiceIconSheet.png",OriginX : 768, XSize : 95, YSize : 90)};
	
	public Texture2D[] DiffGradeTextures = new Texture2D[5]{ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiffGradeIconSheet.png",OriginX : 0, XSize : 160, YSize : 120),
															ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiffGradeIconSheet.png",OriginX : 161, XSize : 160, YSize : 120),
															ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiffGradeIconSheet.png",OriginX : 322, XSize : 160, YSize : 120),
															ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiffGradeIconSheet.png",OriginX : 483, XSize : 160, YSize : 120),
															ResourceLoadUtils.LoadTexture2D("user://Content/DefaultContent/Images/DiffGradeIconSheet.png",OriginX : 644, XSize : 160, YSize : 120)};
	
	
	public List<string> PreloadedImagePaths = new List<string>();
	public List<Image> PreloadedImages = new List<Image>();
	
	public List<string> LoadedSpriteSheetPaths = new List<string>();
	public List<SpriteFrames> LoadedSpriteSheets = new List<SpriteFrames>();
	
	public List<CombatCard> LoadedCombatCards = new List<CombatCard>();
	public List<CharacterCard> LoadedCharacterCards = new List<CharacterCard>();
	public List<Battle> LoadedBattles = new List<Battle>();
	public List<StatusEffect> LoadedStatusEffects = new List<StatusEffect>();
	
	public List<AudioStream> LoadedMenuMusic = new List<AudioStream>();
	public List<AudioStream> LoadedBattleMusic = new List<AudioStream>();
	
	//Stuff for switching between menu and battle
	bool CurrentScene = true; //true = menu, false = battle
	int SceneChangeTicker = 4;
	Battle SwitchToBattle = new Battle();
	
	
	
	int RefreshTicker = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetViewport().SizeChanged += () => MatchWindowSize();
		GetTree().AutoAcceptQuit = false;
		TrueRefresh();
		
		GetChild(0).GetChild(2).GetChild<HSlider>(1).Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(0));
		GetChild(0).GetChild(3).GetChild<HSlider>(1).Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(1));
		GetChild(0).GetChild(4).GetChild<HSlider>(1).Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(2));
		
		//StatusEffect making code, only accessible by manually editing the code to make the false true
		if(false)
		{
			StatusEffect CreationStatusEffect = new StatusEffect();
			
			CreationStatusEffect.Name = "Burn";
			CreationStatusEffect.Identifier = "burn";
			CreationStatusEffect.ImageSource = "user://Content/DefaultContent/Images/StatusEffectIcons/BurnIcon.png";
			CreationStatusEffect.Trigger = "COMBATEND";
			CreationStatusEffect.Condition = new string[4]{"User","Health","Gt","0"};
			CreationStatusEffect.EffectList = new List<string[]>{new string[4]{"User","Health","Dec","Status.burn"},
																	new string[4]{"User","Status.burn","Div","3"}};
			CreationStatusEffect.Description = "At turn end, reduce health by count and lower count.";
			CreationStatusEffect.AutoDecrement = false;
			
			CreationStatusEffect.Store("user://Content/DefaultContent/DonTxtFiles/StatusEffects.txt");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(RefreshTicker == 2) {RefreshTicker--;}
		else if(RefreshTicker == 1) {TrueRefresh();}
		
		if(SceneChangeTicker%2 == 0 && SceneChangeTicker > 0) {SceneChangeTicker--;}
		else if(SceneChangeTicker == 3) 
		{
			if(GetChildCount() > 4) {GetChild(4).QueueFree();}
			if(CurrentScene)
			{
				var MenuScene = GD.Load<PackedScene>("res://Scenes/Main Menu.tscn");
				var MenuInstance = MenuScene.Instantiate();
				AddChild(MenuInstance);
			}
			else
			{
				var BattleScene = GD.Load<PackedScene>("res://Scenes/BattleScene.tscn");
				var BattleInstance = BattleScene.Instantiate();
				AddChild(BattleInstance);
			}
			SceneChangeTicker--;
		}
		else if(SceneChangeTicker == 1) {SetUpScene();}
	}
	
	//Following function sourced from docs.godotengine.org, handles inputs from windows
	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest) {QuitGame();} //Runs the custom quit function if the windows event to close the window is triggered
	}
	
	//Handles custom input events (e.g. esc for settings)
	public override void _Input(InputEvent @event)
	{
		if(@event.IsActionPressed("open_console")) //Opens/Closes the console if the relevant key (`) is pressed
		{GetChild<Node2D>(1).Visible = !(GetChild<Node2D>(1).Visible);}
		if(@event.IsActionPressed("open_settings")) //Opens/Closes the settings menu if the relevant key (esc) is pressed
		{GetChild<Node2D>(0).Visible = !(GetChild<Node2D>(0).Visible);}
		if(@event.IsActionPressed("toggle_combat_skip") && !CurrentScene) //Switches the skip in the battle scene
		{GetNode<BattleSceneManager>("BattleScene").Skip = !(GetNode<BattleSceneManager>("BattleScene").Skip);}
	}
	
	
	//Custom internal functions
	private void MatchWindowSize()
	{
		Scale = new Vector2(GetViewportRect().Size.X/1920, GetViewportRect().Size.Y/1080);
	}
	
	public void QuitGame()
	{
		SavePlayerTeam("user://Content/PlayerTeamSave.txt");
		
		string[,] SettingsPropertiesData = new string[4,2]{{"Name", "AudioSettings"},
															{"Master",Convert.ToString(Mathf.DbToLinear(AudioServer.GetBusVolumeDb(0)))},
															{"SFX",Convert.ToString(Mathf.DbToLinear(AudioServer.GetBusVolumeDb(1)))},
															{"Music",Convert.ToString(Mathf.DbToLinear(AudioServer.GetBusVolumeDb(2)))}};
		
		DataStorageManager.StoreDonTxt("user://Content/Settings.txt", "SETTINGS", DataStorageManager.ConvertToDonTxt("SETTINGS", SettingsPropertiesData), "Name", "AudioSettings");
		
		GetTree().Quit();
	}
	
	public void SwitchScene(bool MenuOrBattle) //true is Menu, false is Battle
	{
		GetChild<Node2D>(2).Show();
		if(MenuOrBattle != CurrentScene)
		{
			CurrentScene = MenuOrBattle;
			SceneChangeTicker = 4;
		}
	}
	
	private void SetUpScene()
	{
		if(CurrentScene) {UpdateMenu();}
		else {SetUpBattle();}
		if(RefreshTicker == 0) {GetChild<Node2D>(2).Hide();}
		SceneChangeTicker = 0;
	}

	private void UpdateMenu()
	{
		GetNode("MainMenu").GetChild(2).GetChild<BattleSelectionMenu>(0).Refresh(LoadedBattles);
		GetNode("MainMenu").GetChild(2).GetChild<CharacterMenu>(1).UpdatePlayerTeam();
		GetNode("MainMenu").GetChild(2).GetChild(1).GetChild<CombatPageSelect>(1).Refresh(LoadedCombatCards);

		GetNode("MainMenu").GetChild(2).GetChild(1).GetChild<CharPageSelect>(2).Refresh(LoadedCharacterCards);
		GetNode("MainMenu").GetChild(2).GetChild<BattleEditMenu>(4).UpdateCharacterCardList(LoadedCharacterCards);
		GetNode("MainMenu").GetChild(2).GetChild<BattleEditMenu>(4).UpdateCombatCardList(LoadedCombatCards);

		GetNode<MainMenu>("MainMenu").LoadMusic(LoadedMenuMusic);
		GetNode<MainMenu>("MainMenu").LoadSoundEffects();
	}

	private void SetUpBattle()
	{
		GetNode<BattleSceneManager>("BattleScene").InitiateBattle(SwitchToBattle, PlayerTeam);
		GetNode<BattleSceneManager>("BattleScene").LoadMusic(LoadedBattleMusic);
		GetNode<BattleSceneManager>("BattleScene").LoadSoundEffects();
	}
	
	public void Refresh()
	{
		GetChild<Node2D>(2).Show();
		RefreshTicker = 2;
	}
	private void TrueRefresh()
	{
		RefreshTicker = 0;
		
		PreloadedImagePaths = ContentLoader.GetPngFiles(Root : ((SceneTree)Engine.GetMainLoop()).Root);
		PreloadedImages = ContentLoader.PreloadImages(PreloadedImagePaths, Root : ((SceneTree)Engine.GetMainLoop()).Root);
		
		LoadedSpriteSheetPaths = new List<string>();
		LoadedSpriteSheets = new List<SpriteFrames>();
		
		PlayerTeam = ContentLoader.LoadPlayerTeam("user://Content/PlayerTeamSave.txt");
		
		LoadedCombatCards = ContentLoader.LoadCombatCards(ContentLoader.GetTxtFilesContaining("==COMBATCARD==", Root : ((SceneTree)Engine.GetMainLoop()).Root));
		LoadedCharacterCards = ContentLoader.LoadCharacterCards(ContentLoader.GetTxtFilesContaining("==CHARACTERCARD==", Root : ((SceneTree)Engine.GetMainLoop()).Root));
		LoadedBattles = ContentLoader.LoadBattles(ContentLoader.GetTxtFilesContaining("==BATTLE==", Root : ((SceneTree)Engine.GetMainLoop()).Root));
		LoadedStatusEffects = ContentLoader.LoadStatusEffects(ContentLoader.GetTxtFilesContaining("==STATUSEFFECT==", Root : ((SceneTree)Engine.GetMainLoop()).Root));
		
		LoadedMenuMusic = ContentLoader.LoadAudioFiles(ContentLoader.GetAudioFilesContaining("MENU_"));
		LoadedBattleMusic = ContentLoader.LoadAudioFiles(ContentLoader.GetAudioFilesContaining("BATTLE_"));

		LoadSoundEffects();

		if (CurrentScene && SceneChangeTicker == 0) { UpdateMenu(); }
		
		if(SceneChangeTicker == 0) {GetChild<Node2D>(2).Hide();}
	}
	
	public void LoadSoundEffects()
	{
		//UI
		List<AudioStream> UI_Streams = ContentLoader.LoadAudioFiles(ContentLoader.GetAudioFilesFromTermList(ContentLoader.UIAudioFileNames));
		foreach (AudioStream TargetStream in UI_Streams)
		{
			switch (TargetStream.ResourceName)
			{
				case "UI_Click":
					GetChild(3).GetChild(0).GetChild<AudioStreamPlayer>(0).Stream = TargetStream;
					break;
			}
		}
	}
	
	public void SavePlayerTeam(string FilePath)
	{
		if (FileAccess.FileExists(FilePath)) //Removes any existing saved data
		{
			using var DataFile = FileAccess.Open(FilePath, FileAccess.ModeFlags.Write);
			DataFile.StoreString(null);
			DataFile.Flush();
		}
		foreach (Character TeamMember in PlayerTeam) { DataStorageManager.StoreDonTxt(FilePath, "CHARACTER", TeamMember.ConvertToDonTxt()); } //Saves the team
	}
	
	public void OpenSettings()
	{GetChild<Node2D>(0).Show();}
	public void CloseSettings()
	{GetChild<Node2D>(0).Hide();}
	
	
	public void StartBattle(Battle InBattle)
	{
		SwitchToBattle = InBattle;
		SwitchScene(false);
	}
	
	//Connection Inputs
}
