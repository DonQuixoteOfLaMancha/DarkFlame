using Godot;
using System;

public partial class Settings : Node2D
{	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	//Controls
	private void _On_Close_Pressed() {Hide();}
	
	private void _On_Master_Volume_Changed(float RawValue)
	{
		if(RawValue == 0) {AudioServer.SetBusMute(0, true);}
		else
		{
			AudioServer.SetBusMute(0, false);
			AudioServer.SetBusVolumeDb(0, Mathf.LinearToDb(RawValue));
		}
	}
	private void _On_SFX_Volume_Changed(float RawValue)
	{
		if(RawValue == 0) {AudioServer.SetBusMute(1, true);}
		else
		{
			AudioServer.SetBusMute(1, false);
			AudioServer.SetBusVolumeDb(1, Mathf.LinearToDb(RawValue));
		}
	}
	private void _On_Music_Volume_Changed(float RawValue)
	{
		if(RawValue == 0) {AudioServer.SetBusMute(2, true);}
		else
		{
			AudioServer.SetBusMute(2, false);
			AudioServer.SetBusVolumeDb(2, Mathf.LinearToDb(RawValue));
		}
	}
	
	
	private void _On_ExitBattle_Pressed()
	{
		GetParent<GameManager>().SwitchScene(true);
		Hide();
	}
}
