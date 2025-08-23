using Godot;
using System;

public static class InformationConversionUtils
{
	public static string ConditionalToText(string Trigger, string[] Condition, string[] Effect)
	{
		string ConditionalText = "";
		switch(Trigger)
		{
			case "BATTLESTART": ConditionalText += "Battle Start: "; break;
			case "ENCOUNTERSTART": ConditionalText += "Encounter Start: "; break;
			case "TURNSTART": ConditionalText += "Turn Start: "; break;
			case "COMBATSTART": ConditionalText += "Combat Start: "; break;
			case "COMBATEND": ConditionalText += "Combat End: "; break;
			case "SONGEND": ConditionalText += "Song End: "; break;
			case "ONUSE": ConditionalText += "On Use: "; break;
			case "ONROLL": ConditionalText += "On Roll: "; break;
			case "ONHIT": ConditionalText += "On Hit: "; break;
			case "ONDAMAGED": ConditionalText += "On Damaged: "; break;
			case "CLASHLOSE": ConditionalText += "On Clash Lose: "; break;
			case "CLASHWIN": ConditionalText += "On Clash Win: "; break;
			case "CLASHDRAW": ConditionalText += "On Clash Draw: "; break;
			case "EMOTIONLVLCHANGE": ConditionalText += "When Emotion Level Increases: "; break;
			case "ONCARDUSE": ConditionalText += "When Using a Card: "; break;
			case "ONALLYDEATH": ConditionalText += "On Ally Death: "; break;
			case "ONENEMYDEATH": ConditionalText += "On Enemy Death: "; break;
			default: return "";
		}
		ConditionalText += "If ";
		switch(Condition[0])
		{
			case "Roll": ConditionalText += "roll "; break;
			case "User": ConditionalText += "own "; break;
			case "Target": ConditionalText += "target "; break;
		}
		if(Condition[0] != "Roll")
		{
			switch(Condition[1])
			{
				case "Health": ConditionalText += "health "; break;
				case "StaggerHealth": ConditionalText += "stagger "; break;
				case "Stamina": ConditionalText += "stamina "; break;
				case "MaxHealth": ConditionalText+= "max health "; break;
				case "MaxStaggerHealth": ConditionalText+= "max stagger "; break;
				case "MaxStamina": ConditionalText+= "max stamina "; break;
				case "Hand": ConditionalText+= "hand size "; break;
				case "SpeedDie": ConditionalText += "speed die count "; break;
				case "EmotionLvl": ConditionalText += "emotion level "; break;
				case "BattleUsedCards":  ConditionalText += "cards used in battle "; break;
				case "EncounterUsedCards":  ConditionalText += "cards used in encounter "; break;
				case "UniqueUsedCards":  ConditionalText += "unique cards used "; break;
				case "Singleton": ConditionalText += "is singleton "; break;
				case "DiceType": ConditionalText += "dice type "; break;
				default:
					if(Condition[1].Length>6 && Condition[1].Substring(0, 7) == "Status.")
					{ConditionalText += Condition[1].Substring(7)+" ";}
					break;
			}
		}
		if(Condition[1] != "Singleton")
		{
			switch(Condition[2])
			{
				case "Eq": ConditionalText += "= "; break;
				case "Lt": ConditionalText += "< "; break;
				case "Gt": ConditionalText += "> "; break;
				case "Neq": ConditionalText += "≠ "; break;
			}
			switch(Condition[3])
			{
				case "Health": ConditionalText += "health "; break;
				case "StaggerHealth": ConditionalText += "stagger "; break;
				case "Stamina": ConditionalText += "stamina "; break;
				case "MaxHealth": ConditionalText+= "max health "; break;
				case "MaxStaggerHealth": ConditionalText+= "max stagger "; break;
				case "MaxStamina": ConditionalText+= "max stamina "; break;
				case "Hand": ConditionalText+= "hand size "; break;
				case "SpeedDie": ConditionalText += "speed die count "; break;
				case "EmotionLvl": ConditionalText += "emotion level "; break;
				case "BattleUsedCards":  ConditionalText += "cards used in battle "; break;
				case "EncounterUsedCards":  ConditionalText += "cards used in encounter "; break;
				case "UniqueUsedCards":  ConditionalText += "unique cards used "; break;
				default: ConditionalText += Condition[3]; break;
			}
		}
		ConditionalText += ": ";
		
		if(Effect[0] == "Roll") //Effect is a roll change
		{
			switch(Effect[2])
			{
				case "Inc": ConditionalText += "Increase roll by ("; break;
				case "Dec": ConditionalText += "Decrease roll by ("; break;
				case "Set": ConditionalText += "Set roll to ("; break;
				case "Mlt": ConditionalText += "Multiply roll by ("; break;
				case "Div": ConditionalText += "Divide roll by ("; break;
			}
			
			if(Effect[3].Length>6 && Effect[3].Substring(0, 7) == "Status.")
			{ConditionalText += Effect[3].Substring(7);}
			else {ConditionalText += Effect[3];}
			
			ConditionalText += ").";
		}
		else //Effect involves user or target
		{
			if(Effect[1] == "Draw" || Effect[1] == "Discard") //Effect is a draw or discard effect
			{
				if(Effect[0] == "Target")
				{
					ConditionalText += "Target ";
					if(Effect[1] == "Draw") {ConditionalText += "draws (";}
					else {ConditionalText += "discards (";}
				}
				else{ConditionalText += Effect[1]+" (";}
				
				if(Effect[3].Length>6 && Effect[3].Substring(0, 7) == "Status.")
				{ConditionalText += Effect[3].Substring(7);}
				else {ConditionalText += Effect[3];}
				
				ConditionalText += ") cards.";
			}
			else if(Effect[1] == "ResetUniqueCardTracker") //Effect is reset unique cards used
			{
				if(Effect[0] == "Target") {ConditionalText += "Reset targets unique card tracker.";}
				else {ConditionalText += "Reset unique card tracker.";}
			}
			else //Effect involves a character stat or status effect
			{
				switch(Effect[2])
				{
					case "Inc": ConditionalText += "Increase "; break;
					case "Dec": ConditionalText += "Decrease "; break;
					case "Set": ConditionalText += "Set "; break;
					case "Mlt": ConditionalText += "Multiply "; break;
					case "Div": ConditionalText += "Divide "; break;
				}
				
				if(Effect[0] == "Target") {ConditionalText += "target ";}
				else {ConditionalText += "own ";}
				
				switch(Effect[1])
				{
					case "Health": ConditionalText += "health "; break;
					case "StaggerHealth": ConditionalText += "stagger "; break;
					case "Stamina": ConditionalText += "stamina "; break;
					case "MaxHealth": ConditionalText += "max health "; break;
					case "MaxStaggerHealth": ConditionalText += "max stagger "; break;
					case "MaxStamina": ConditionalText += "max stamina "; break;
					case "SpeedDie": ConditionalText += "speed die count "; break;
					case "EmotionPoints": ConditionalText += "emotion points "; break;
					case "BattleUsedCards":  ConditionalText += "cards used in battle tracker "; break;
					case "EncounterUsedCards":  ConditionalText += "cards used in encounter tracker "; break;
					default:
						if(Effect[1].Length>6 && Effect[1].Substring(0, 7) == "Status.")
						{ConditionalText += Effect[1].Substring(7)+" ";}
						break;
				}
				
				if(Effect[2] == "Set") {ConditionalText += "to (";}
				else {ConditionalText += "by (";}
				
				if(Effect[3].Length>6 && Effect[3].Substring(0, 7) == "Status.")
				{ConditionalText += Effect[3].Substring(7);}
				else {ConditionalText += Effect[3];}
				
				ConditionalText += ").";
			}
		}
		
		if(ConditionalText.Contains("If own health > 0: ")) {ConditionalText = ConditionalText.Replace("If own health > 0: ", "");}
		
		return ConditionalText;
	}
}
