using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class MainMenu : Node2D
{
	int TrackInfoScrollSpeed = 25; //How fast the text for music track scrolls, in units per second
	int SongToBeChosen = 0; //Tracker for picking a new song when the list is updated
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LoadSoundEffects();
		LoadMusic();
		
		//Set page to the battle selection page
		_SubMenu_Chosen(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Scroll the music track info text
		if(GetChild(6).GetChild<Label>(0).Position.X > -(GetChild(6).GetChild<Label>(0).Size.X/2))
		{
			GetChild(6).GetChild<Label>(0).SetPosition(new Vector2(GetChild(6).GetChild<Label>(0).Position.X-TrackInfoScrollSpeed*(float)delta, 0));
		}
		else
		{
			GetChild(6).GetChild<Label>(0).SetPosition(new Vector2(-(GetChild(6).GetChild<Label>(0).Size.X/2)-(GetChild(6).GetChild<Label>(0).Position.X), 0));
		}
		
		
		//Picking a song after an update
		if(SongToBeChosen > 1) {SongToBeChosen--;}
		if(SongToBeChosen == 1)
		{
			//Choose a random menu music track and update the music track info label
			Random Rand = new Random();
			if(GetChild(5).GetChild(1).GetChildCount() > 0)
			{
				int AudioNum = Rand.Next(0, GetChild(5).GetChild(1).GetChildCount());
				GetChild(6).GetChild<Label>(0).Text = "Current Track: "+GetChild(5).GetChild(1).GetChild<AudioStreamPlayer>(AudioNum).Stream.ResourceName + String.Concat(Enumerable.Repeat("      Current Track: "+GetChild(5).GetChild(1).GetChild<AudioStreamPlayer>(AudioNum).Stream.ResourceName, 3));
				GetChild(5).GetChild(1).GetChild<AudioStreamPlayer>(AudioNum).Play();
			}
			SongToBeChosen = 0;
		}
	}
	
	//Custom internal functions
	public void LoadMusic(List<AudioStream> InputMusicList = null)
	{
		//Checks it actually has an input. If not, just pulls it from the manager manually
		if(InputMusicList == null)
		{InputMusicList = ((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).LoadedMenuMusic;}
		
		//Clears existing song nodes
		for(int Index = 0; Index < GetChild(5).GetChild(1).GetChildCount(); Index++)
		{GetChild(5).GetChild(1).GetChild(Index).QueueFree();}
		
		//Creates the new nodes
		for(int Index = 0; Index < InputMusicList.Count; Index++)
		{
			int MTF_Num = Index; //For some reason if I used index when connecting below, they all use final value of index, so I have to do this weird middleman thing
			AudioStreamPlayer AudioPlayerScene = new AudioStreamPlayer();
			AudioPlayerScene.Stream = InputMusicList[Index];
			AudioPlayerScene.Bus = "Music";
			AudioPlayerScene.Finished += () => _MusicTrack_Finished(MTF_Num);
			GetChild(5).GetChild(1).AddChild(AudioPlayerScene);
		}
		
		SongToBeChosen = 3;
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
					GetChild(5).GetChild(0).GetChild<AudioStreamPlayer>(0).Stream = TargetStream;
					break;
				case "Page_Turn":
					GetChild(5).GetChild(0).GetChild<AudioStreamPlayer>(1).Stream = TargetStream;
					break;
				case "Card_Select":
					GetChild(5).GetChild(0).GetChild<AudioStreamPlayer>(2).Stream = TargetStream;
					break;
				case "Card_Cancel":
					GetChild(5).GetChild(0).GetChild<AudioStreamPlayer>(3).Stream = TargetStream;
					break;
				case "UI_Save":
					GetChild(5).GetChild(0).GetChild<AudioStreamPlayer>(4).Stream = TargetStream;
					break;
				case "UI_Load":
					GetChild(5).GetChild(0).GetChild<AudioStreamPlayer>(5).Stream = TargetStream;
					break;
			}
		}
	}
	
	//Connection Inputs
	private void _SubMenu_Chosen(int MenuNum)
	{
		if (!GetChild(2).GetChild<Node2D>(MenuNum).Visible)
		{
			GetChild(5).GetChild(0).GetChild<AudioStreamPlayer>(1).Play();
			for (int Index = 0; Index < 5; Index++)
			{
				if (Index == MenuNum) { GetChild(2).GetChild<Node2D>(Index).Show(); GetChild(0).GetChild(3).GetChild<Node2D>(Index).Show(); }
				else { GetChild(2).GetChild<Node2D>(Index).Hide(); GetChild(0).GetChild(3).GetChild<Node2D>(Index).Hide(); }
			}
		}
	}
	
	//Music auto-changing
	private void _MusicTrack_Finished(int Index)
	{
		if(Index<GetChild(5).GetChild(1).GetChildCount()-1) //Checks track isn't the last one in the list
		{ //Plays next track in list and updates the track info label
			GetChild(5).GetChild(1).GetChild<AudioStreamPlayer>(Index+1).Play();
			GetChild(6).GetChild<Label>(0).Text = "Current Track: "+GetChild(5).GetChild(1).GetChild<AudioStreamPlayer>(Index+1).Stream.ResourceName + String.Concat(Enumerable.Repeat("      Current Track: "+GetChild(5).GetChild(1).GetChild<AudioStreamPlayer>(Index+1).Stream.ResourceName, 3));
		}
		else
		{ //Loops back to first track in list and updates the track info label
			GetChild(5).GetChild(1).GetChild<AudioStreamPlayer>(0).Play();
			GetChild(6).GetChild<Label>(0).Text = "Current Track: "+GetChild(5).GetChild(1).GetChild<AudioStreamPlayer>(0).Stream.ResourceName + String.Concat(Enumerable.Repeat("      Current Track: "+GetChild(5).GetChild(1).GetChild<AudioStreamPlayer>(0).Stream.ResourceName, 3));
		}
	}
	
	private void _On_Close_Game_Pressed()
	{
		GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
	}
	
	private void _On_Settings_Pressed()
	{
		GetParent<GameManager>().OpenSettings();
	}
	
	private void _On_Refresh_Pressed()
	{
		GetParent<GameManager>().SavePlayerTeam("user://Content/PlayerTeamSave.txt");
		GetParent<GameManager>().Refresh();
	}
	
	private void _CustomContentInfo_Pressed()
	{
		GetChild<Node2D>(3).Visible = !(GetChild<Node2D>(3).Visible);
	}
	
	private void _PassiveEditingInfo_Pressed()
	{
		GetChild<Node2D>(4).Visible = !(GetChild<Node2D>(4).Visible);
	}
}
