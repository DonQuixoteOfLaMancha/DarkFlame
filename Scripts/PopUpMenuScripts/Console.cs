using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Console : Node2D
{
	bool AutoOpen = false;
	string[] CMDLIST = {"Cmds - Lists all commands",
						"Close - Closes console",
						"ToggleAutoOpen - Toggles whether console opens when something is written to it",
						"Echo ____ - Echos the given text to console"};
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	// CIF
	
	public void WriteToConsole(string Output)
	{
		if(AutoOpen) {Show();}
		GetChild<TextEdit>(1).Text += "\n"+Output;
	}
	
	private void ListCommands()
	{
		for(int Index = 0; Index<CMDLIST.Length; Index++)
		{WriteToConsole(CMDLIST[Index]);}
	}
	
	// Connection Inputs
	private void _SendInput()
	{
		string InputString = GetChild<TextEdit>(3).Text;
		GetChild<TextEdit>(3).Text = "";
		switch(true)
		{
			case bool _ when Regex.IsMatch(InputString, @"Cmds"):
				ListCommands(); break;
			case bool _ when Regex.IsMatch(InputString, @"Close"): 
				Hide(); break;
			case bool _ when Regex.IsMatch(InputString, @"ToggleAutoOpen"): 
				AutoOpen = !AutoOpen; break;
			case bool _ when Regex.IsMatch(InputString, @"Echo .*"):
				WriteToConsole(InputString.Substring(5)); break;
			default: 
				WriteToConsole("'"+InputString+"' is not a valid command."); break;
		}
	}
}
