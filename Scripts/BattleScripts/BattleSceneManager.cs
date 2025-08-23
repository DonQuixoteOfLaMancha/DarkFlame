using Godot;
using System;
using System.Collections.Generic;

public partial class BattleSceneManager : Node2D
{
	Random Rand = new Random();
	
	Battle ActiveBattle;
	public int EncounterIndex = 0;
	Character[] PlayerTeam;
	Character[] EnemyTeam;
	List<SpeedDice> SpeedDieList = new List<SpeedDice>();
	
	List<BaseButton> InputNodeList; //To make disabling and re-enabling inputs easier
	
	int ProselyteTargetIndex = -1; //Character index used for a specific targeting type
	
	public bool Skip = false; //Used for skipping all the timer stuff in combat phase, mainly intended for me to use to get to stuff faster
	
	public int SongToBeChosen = 0;
	
	//Encounter Proceed and Battle End variables
	public bool ProceedToNextEncounter = false;
	public double NextEncounterSleepTimer = 0.0;
	
	
	//Info provision variables
	Character InfoTarget;
	
	//Dice setting variables
	bool CardBeingSet = false;
	SpeedDice SettingDice;
	
	//General Turn Play Variables
	bool TriggerNextSpeedDice = false;
	
	//Character Movement Variables
	Character MoveChar_CharacterLeft;
	Character MoveChar_CharacterRight;
	int MoveChar_CharacterLeft_TargetX = 0;
	int MoveChar_CharacterRight_TargetX = 0;
	int MoveChar_TargetY = 0;
	double MoveChar_TimeRemaining = 0.0;
	bool MoveChar_Active = false;
	
	//Character Battling Variables
	Character BattleChar_Attacker;
	Character BattleChar_Defender;
	SpeedDice BattleChar_Attacker_Dice;
	SpeedDice BattleChar_Defender_Dice;
	public int BattleChar_Index = 0;
	public int BattleChar_DamageDealt = 0;
	double BattleChar_SleepTime = 0.0;
	bool BattleChar_Active = false;
	bool BattleChar_DiceSpun = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ActiveBattle = new Battle();

		PlayerTeam = new Character[5]{GetChild(4).GetChild(1).GetChild<Character>(0),
										GetChild(4).GetChild(1).GetChild<Character>(1),
										GetChild(4).GetChild(1).GetChild<Character>(2),
										GetChild(4).GetChild(1).GetChild<Character>(3),
										GetChild(4).GetChild(1).GetChild<Character>(4)};
		EnemyTeam = new Character[5]{GetChild(4).GetChild(0).GetChild<Character>(0),
										GetChild(4).GetChild(0).GetChild<Character>(1),
										GetChild(4).GetChild(0).GetChild<Character>(2),
										GetChild(4).GetChild(0).GetChild<Character>(3),
										GetChild(4).GetChild(0).GetChild<Character>(4)};

		InputNodeList = new List<BaseButton>(){GetChild(1).GetChild<BaseButton>(3), //Stores a reference to every button in the scene (excl. speed die)
												GetChild(2).GetChild<BaseButton>(2),
												GetChild(3).GetChild<BaseButton>(2),
												GetChild(4).GetChild(0).GetChild(0).GetChild<BaseButton>(6), //Start Chars
												GetChild(4).GetChild(0).GetChild(1).GetChild<BaseButton>(6),
												GetChild(4).GetChild(0).GetChild(2).GetChild<BaseButton>(6),
												GetChild(4).GetChild(0).GetChild(3).GetChild<BaseButton>(6),
												GetChild(4).GetChild(0).GetChild(4).GetChild<BaseButton>(6),
												GetChild(4).GetChild(1).GetChild(0).GetChild<BaseButton>(6),
												GetChild(4).GetChild(1).GetChild(1).GetChild<BaseButton>(6),
												GetChild(4).GetChild(1).GetChild(2).GetChild<BaseButton>(6),
												GetChild(4).GetChild(1).GetChild(3).GetChild<BaseButton>(6),
												GetChild(4).GetChild(1).GetChild(4).GetChild<BaseButton>(6) //End Chars
												};

		LoadSoundEffects();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Skip)
		{
			NextEncounterSleepTimer = 0;
			MoveChar_TimeRemaining = 0;
			if(BattleChar_Attacker_Dice != null) {BattleChar_Attacker_Dice.RollTimer = 0;}
			if(BattleChar_Defender_Dice != null) {BattleChar_Defender_Dice.RollTimer = 0;}
			if(BattleChar_SleepTime>0.1) {BattleChar_SleepTime = 0.1;}
		}
		
		if(ProceedToNextEncounter) //Plays either the encounter proceed animation or the battle end animation
		{
			if(NextEncounterSleepTimer <= 0.0)
			{
				ProceedToNextEncounter = false;
				NextEncounterSleepTimer = 0;
				if(EncounterIndex < ActiveBattle.Encounters.Count) {InitiateEncounter();}
				else {ExitBattle();}
			}
			else
			{
				NextEncounterSleepTimer -= delta;
				if(NextEncounterSleepTimer < 0.0) {NextEncounterSleepTimer = 0;}
			}
		}
		
		if(TriggerNextSpeedDice) //Main turn play process
		{
			if(SpeedDieList.Count > 0)
			{
				try
				{
					if(SpeedDieList[0].SlotCard != null && SpeedDieList[0].Target != null && SpeedDieList[0].Target.Health > 0 && SpeedDieList[0].GetParent().GetParent().GetParent<Character>().Health > 0)
					{
						SpeedDieList[0].Show();
						BattleChar_Attacker = SpeedDieList[0].GetParent().GetParent().GetParent<Character>();
						BattleChar_Defender = SpeedDieList[0].Target;
						BattleChar_Attacker_Dice = SpeedDieList[0];
						for(int Index = 0; Index < BattleChar_Defender.GetChild(5).GetChild(2).GetChildCount(); Index++)
						{
							if(BattleChar_Defender.GetChild(5).GetChild(2).GetChild<SpeedDice>(Index).SlotCard != null)
							{
								if(BattleChar_Defender_Dice != null)
								{
									if(BattleChar_Defender_Dice.Value < BattleChar_Defender.GetChild(5).GetChild(2).GetChild<SpeedDice>(Index).Value)
									{
										BattleChar_Defender_Dice.Hide();
										BattleChar_Defender_Dice = BattleChar_Defender.GetChild(5).GetChild(2).GetChild<SpeedDice>(Index);
									}
								}
								else {BattleChar_Defender_Dice = BattleChar_Defender.GetChild(5).GetChild(2).GetChild<SpeedDice>(Index);}
								BattleChar_Defender_Dice.Show();
							}
						}
						BattleChar_Attacker_Dice.Show();
						BattleChar_Attacker.SetModulate(new Color(1,1,1,1));
						BattleChar_Defender.SetModulate(new Color(1,1,1,1));
						BattleChar_Attacker.ZIndex = 0;
						BattleChar_Defender.ZIndex = 0;
						MoveCharsToApproach(BattleChar_Attacker, BattleChar_Defender);
						TriggerNextSpeedDice = false;
					}
					else if(SpeedDieList[0].SlotCard != null) {SpeedDieList[0].QueueFree();}
				}
				catch{}
				SpeedDieList.RemoveAt(0);
			}
			else
			{
				TriggerNextSpeedDice = false;
				TriggerPhaseConditionals("COMBATEND");
				StartTurn();
			}
		}
		
		//MoveChar Process
		else if(MoveChar_Active)
		{
			MoveChar_CharacterLeft.SetPosition(new Vector2(MoveChar_CharacterLeft.Position.X + (MoveChar_CharacterLeft_TargetX-MoveChar_CharacterLeft.Position.X)*(float)(delta/MoveChar_TimeRemaining),
															MoveChar_CharacterLeft.Position.Y + (MoveChar_TargetY-MoveChar_CharacterLeft.Position.Y)*(float)(delta/MoveChar_TimeRemaining)));
			MoveChar_CharacterRight.SetPosition(new Vector2(MoveChar_CharacterRight.Position.X + (MoveChar_CharacterRight_TargetX-MoveChar_CharacterRight.Position.X)*(float)(delta/MoveChar_TimeRemaining),
															MoveChar_CharacterRight.Position.Y + (MoveChar_TargetY-MoveChar_CharacterRight.Position.Y)*(float)(delta/MoveChar_TimeRemaining)));	
			
			
			MoveChar_TimeRemaining -= delta;
			if(MoveChar_TimeRemaining <= 0)
			{
				MoveChar_TimeRemaining = 0.0;
				MoveChar_CharacterLeft.SetPosition(new Vector2(MoveChar_CharacterLeft_TargetX, MoveChar_TargetY));
				MoveChar_CharacterRight.SetPosition(new Vector2(MoveChar_CharacterRight_TargetX, MoveChar_TargetY));
				
				BattleChar_Attacker.CardsUsedBattle++;
				BattleChar_Attacker.CardsUsedEncounter++;
				if(BattleChar_Attacker.UniqueCardsUsed.FindIndex(x => x.Name.Equals(BattleChar_Attacker_Dice.SlotCard.Name)) == -1)
				{BattleChar_Attacker.UniqueCardsUsed.Add(BattleChar_Attacker_Dice.SlotCard);}
				if(BattleChar_Attacker_Dice.SlotCard.ConditionalTrigger == "ONUSE")
				{TriggerConditional(BattleChar_Attacker_Dice.SlotCard, BattleChar_Attacker);}
				BattleChar_Attacker.TriggerPassivesAndStatuses("ONCARDUSE");
					
				if(BattleChar_Defender_Dice != null)
				{
					BattleChar_Defender.CardsUsedBattle++;
					BattleChar_Defender.CardsUsedEncounter++;
					if(BattleChar_Defender.UniqueCardsUsed.FindIndex(x => x.Name.Equals(BattleChar_Defender_Dice.SlotCard.Name)) == -1)
					{BattleChar_Defender.UniqueCardsUsed.Add(BattleChar_Defender_Dice.SlotCard);}
					if(BattleChar_Defender_Dice.SlotCard.ConditionalTrigger == "ONUSE")
					{TriggerConditional(BattleChar_Defender_Dice.SlotCard, BattleChar_Defender);}
					BattleChar_Defender.TriggerPassivesAndStatuses("ONCARDUSE");
				}
				
				MoveChar_Active = false;
				BattleChar_Active = true;
			}
		}
		
		//BattleChar Process
		else if(BattleChar_Active)
		{
			if(BattleChar_SleepTime <= 0)
			{
				if(!BattleChar_DiceSpun)
				{
					if(BattleChar_Defender_Dice != null && BattleChar_Defender.Health > 0 && BattleChar_Attacker.Health > 0)
					{
						if(BattleChar_Index < BattleChar_Attacker_Dice.SlotCard.Dice.Count || BattleChar_Index < BattleChar_Defender_Dice.SlotCard.Dice.Count)
						{
							if(BattleChar_Index < BattleChar_Attacker_Dice.SlotCard.Dice.Count && BattleChar_Attacker.StaggerHealth > 0)
							{
								if(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType < 5)
								{
									BattleChar_Attacker.UpdateSpriteState(3+BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType);
									GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType).Play();
								}
								else {BattleChar_Attacker.UpdateSpriteState(3);}
							}
							if(BattleChar_Index < BattleChar_Defender_Dice.SlotCard.Dice.Count && BattleChar_Defender.StaggerHealth > 0)
							{
								if(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].DiceType < 5)
								{
									BattleChar_Defender.UpdateSpriteState(3+BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].DiceType);
									GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].DiceType).Play();
								}
								else {BattleChar_Defender.UpdateSpriteState(3);}
							}
							BattleChar_Attacker_Dice.RollIndexDice(BattleChar_Index);
							BattleChar_Defender_Dice.RollIndexDice(BattleChar_Index);
							BattleChar_SleepTime = 3.5; //Time period of dice roll, needs changing along with the num used in speed die
							BattleChar_DiceSpun = true;
						}
						else {BattleChar_EndClash();}
					}
					else
					{
						if(BattleChar_Index < BattleChar_Attacker_Dice.SlotCard.Dice.Count && BattleChar_Defender.Health > 0 && BattleChar_Attacker.Health > 0 && BattleChar_Attacker.StaggerHealth>0)
						{
							if(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType < 5)
							{
								BattleChar_Attacker.UpdateSpriteState(3+BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType);
								GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType).Play();
							}
							else {BattleChar_Attacker.UpdateSpriteState(3);}
							BattleChar_Attacker_Dice.RollIndexDice(BattleChar_Index);
							BattleChar_SleepTime = 3.5; //Time period of dice roll, needs changing along with the num used in speed die
							BattleChar_DiceSpun = true;
						}
						else {BattleChar_EndClash();}
					}
				}
				else
				{
						//Add emotion points for min or max rolls, and trigger ONROLL conditionals
					if(BattleChar_Index < BattleChar_Attacker_Dice.SlotCard.Dice.Count)
					{
						if(BattleChar_Attacker_Dice.Value == BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].MinRoll || BattleChar_Attacker_Dice.Value == BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].MaxRoll)
						{BattleChar_Attacker.EmotionPoints++;}
						if(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].ConditionalTrigger == "ONROLL")
						{TriggerConditional(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index], BattleChar_Attacker, BattleChar_Attacker_Dice);}
						BattleChar_Attacker.TriggerPassivesAndStatuses("ONROLL", BattleChar_Attacker_Dice);
					}
					if(BattleChar_Defender_Dice != null)
					{
						if(BattleChar_Index < BattleChar_Defender_Dice.SlotCard.Dice.Count)
						{
							if(BattleChar_Defender_Dice.Value == BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].MinRoll || BattleChar_Defender_Dice.Value == BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].MaxRoll)
							{BattleChar_Defender.EmotionPoints++;}
							if(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].ConditionalTrigger == "ONROLL")
							{TriggerConditional(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index], BattleChar_Defender, BattleChar_Defender_Dice);}
							BattleChar_Defender.TriggerPassivesAndStatuses("ONROLL", BattleChar_Defender_Dice);
						}
					}
					
					if(BattleChar_Attacker_Dice.Value == -1984) //Attacker has no more dice
					{
						//Sounds
						if(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].DiceType<5)
						{GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].DiceType).Play();}
						GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(5).Play();
						//Emotion points for taking a one-sided
						BattleChar_Attacker.EmotionPoints++;
						//Sprite
						BattleChar_Attacker.UpdateSpriteState(2);
						//Damage
						BattleChar_DamageDealt = BattleChar_Attacker.Damage(BattleChar_Defender_Dice.Value, BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].DiceType);
						//Conditionals
						BattleChar_Defender.TriggerPassivesAndStatuses("ONHIT", BattleChar_Defender_Dice);
						if(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].ConditionalTrigger == "ONHIT" && BattleChar_DamageDealt > 0)
						{TriggerConditional(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index], BattleChar_Defender, BattleChar_Defender_Dice);}
						if(BattleChar_DamageDealt > 0)
						{BattleChar_Attacker.TriggerPassivesAndStatuses("ONDAMAGED", BattleChar_Attacker_Dice);}
					}
					else if(BattleChar_Defender_Dice == null) //Defender has no dice
					{
						//Sounds
						if(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType<5)
						{GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType).Play();}
						GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(5).Play();
						//Emotion points for taking a one-sided
						BattleChar_Defender.EmotionPoints++;
						//Sprite
						BattleChar_Defender.UpdateSpriteState(2);
						//Damage
						BattleChar_DamageDealt = BattleChar_Defender.Damage(BattleChar_Attacker_Dice.Value, BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType);
						//Conditionals
						BattleChar_Attacker.TriggerPassivesAndStatuses("ONHIT", BattleChar_Attacker_Dice);
						if(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].ConditionalTrigger == "ONHIT" && BattleChar_DamageDealt > 0)
						{TriggerConditional(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index], BattleChar_Attacker, BattleChar_Attacker_Dice);}
						if(BattleChar_DamageDealt > 0)
						{BattleChar_Defender.TriggerPassivesAndStatuses("ONDAMAGED");}
					}
					else if(BattleChar_Defender_Dice.Value == -1984) //Defender ran out of dice
					{
						//Sounds
						if(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType<5)
						{GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType).Play();}
						GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(5).Play();
						//Emotion points for taking a one-sided
						BattleChar_Defender.EmotionPoints++;
						//Sprite
						BattleChar_Defender.UpdateSpriteState(2);
						//Damage
						BattleChar_DamageDealt = BattleChar_Defender.Damage(BattleChar_Attacker_Dice.Value, BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType);
						//Conditionals
						BattleChar_Attacker.TriggerPassivesAndStatuses("ONHIT", BattleChar_Attacker_Dice);
						if(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].ConditionalTrigger == "ONHIT" && BattleChar_DamageDealt > 0)
						{TriggerConditional(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index], BattleChar_Attacker, BattleChar_Attacker_Dice);}
						if(BattleChar_DamageDealt > 0)
						{BattleChar_Defender.TriggerPassivesAndStatuses("ONDAMAGED");}
					}
					else
					{
						//Emotion Points for clash
						BattleChar_Attacker.EmotionPoints++;
						BattleChar_Defender.EmotionPoints++;
						
						//Sounds
						if(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType<5)
						{GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType).Play();}
						if(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].DiceType<5)
						{GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].DiceType).Play();}
						
						if(BattleChar_Attacker_Dice.Value < BattleChar_Defender_Dice.Value) //Defender wins clash
						{
							//Damage Sound
							GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(5).Play();
							//Sprite
							BattleChar_Attacker.UpdateSpriteState(2);
							//Damage
							BattleChar_DamageDealt = BattleChar_Attacker.Damage(BattleChar_Defender_Dice.Value, BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].DiceType);
							//Evade Re-use and heal
							if(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].DiceType == 4)
							{
								BattleChar_Defender_Dice.SlotCard.Dice.Add(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index]);
								BattleChar_Defender.StaggerHealth += BattleChar_Attacker_Dice.Value;
								if(BattleChar_Defender.StaggerHealth <= 0) {BattleChar_Defender.StaggerHealth = 0; BattleChar_Defender.UpdateSpriteState(2);}
								else if(BattleChar_Defender.StaggerHealth > BattleChar_Defender.MaxStaggerHealth) {BattleChar_Defender.StaggerHealth = BattleChar_Defender.MaxStaggerHealth;}
								BattleChar_Defender.GetChild(5).GetChild(1).GetChild<ProgressBar>(1).Value = (float)BattleChar_Defender.StaggerHealth/(float)BattleChar_Defender.MaxStaggerHealth;
							}
							//Conditionals
							if(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].ConditionalTrigger == "CLASHLOSE")
							{TriggerConditional(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index], BattleChar_Attacker, BattleChar_Attacker_Dice);}
							if(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].ConditionalTrigger == "CLASHWIN" || (BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].ConditionalTrigger == "ONHIT" && BattleChar_DamageDealt > 0))
							{TriggerConditional(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index], BattleChar_Defender, BattleChar_Defender_Dice);}
							if(BattleChar_DamageDealt > 0) {BattleChar_Attacker.TriggerPassivesAndStatuses("ONDAMAGED", BattleChar_Attacker_Dice);}
							BattleChar_Attacker.TriggerPassivesAndStatuses("CLASHLOSE", BattleChar_Attacker_Dice);
							BattleChar_Defender.TriggerPassivesAndStatuses("CLASHWIN", BattleChar_Defender_Dice);
						}
						else if(BattleChar_Defender_Dice.Value < BattleChar_Attacker_Dice.Value) //Attacker wins clash
						{
							//Damage Sound
							GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(5).Play();
							//Sprite
							BattleChar_Defender.UpdateSpriteState(2);
							//Damage
							BattleChar_DamageDealt = BattleChar_Defender.Damage(BattleChar_Attacker_Dice.Value, BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType);
							//Evade re-use and heal
							if(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].DiceType == 4)
							{
								BattleChar_Attacker_Dice.SlotCard.Dice.Add(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index]);
								BattleChar_Attacker.StaggerHealth += BattleChar_Attacker_Dice.Value;
								if(BattleChar_Attacker.StaggerHealth <= 0) {BattleChar_Attacker.StaggerHealth = 0; BattleChar_Attacker.UpdateSpriteState(2);}
								else if(BattleChar_Attacker.StaggerHealth > BattleChar_Attacker.MaxStaggerHealth) {BattleChar_Attacker.StaggerHealth = BattleChar_Attacker.MaxStaggerHealth;}
								BattleChar_Attacker.GetChild(5).GetChild(1).GetChild<ProgressBar>(1).Value = (float)BattleChar_Attacker.StaggerHealth/(float)BattleChar_Attacker.MaxStaggerHealth;
							}
							//Conditionals
							if(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].ConditionalTrigger == "CLASHWIN" || (BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].ConditionalTrigger == "ONHIT" && BattleChar_DamageDealt > 0))
							{TriggerConditional(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index], BattleChar_Attacker, BattleChar_Attacker_Dice);}
							if(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].ConditionalTrigger == "CLASHLOSE")
							{TriggerConditional(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index], BattleChar_Defender, BattleChar_Defender_Dice);}
							if(BattleChar_DamageDealt > 0) {BattleChar_Defender.TriggerPassivesAndStatuses("ONDAMAGED", BattleChar_Defender_Dice);}
							BattleChar_Attacker.TriggerPassivesAndStatuses("CLASHWIN", BattleChar_Attacker_Dice);
							BattleChar_Defender.TriggerPassivesAndStatuses("CLASHLOSE", BattleChar_Defender_Dice);
						}
						else //Draw
						{
							if(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index].ConditionalTrigger == "CLASHDRAW")
							{TriggerConditional(BattleChar_Attacker_Dice.SlotCard.Dice[BattleChar_Index], BattleChar_Attacker, BattleChar_Attacker_Dice);}
							if(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index].ConditionalTrigger == "CLASHDRAW")
							{TriggerConditional(BattleChar_Defender_Dice.SlotCard.Dice[BattleChar_Index], BattleChar_Defender, BattleChar_Defender_Dice);}
							BattleChar_Attacker.TriggerPassivesAndStatuses("CLASHDRAW");
							BattleChar_Defender.TriggerPassivesAndStatuses("CLASHDRAW");
						}
					}
					BattleChar_Index++;
					BattleChar_DiceSpun = false;
					BattleChar_SleepTime = 1.0;
				}
			}
			else
			{
				BattleChar_SleepTime -= delta;
				if(BattleChar_SleepTime < 0) {BattleChar_SleepTime = 0;}
			}
		}
		
		//Music picker
		if(SongToBeChosen > 1) {SongToBeChosen--;}
		if(SongToBeChosen == 1)
		{
			if(GetChild(10).GetChild(3).GetChildCount() > 0)
			{
				int AudioNum = Rand.Next(0, GetChild(10).GetChild(3).GetChildCount());
				GetChild<Label>(11).Text = "Current Track: "+GetChild(10).GetChild(3).GetChild<AudioStreamPlayer>(AudioNum).Stream.ResourceName;
				GetChild(10).GetChild(3).GetChild<AudioStreamPlayer>(AudioNum).Play();
			}
			SongToBeChosen = 0;
		}
	}
	
	//CIF
	public void LoadMusic(List<AudioStream> InputMusicList = null)
	{
		//Checks it actually has an input. If not, just pulls it from the manager manually
		if(InputMusicList == null)
		{InputMusicList = ((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).LoadedBattleMusic;}
		
		//Clears existing song nodes
		for(int Index = 0; Index < GetChild(10).GetChild(3).GetChildCount(); Index++)
		{GetChild(10).GetChild(3).GetChild(Index).QueueFree();}
		
		//Creates the new nodes
		for(int Index = 0; Index < InputMusicList.Count; Index++)
		{
			int MTF_Num = Index; //For some reason if I used index when connecting below, they all use final value of index, so I have to do this weird middleman thing
			AudioStreamPlayer AudioPlayerScene = new AudioStreamPlayer();
			AudioPlayerScene.Stream = InputMusicList[Index];
			AudioPlayerScene.Bus = "Music";
			AudioPlayerScene.Finished += () => _MusicTrack_Finished(MTF_Num);
			GetChild(10).GetChild(3).AddChild(AudioPlayerScene);
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
					GetChild(10).GetChild(0).GetChild<AudioStreamPlayer>(0).Stream = TargetStream;
					break;
				case "Card_Select":
					GetChild(10).GetChild(0).GetChild<AudioStreamPlayer>(1).Stream = TargetStream;
					break;
				case "Card_Cancel":
					GetChild(10).GetChild(0).GetChild<AudioStreamPlayer>(2).Stream = TargetStream;
					break;
			}
		}

		//Phase
		List<AudioStream> Phase_Streams = ContentLoader.LoadAudioFiles(ContentLoader.GetAudioFilesFromTermList(ContentLoader.PhaseAudioFileNames));
		foreach (AudioStream TargetStream in Phase_Streams)
		{
			switch (TargetStream.ResourceName)
			{
				case "EncounterStart":
					GetChild(10).GetChild(1).GetChild<AudioStreamPlayer>(0).Stream = TargetStream;
					break;
				case "CombatStart":
					GetChild(10).GetChild(1).GetChild<AudioStreamPlayer>(1).Stream = TargetStream;
					break;
				case "BattleLost":
					GetChild(10).GetChild(1).GetChild<AudioStreamPlayer>(2).Stream = TargetStream;
					break;
				case "BattleWon":
					GetChild(10).GetChild(1).GetChild<AudioStreamPlayer>(3).Stream = TargetStream;
					break;
			}
		}
		
		//Clash
		List<AudioStream> Clash_Streams = ContentLoader.LoadAudioFiles(ContentLoader.GetAudioFilesFromTermList(ContentLoader.ClashAudioFileNames));
		foreach (AudioStream TargetStream in Clash_Streams)
		{
			switch (TargetStream.ResourceName)
			{
				case "Clash_Slash":
					GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(0).Stream = TargetStream;
					break;
				case "Clash_Pierce":
					GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(1).Stream = TargetStream;
					break;
				case "Clash_Blunt":
					GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(2).Stream = TargetStream;
					break;
				case "Clash_Block":
					GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(3).Stream = TargetStream;
					break;
				case "Clash_Evade":
					GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(4).Stream = TargetStream;
					break;
				case "Clash_Damage":
					GetChild(10).GetChild(2).GetChild<AudioStreamPlayer>(5).Stream = TargetStream;
					break;
			}
		}
	}
	
	
	public void InitiateBattle(Battle SetBattle, Character[] SetPTeam)
	{
		ActiveBattle = SetBattle;
		for (int Index = 0; Index < 5; Index++)
		{
			PlayerTeam[Index].CopyCharacter(SetPTeam[Index]);
			PlayerTeam[Index].Reset();
		}

		EncounterIndex = 0;
		InitiateEncounter();
	}
	
	private void InitiateEncounter()
	{
		GetChild(10).GetChild(1).GetChild<AudioStreamPlayer>(0).Play();
		
		InfoTarget = null;
		GetChild<Node2D>(2).Hide();
		GetChild<Node2D>(8).Hide();
		
		GetChild<Node2D>(9).Hide();
		GetChild<Sprite2D>(0).Texture = ActiveBattle.Encounters[EncounterIndex].BackGroundImage;
		if(GetChild<Sprite2D>(0).Texture != null) {GetChild<Sprite2D>(0).Scale = new Vector2((float)1920/GetChild<Sprite2D>(0).Texture.GetWidth(), (float)1080/GetChild<Sprite2D>(0).Texture.GetHeight());}
		
		for(int Index = 0; Index<5; Index++)
		{
			EnemyTeam[Index].CopyCharacter(ActiveBattle.Encounters[EncounterIndex].Enemies[Index]);
			EnemyTeam[Index].Reset();
			
			PlayerTeam[Index].ActiveDeck.Clear();
			EnemyTeam[Index].ActiveDeck.Clear();
			PlayerTeam[Index].Hand.Clear();
			EnemyTeam[Index].Hand.Clear();
			for(int CardIndex = 0; CardIndex<9; CardIndex++)
			{
				if(PlayerTeam[Index].Deck[CardIndex] != null)
				{PlayerTeam[Index].ActiveDeck.Add(PlayerTeam[Index].Deck[CardIndex]);}
				if(EnemyTeam[Index].Deck[CardIndex] != null)
				{EnemyTeam[Index].ActiveDeck.Add(EnemyTeam[Index].Deck[CardIndex]);}
			}
			PlayerTeam[Index].DrawCard(5);
			EnemyTeam[Index].DrawCard(5);
			
			PlayerTeam[Index].CardsUsedEncounter = 0;
		}
		if(EncounterIndex == 0) {TriggerPhaseConditionals("BATTLESTART");};
		TriggerPhaseConditionals("ENCOUNTERSTART");
		StartTurn();
	}
	
	private void StartTurn()
	{
		SpeedDieList.Clear();
		
		TriggerPhaseConditionals("TURNSTART");
		
		bool EnemyTeamDead = true;
		bool PlayerTeamDead = true;
		foreach(Character CheckChar in EnemyTeam) {if(CheckChar.Health > 0) {EnemyTeamDead = false;}}
		foreach(Character CheckChar in PlayerTeam) {if(CheckChar.Health > 0) {PlayerTeamDead = false;}}
		if(EnemyTeamDead || PlayerTeamDead)
		{
			EncounterIndex++;
			ProceedToNextEncounter = true;
			NextEncounterSleepTimer = 4.0;
			GetChild<Node2D>(9).Show();
			if(EnemyTeamDead)
			{
				if(EncounterIndex < ActiveBattle.Encounters.Count) {GetChild(9).GetChild<Label>(1).Text = "Proceeding..";}
				else {GetChild(9).GetChild<Label>(1).Text = "Battle Cleared"; GetChild(10).GetChild(1).GetChild<AudioStreamPlayer>(3).Play();}
			}
			else {GetChild(9).GetChild<Label>(1).Text = "Battle Failed"; EncounterIndex = 1984; GetChild(10).GetChild(1).GetChild<AudioStreamPlayer>(2).Play();}
			return;
		}
		
		
		EnableInputs();
		Vector2[] EnemyPositions = new Vector2[5] {new Vector2(346, 406), new Vector2(48, 231), new Vector2(663, 231), new Vector2(48, 581), new Vector2(663, 581)};
		Vector2[] PlayerPositions = new Vector2[5] {new Vector2(1306, 406), new Vector2(1604, 231), new Vector2(1008, 231), new Vector2(1604, 581), new Vector2(1008, 581)};
		for(int Index = 0; Index<5; Index++)
		{
			if(EnemyTeam[Index].Health > 0)
			{
				EnemyTeam[Index].Show();
				EnemyTeam[Index].SetModulate(new Color(1,1,1,1));
				EnemyTeam[Index].ZIndex = 0;
				EnemyTeam[Index].Position = EnemyPositions[Index];
				EnemyTeam[Index].Orientation = true;
				EnemyTeam[Index].UpdateOrientation();
				EnemyTeam[Index].NewTurn();
				if(EnemyTeam[Index].Staggered)
				{
					EnemyTeam[Index].Staggered = false;
					EnemyTeam[Index].StaggerHealth = EnemyTeam[Index].MaxStaggerHealth;
					EnemyTeam[Index].GetChild(5).GetChild(1).GetChild<ProgressBar>(1).Value = 1;
				}
				if(!EnemyTeam[Index].Staggered)
				{
					if(EnemyTeam[Index].StaggerHealth > 0)
					{
						AddSpeedDice(EnemyTeam[Index]);
						for(int DiceIndex = EnemyTeam[Index].GetChild(5).GetChild(2).GetChildCount()-EnemyTeam[Index].SpeedDieCount; DiceIndex<EnemyTeam[Index].GetChild(5).GetChild(2).GetChildCount(); DiceIndex++)
						{
							SpeedDieList.Add(EnemyTeam[Index].GetChild(5).GetChild(2).GetChild<SpeedDice>(DiceIndex));
						}
					}
					else {EnemyTeam[Index].Staggered = true;}
				}
			}
			else {EnemyTeam[Index].Hide();}
			
			if(PlayerTeam[Index].Health > 0)
			{
				PlayerTeam[Index].Show();
				PlayerTeam[Index].SetModulate(new Color(1,1,1,1));
				PlayerTeam[Index].ZIndex = 0;
				PlayerTeam[Index].Position = PlayerPositions[Index];
				PlayerTeam[Index].Orientation = false;
				PlayerTeam[Index].UpdateOrientation();
				PlayerTeam[Index].NewTurn();
				if(PlayerTeam[Index].Staggered)
				{
					PlayerTeam[Index].Staggered = false;
					PlayerTeam[Index].StaggerHealth = PlayerTeam[Index].MaxStaggerHealth;
					PlayerTeam[Index].GetChild(5).GetChild(1).GetChild<ProgressBar>(1).Value = 1;
				}
				if(!PlayerTeam[Index].Staggered)
				{
					if(PlayerTeam[Index].StaggerHealth > 0)
					{
						AddSpeedDice(PlayerTeam[Index]);
						for(int DiceIndex = PlayerTeam[Index].GetChild(5).GetChild(2).GetChildCount()-PlayerTeam[Index].SpeedDieCount; DiceIndex<PlayerTeam[Index].GetChild(5).GetChild(2).GetChildCount(); DiceIndex++)
						{
							SpeedDieList.Add(PlayerTeam[Index].GetChild(5).GetChild(2).GetChild<SpeedDice>(DiceIndex));
						}
					}
					else {PlayerTeam[Index].Staggered = true;}
				}
			}
			else {PlayerTeam[Index].Hide();}
		}
		SetEnemyTargets();
	}
	
	private void ExitBattle()
	{
		for(int Index = 0; Index < GetChild(10).GetChild(3).GetChildCount(); Index++)
		{
			GetChild(10).GetChild(3).GetChild<AudioStreamPlayer>(Index).Stop();
		}
		
		BattleChar_Attacker = null;
		BattleChar_Defender = null;
		BattleChar_Attacker_Dice = null;
		BattleChar_Defender_Dice = null;
		BattleChar_Index = 0;
		BattleChar_DamageDealt = 0;
		BattleChar_Active = false;
		MoveChar_Active = false;
		TriggerNextSpeedDice = false;
		
		GetParent<GameManager>().SwitchScene(true);
	}
	
	
	//Enemy targeting
	private void SetEnemyTargets()
	{
		switch(ActiveBattle.Encounters[EncounterIndex].TargetingType)
		{
			case "Random": Targeting_Random(); break;
			case "BeatDown": Targeting_BeatDown(); break;
			case "Proselyte": Targeting_Proselyte(); break;
			default: Targeting_Random(); break;
		}
	}
	
	private void Targeting_Random()
	{
		int MinCostCard = 100;
		
		int RNumCard = 0;
		int RNumChar = 0;
		foreach(SpeedDice TargetingDice in SpeedDieList)
		{
			MinCostCard = 100;
			if(Array.IndexOf(EnemyTeam, TargetingDice.GetParent().GetParent().GetParent<Character>()) != -1)
			{
				foreach(CombatCard CostCheckCard in TargetingDice.GetParent().GetParent().GetParent<Character>().Hand)
				{if(CostCheckCard.StaminaCost < MinCostCard) {MinCostCard = CostCheckCard.StaminaCost;}}
				if(MinCostCard > TargetingDice.GetParent().GetParent().GetParent<Character>().Stamina) {continue;}
				
				while(TargetingDice.SlotCard == null && TargetingDice.GetParent().GetParent().GetParent<Character>().Hand.Count > 0)
				{
					RNumCard = Rand.Next(0,TargetingDice.GetParent().GetParent().GetParent<Character>().Hand.Count);
					if(TargetingDice.GetParent().GetParent().GetParent<Character>().Hand[RNumCard].StaminaCost <= TargetingDice.GetParent().GetParent().GetParent<Character>().Stamina)
					{
						RNumChar = Rand.Next(0,5);
						while(PlayerTeam[RNumChar].Health <= 0) {RNumChar = Rand.Next(0,5);}
						TargetingDice.AssignTarget(PlayerTeam[RNumChar], TargetingDice.GetParent().GetParent().GetParent<Character>().Hand[RNumCard]);
						TargetingDice.GetParent().GetParent().GetParent<Character>().Hand.RemoveAt(RNumCard);
					}
				}
			}
		}
	}
	private void Targeting_BeatDown()
	{
		int RNumCard = 0;
		int RNumChar = 0;
		RNumChar = Rand.Next(0,5);
		while(PlayerTeam[RNumChar].Health <= 0) {RNumChar = Rand.Next(0,5);}
		foreach(SpeedDice TargetingDice in SpeedDieList)
		{
			if(Array.IndexOf(EnemyTeam, TargetingDice.GetParent().GetParent().GetParent<Character>()) != -1)
			{
				while(TargetingDice.SlotCard == null)
				{
					RNumCard = Rand.Next(0,TargetingDice.GetParent().GetParent().GetParent<Character>().Hand.Count);
					if(TargetingDice.GetParent().GetParent().GetParent<Character>().Hand[RNumCard].StaminaCost <= TargetingDice.GetParent().GetParent().GetParent<Character>().Stamina)
					{
						TargetingDice.AssignTarget(PlayerTeam[RNumChar], TargetingDice.GetParent().GetParent().GetParent<Character>().Hand[RNumCard]);
						TargetingDice.GetParent().GetParent().GetParent<Character>().Hand.RemoveAt(RNumCard);
					}
				}
			}
		}
	}
	private void Targeting_Proselyte()
	{
		int RNumCard = 0;
		while(ProselyteTargetIndex == -1 || PlayerTeam[ProselyteTargetIndex].Health <= 0)
		{ProselyteTargetIndex = Rand.Next(0,5);}
		foreach(SpeedDice TargetingDice in SpeedDieList)
		{
			if(Array.IndexOf(EnemyTeam, TargetingDice.GetParent().GetParent().GetParent<Character>()) != -1)
			{
				while(TargetingDice.SlotCard == null && TargetingDice.GetParent().GetParent().GetParent<Character>().Hand.Count > 0)
				{
					RNumCard = Rand.Next(0,TargetingDice.GetParent().GetParent().GetParent<Character>().Hand.Count);
					if(TargetingDice.GetParent().GetParent().GetParent<Character>().Hand[RNumCard].StaminaCost <= TargetingDice.GetParent().GetParent().GetParent<Character>().Stamina)
					{
						TargetingDice.AssignTarget(PlayerTeam[ProselyteTargetIndex], TargetingDice.GetParent().GetParent().GetParent<Character>().Hand[RNumCard]);
						TargetingDice.GetParent().GetParent().GetParent<Character>().Hand.RemoveAt(RNumCard);
					}
				}
			}
		}
	}
	
	
	
	//Turn play stuff
	private void PlayTurn() //Begins the process of acting out the established clashes
	{
		if(!CardBeingSet && !ProceedToNextEncounter)
		{
			GetChild(10).GetChild(1).GetChild<AudioStreamPlayer>(1).Play();
			
			GetChild<Node2D>(8).Hide();
			
			SpeedDieList = SortSpeedDieList(SpeedDieList);
			DisableInputs(); 
			for(int Index = 0; Index < 5; Index++) //Puts all characters in to a background state, so as to highlight the clashing ones
			{
				EnemyTeam[Index].SetModulate(new Color(1,1,1,0.5f));
				EnemyTeam[Index].ZIndex = -5;
				PlayerTeam[Index].SetModulate(new Color(1,1,1,0.5f));
				PlayerTeam[Index].ZIndex = -5;
			}
			foreach(Node2D TargetDice in SpeedDieList) //Hides and disables the input of all dice
			{
				TargetDice.GetChild<Node2D>(4).Show();
				TargetDice.Hide();
				TargetDice.GetChild<BaseButton>(3).Disabled = true;
			}
			TriggerNextSpeedDice = true;
			TriggerPhaseConditionals("COMBATSTART");
		}
	}
	
	private void DisableInputs() //Disables all buttons
	{
		foreach(BaseButton TargetInput in InputNodeList)
		{TargetInput.Disabled = true;}
	}
	private void EnableInputs() //Enables all buttons
	{
		foreach(BaseButton TargetInput in InputNodeList)
		{TargetInput.Disabled = false;}
	}
	
	private void MoveCharsToApproach(Character Char1, Character Char2) //Makes 2 characters move towards one another, works with a segement in _Process()
	{
		if(Char1.Position.X < Char2.Position.X)
		{
			Char1.Orientation = true;
			Char2.Orientation = false;
			MoveChar_CharacterLeft = Char1;
			MoveChar_CharacterRight = Char2;
		}
		else
		{
			Char2.Orientation = false;
			Char2.Orientation = true;
			MoveChar_CharacterLeft = Char2;
			MoveChar_CharacterRight = Char1;
		}
		Char1.UpdateSpriteState(1);
		Char2.UpdateSpriteState(1);
		
		MoveChar_TargetY = (int)(Char1.Position.Y+Char2.Position.Y)/2;
		
		int MoveChar_Overlap = 358; double MoveChar_Scale = 0.15; //Overlap is the total amount of overlap, where full overlap is 1792 (the size of a chararacter). Scale is the (horizontal) scale of the characters.
		
		int MoveChar_MeanX = (int)(Char1.Position.X+Char2.Position.X+1792*MoveChar_Scale)/2;
		
		MoveChar_CharacterLeft_TargetX = MoveChar_MeanX - (int)(1792*MoveChar_Scale) + (int)((MoveChar_Overlap/2)*MoveChar_Scale);
		MoveChar_CharacterRight_TargetX = MoveChar_MeanX - (int)((MoveChar_Overlap/2)*MoveChar_Scale);
		
		MoveChar_TimeRemaining = 2.5; //How long the movement will take
		MoveChar_Active = true;
	}
	
	private void BattleChar_EndClash() //Resets the BattleChar variables and returns the characters to a background state
	{
		if(BattleChar_Attacker.Health>0) {BattleChar_Attacker.UpdateSpriteState(0);} //Set both back to Idle state
		if(BattleChar_Defender.Health>0) {BattleChar_Defender.UpdateSpriteState(0);}
		BattleChar_Attacker.SetModulate(new Color(1,1,1,0.5f));
		BattleChar_Defender.SetModulate(new Color(1,1,1,0.5f));
		BattleChar_Attacker.ZIndex = -5;
		BattleChar_Defender.ZIndex = -5;
		BattleChar_Attacker.ActiveDeck.Add(BattleChar_Attacker_Dice.SlotCard);
		BattleChar_Attacker_Dice.QueueFree();
		if(BattleChar_Defender_Dice != null)
		{
			BattleChar_Defender.ActiveDeck.Add(BattleChar_Defender_Dice.SlotCard);
			BattleChar_Defender_Dice.QueueFree();
		}
		BattleChar_Attacker = null;
		BattleChar_Defender = null;
		BattleChar_Attacker_Dice = null;
		BattleChar_Defender_Dice = null;
		BattleChar_Index = 0;
		BattleChar_DamageDealt = 0;
		BattleChar_Active = false;
		TriggerNextSpeedDice = true;
	}
	
	
	
	private List<SpeedDice> SortSpeedDieList(List<SpeedDice> InputList)
	{
		List<SpeedDice> OutputList = new List<SpeedDice>();
		while(InputList.Count > 0)
		{
			int HighestSpeed = 0;
			for(int Index = 0; Index<InputList.Count; Index++)
			{if(InputList[Index].Value > HighestSpeed) {HighestSpeed = InputList[Index].Value;}}
			for(int Index = 0; Index<InputList.Count; Index++)
			{
				while(true && Index<InputList.Count)
				{
					if(InputList[Index].Value == HighestSpeed)
					{
						OutputList.Add(InputList[Index]);
						InputList.RemoveAt(Index);
					}
					else {break;}
				}
			}
		}
		return OutputList;
	}
	
	
	//Updating display stuff
	private void RefreshInfo()
	{
		if(InfoTarget != null)
		{
			//MainSection
			GetChild(2).GetChild(3).GetChild<Label>(0).Text = InfoTarget.Name;
			GetChild(2).GetChild(3).GetChild<Label>(1).Text = Convert.ToString(InfoTarget.Health);
			GetChild(2).GetChild(3).GetChild<Label>(2).Text = Convert.ToString(InfoTarget.StaggerHealth);
			
			//Hand
			for(int Index = 0; Index < InfoTarget.Hand.Count; Index++)
			{if(InfoTarget.Hand[Index] == null) {InfoTarget.Hand.RemoveAt(Index);}}
			for(int Index = 0; Index < 9; Index++)
			{
				if(Index < InfoTarget.Hand.Count)
				{
					GetChild(8).GetChild(1).GetChild<Node2D>(Index).Show();
					GetChild(8).GetChild(1).GetChild<CombatCard>(Index).CopyCombatCard(InfoTarget.Hand[Index]);
					GetChild(8).GetChild(1).GetChild<CombatCard>(Index).Refresh();
				}
				else
				{
					GetChild(8).GetChild(1).GetChild<Node2D>(Index).Hide();
				}
			}
			
			//Character Card
			GetChild(8).GetChild<CharacterCard>(2).CopyCharCard(InfoTarget.CharCard);
			GetChild(8).GetChild<CharacterCard>(2).Refresh();
			
			//Passives
			GetChild(8).GetChild<PassiveContainer>(3).UpdatePassives(InfoTarget.CharCard.PassiveList);
			
			//Numbers stuff
			GetChild(8).GetChild(4).GetChild(0).GetChild<Label>(1).Text = Convert.ToString(InfoTarget.EmotionLevel);
			GetChild(8).GetChild(4).GetChild(1).GetChild<Label>(1).Text = Convert.ToString(InfoTarget.EmotionPoints);
			GetChild(8).GetChild(4).GetChild(2).GetChild<Label>(1).Text = Convert.ToString(InfoTarget.CardsUsedBattle);
			GetChild(8).GetChild(4).GetChild(3).GetChild<Label>(1).Text = Convert.ToString(InfoTarget.CardsUsedEncounter);
			GetChild(8).GetChild(4).GetChild(4).GetChild<Label>(1).Text = Convert.ToString(InfoTarget.UniqueCardsUsed.Count);
			GetChild(8).GetChild(4).GetChild(5).GetChild<Label>(1).Text = Convert.ToString(InfoTarget.Singleton);
		}
	}
	
	private void RefreshCardSelectionMenu()
	{
		for(int Index = 0; Index < SettingDice.GetParent().GetParent().GetParent<Character>().Hand.Count; Index++)
		{if(SettingDice.GetParent().GetParent().GetParent<Character>().Hand[Index] == null) {SettingDice.GetParent().GetParent().GetParent<Character>().Hand.RemoveAt(Index);}}
		for(int Index = 0; Index < 9; Index++)
		{
			if(Index < SettingDice.GetParent().GetParent().GetParent<Character>().Hand.Count)
			{
				GetChild(7).GetChild(1).GetChild(0).GetChild<Node2D>(Index).Show();
				GetChild(7).GetChild(1).GetChild(0).GetChild<CombatCard>(Index).CopyCombatCard(SettingDice.GetParent().GetParent().GetParent<Character>().Hand[Index]);
				GetChild(7).GetChild(1).GetChild(0).GetChild<CombatCard>(Index).Refresh();
			}
			else
			{
				GetChild(7).GetChild(1).GetChild(0).GetChild<Node2D>(Index).Hide();
			}
		}
		
		if(SettingDice.SlotCard != null)
		{
			GetChild(7).GetChild<Node2D>(2).Show();
			GetChild(7).GetChild<CombatCard>(2).CopyCombatCard(SettingDice.SlotCard);
			GetChild(7).GetChild<CombatCard>(2).Refresh();
		}
		else {GetChild(7).GetChild<Node2D>(2).Hide();}
	}
	
	//Character Manipulation
	private void AddSpeedDice(Character Target)
	{
		for(int Index = 0; Index < Target.GetChild(5).GetChild(2).GetChildCount(); Index++)
		{
			Target.GetChild(5).GetChild(2).GetChild(Index).QueueFree();
		}
		var DiceScene = GD.Load<PackedScene>("res://Scenes/ObjectScenes/SpeedDice.tscn");
		for(int Index = 0; Index < Target.SpeedDieCount; Index++)
		{
			var DiceInstance = DiceScene.Instantiate();
			
			Target.GetChild(5).GetChild(2).AddChild(DiceInstance);
			Target.GetChild(5).GetChild(2).GetChild<SpeedDice>(Target.GetChild(5).GetChild(2).GetChildCount()-1).Position = new Vector2(((Index+1)*1792/(Target.SpeedDieCount+1)) - 225, 0);
			Target.GetChild(5).GetChild(2).GetChild<SpeedDice>(Target.GetChild(5).GetChild(2).GetChildCount()-1).Value = Rand.Next(Target.CharCard.MinSpeed, Target.CharCard.MaxSpeed+1);
			Target.GetChild(5).GetChild(2).GetChild<SpeedDice>(Target.GetChild(5).GetChild(2).GetChildCount()-1).OriginalPosX = ((Index+1)*1792/(Target.SpeedDieCount+1)) - 225;
			Target.GetChild(5).GetChild(2).GetChild<SpeedDice>(Target.GetChild(5).GetChild(2).GetChildCount()-1).OriginalPosY = 0;
		}
	}
	
	
	//Conditionals
	private void TriggerPhaseConditionals(string PhaseIdentifier)
	{
		string[] CardTriggers = new string[] {"COMBATSTART"};
		
		foreach(Character CheckChar in EnemyTeam)
		{
			if(CheckChar.Health > 0)
			{
				CheckChar.TriggerPassivesAndStatuses(PhaseIdentifier);
			}
		}
		foreach(Character CheckChar in PlayerTeam)
		{
			if(CheckChar.Health > 0)
			{
				CheckChar.TriggerPassivesAndStatuses(PhaseIdentifier);
			}
		}
		
		if(Array.IndexOf(CardTriggers, PhaseIdentifier) != -1)
		{
			foreach(SpeedDice CheckDice in SpeedDieList)
			{
				if(CheckDice.SlotCard != null)
				{
					if(CheckDice.SlotCard.ConditionalTrigger == PhaseIdentifier)
					{TriggerConditional(CheckDice.SlotCard, CheckDice.GetParent().GetParent().GetParent<Character>());}
				}
			}
		}
	}
	
	private CombatCard TriggerConditional(CombatCard SourceCCard, Character Owner)
	{if(CheckConditionalCondition(SourceCCard.Condition, Owner)) {EnactConditionalEffect(SourceCCard.Effect, Owner);} return null;}
	
	private CombatDice TriggerConditional(CombatDice SourceCDice, Character Owner, SpeedDice OwnerDice = null)
	{if(CheckConditionalCondition(SourceCDice.Condition, Owner, OwnerDice)) {EnactConditionalEffect(SourceCDice.Effect, Owner, OwnerDice);} return null;}
	
	
	
	private bool CheckConditionalCondition(string[] Condition, Character Owner, SpeedDice OwnerDice = null)
	{
		Character TargetChar = null;
		int ValueA = 0;
		int ValueB = 0;
		switch(Condition[0])
		{
			case "Roll":
				if(OwnerDice != null) {ValueA = OwnerDice.Value;}
				break;
			case "User":
				TargetChar = Owner;
				break;
			case "Target":
				if(OwnerDice != null) {TargetChar = OwnerDice.Target;}
				else {return false;}
				break;
			default:
				return false;
		}
		if(TargetChar != null)
		{
			switch(Condition[1])
			{
				case "Health": ValueA = TargetChar.Health; break;
				case "StaggerHealth": ValueA = TargetChar.StaggerHealth; break;
				case "Stamina": ValueA = TargetChar.Stamina; break;
				case "MaxHealth": ValueA = TargetChar.MaxHealth; break;
				case "MaxStaggerHealth": ValueA = TargetChar.MaxStaggerHealth; break;
				case "MaxStamina": ValueA = TargetChar.MaxStamina; break;
				case "Hand": ValueA = TargetChar.Hand.Count; break;
				case "SpeedDie": ValueA = TargetChar.SpeedDieCount; break;
				case "EmotionLvl": ValueA = TargetChar.EmotionLevel; break;
				case "BattleUsedCards": ValueA = TargetChar.CardsUsedBattle; break;
				case "EncounterUsedCards": ValueA = TargetChar.CardsUsedEncounter; break;
				case "UniqueUsedCards": ValueA = TargetChar.UniqueCardsUsed.Count; break;
				case "Singleton": return TargetChar.Singleton;
				case "DiceType":
					if(OwnerDice != null) {if(BattleChar_Index<OwnerDice.SlotCard.Dice.Count) {ValueA = OwnerDice.SlotCard.Dice[BattleChar_Index].DiceType;}}
					break;
				default:
					if(Condition[1].Length>6 && Condition[1].Substring(0, 7) == "Status.")
					{
						if(TargetChar.StatusEffects.FindIndex(x => x.Identifier.Equals(Condition[1].Substring(7))) != -1)
						{ValueA = TargetChar.StatusEffects[TargetChar.StatusEffects.FindIndex(x => x.Identifier.Equals(Condition[1].Substring(7)))].Count;}
					}
					else {return false;}
					break;
			}
			
			switch(Condition[3])
			{
				case "Health": ValueB = TargetChar.Health; break;
				case "StaggerHealth": ValueB = TargetChar.StaggerHealth; break;
				case "Stamina": ValueB = TargetChar.Stamina; break;
				case "MaxHealth": ValueB = TargetChar.MaxHealth; break;
				case "MaxStaggerHealth": ValueB = TargetChar.MaxStaggerHealth; break;
				case "MaxStamina": ValueB = TargetChar.MaxStamina; break;
				case "Hand": ValueB = TargetChar.Hand.Count; break;
				case "SpeedDie": ValueB = TargetChar.SpeedDieCount; break;
				case "EmotionLvl": ValueB = TargetChar.EmotionLevel; break;
				case "BattleUsedCards": ValueB = TargetChar.CardsUsedBattle; break;
				case "EncounterUsedCards": ValueB = TargetChar.CardsUsedEncounter; break;
				case "UniqueUsedCards": ValueB = TargetChar.UniqueCardsUsed.Count; break;
				default:
					try{ValueB = Convert.ToInt32(Condition[3]);}
					catch{}
					break;
			}
		}
		else
		{
			try{ValueB = Convert.ToInt32(Condition[3]);}
			catch{return false;}
		}
		
		switch(Condition[2])
		{
			case "Eq": return (ValueA == ValueB);
			case "Lt": return (ValueA < ValueB);
			case "Gt": return (ValueA > ValueB);
			case "Neq": return (ValueA != ValueB);
			default:
				return false;
		}
	}
	
	
	
	private void EnactConditionalEffect(string[] Effect, Character Owner, SpeedDice OwnerDice = null)
	{
		bool CharTargeted = false;
		Character TargetChar = Owner;
		SpeedDice TargetDice = null;
		StatusEffect TargetStatus = null;
		float ValueB = 0;
		switch(Effect[0])
		{
			case "Roll":
				if(OwnerDice != null) {TargetDice = OwnerDice;}
				else {return;}
				break;
			case "User":
				TargetChar = Owner;
				CharTargeted = true;
				break;
			case "Target":
				if(OwnerDice != null) {TargetChar = OwnerDice.Target; CharTargeted = true;}
				else if(BattleChar_Attacker != null && BattleChar_Defender != null)
				{
					if(Owner == BattleChar_Attacker) {TargetChar = BattleChar_Defender; CharTargeted = true;}
					else if (Owner == BattleChar_Defender) {TargetChar = BattleChar_Attacker; CharTargeted = true;}
				}
				break;
			default:
				return;
		}
		
		switch(Effect[3])
			{
				case "Health": ValueB = Convert.ToSingle(TargetChar.Health); break;
				case "StaggerHealth": ValueB = Convert.ToSingle(TargetChar.StaggerHealth); break;
				case "Stamina": ValueB = Convert.ToSingle(TargetChar.Stamina); break;
				case "MaxHealth": ValueB = Convert.ToSingle(TargetChar.MaxHealth); break;
				case "MaxStaggerHealth": ValueB = Convert.ToSingle(TargetChar.MaxStaggerHealth); break;
				case "MaxStamina": ValueB = Convert.ToSingle(TargetChar.MaxStamina); break;
				case "EmotionLvl": ValueB = Convert.ToSingle(TargetChar.EmotionLevel); break;
				default:
					if(Effect[3].Length>6 && Effect[3].Substring(0, 7) == "Status.")
					{
						if(TargetChar.StatusEffects.FindIndex(x => x.Identifier.Equals(Effect[3].Substring(7))) != -1)
						{ValueB = Convert.ToSingle(TargetChar.StatusEffects[TargetChar.StatusEffects.FindIndex(x => x.Identifier.Equals(Effect[3].Substring(7)))].Count);}
					}
					else
					{
						try {ValueB = Convert.ToSingle(Effect[3]);}
						catch{}
					}
					break;
			}
		
		if(CharTargeted)
		{
			switch(Effect[1])
			{
				case "Health":
					if(Effect[2] == "Inc") {TargetChar.Health += Convert.ToInt32(ValueB); if(ValueB < 0) {TargetChar.TriggerPassivesAndStatuses("ONDAMAGED");}}
					if(Effect[2] == "Dec") {TargetChar.Health -= Convert.ToInt32(ValueB); if(ValueB > 0) {TargetChar.TriggerPassivesAndStatuses("ONDAMAGED");}}
					if(Effect[2] == "Set") {if(ValueB < TargetChar.Health) {TargetChar.TriggerPassivesAndStatuses("ONDAMAGED");} TargetChar.Health = Convert.ToInt32(ValueB);}
					if(Effect[2] == "Mlt") {TargetChar.Health = Convert.ToInt32((float)TargetChar.Health*ValueB); if(ValueB < 1) {TargetChar.TriggerPassivesAndStatuses("ONDAMAGED");}}
					if(Effect[2] == "Div") {TargetChar.Health = Convert.ToInt32((float)TargetChar.Health/ValueB); if(ValueB > 1) {TargetChar.TriggerPassivesAndStatuses("ONDAMAGED");}}
					
					if(TargetChar.Health <= 0) {TargetChar.Death();}
					else if(TargetChar.Health > TargetChar.MaxHealth) {TargetChar.Health = TargetChar.MaxHealth;}
					TargetChar.GetChild(5).GetChild(0).GetChild<ProgressBar>(1).Value = (float)TargetChar.Health/(float)TargetChar.MaxHealth;
					break;
				case "StaggerHealth":
					if(Effect[2] == "Inc") {TargetChar.StaggerHealth += Convert.ToInt32(ValueB); if(ValueB < 0) {TargetChar.TriggerPassivesAndStatuses("ONDAMAGED");}}
					if(Effect[2] == "Dec") {TargetChar.StaggerHealth -= Convert.ToInt32(ValueB); if(ValueB > 0) {TargetChar.TriggerPassivesAndStatuses("ONDAMAGED");}}
					if(Effect[2] == "Set") {if(ValueB < TargetChar.Health) {TargetChar.TriggerPassivesAndStatuses("ONDAMAGED");} TargetChar.StaggerHealth = Convert.ToInt32(ValueB);}
					if(Effect[2] == "Mlt") {TargetChar.StaggerHealth = Convert.ToInt32((float)TargetChar.StaggerHealth*ValueB); if(ValueB < 1) {TargetChar.TriggerPassivesAndStatuses("ONDAMAGED");}}
					if(Effect[2] == "Div") {TargetChar.StaggerHealth = Convert.ToInt32((float)TargetChar.StaggerHealth/ValueB); if(ValueB > 1) {TargetChar.TriggerPassivesAndStatuses("ONDAMAGED");}}
					
					if(TargetChar.StaggerHealth <= 0) {TargetChar.StaggerHealth = 0; TargetChar.UpdateSpriteState(2);}
					else if(TargetChar.StaggerHealth > TargetChar.MaxStaggerHealth) {TargetChar.StaggerHealth = TargetChar.MaxStaggerHealth;}
					TargetChar.GetChild(5).GetChild(1).GetChild<ProgressBar>(1).Value = (float)TargetChar.StaggerHealth/(float)TargetChar.MaxStaggerHealth;
					break;
				case "Stamina":
					if(Effect[2] == "Inc") {TargetChar.Stamina += Convert.ToInt32(ValueB);}
					if(Effect[2] == "Dec") {TargetChar.Stamina -= Convert.ToInt32(ValueB);}
					if(Effect[2] == "Set") {TargetChar.Stamina = Convert.ToInt32(ValueB);}
					if(Effect[2] == "Mlt") {TargetChar.Stamina = Convert.ToInt32((float)TargetChar.Stamina*ValueB);}
					if(Effect[2] == "Div") {TargetChar.Stamina = Convert.ToInt32((float)TargetChar.Stamina/ValueB);}
					break;
				case "MaxHealth":
					if(Effect[2] == "Inc") {TargetChar.MaxHealth += Convert.ToInt32(ValueB);}
					if(Effect[2] == "Dec") {TargetChar.MaxHealth -= Convert.ToInt32(ValueB);}
					if(Effect[2] == "Set") {TargetChar.MaxHealth = Convert.ToInt32(ValueB);}
					if(Effect[2] == "Mlt") {TargetChar.MaxHealth = Convert.ToInt32((float)TargetChar.MaxHealth*ValueB);}
					if(Effect[2] == "Div") {TargetChar.MaxHealth = Convert.ToInt32((float)TargetChar.MaxHealth/ValueB);}
					
					if(TargetChar.Health > TargetChar.MaxHealth) {TargetChar.Health = TargetChar.MaxHealth;}
					if(TargetChar.Health <= 0) {TargetChar.Death();}
					TargetChar.GetChild(5).GetChild(0).GetChild<ProgressBar>(1).Value = (float)TargetChar.Health/(float)TargetChar.MaxHealth;
					break;
				case "MaxStaggerHealth":
					if(Effect[2] == "Inc") {TargetChar.MaxStaggerHealth += Convert.ToInt32(ValueB);}
					if(Effect[2] == "Dec") {TargetChar.MaxStaggerHealth -= Convert.ToInt32(ValueB);}
					if(Effect[2] == "Set") {TargetChar.MaxStaggerHealth = Convert.ToInt32(ValueB);}
					if(Effect[2] == "Mlt") {TargetChar.MaxStaggerHealth = Convert.ToInt32((float)TargetChar.MaxStaggerHealth*ValueB);}
					if(Effect[2] == "Div") {TargetChar.MaxStaggerHealth = Convert.ToInt32((float)TargetChar.MaxStaggerHealth/ValueB);}
					
					if(TargetChar.StaggerHealth > TargetChar.MaxStaggerHealth) {TargetChar.StaggerHealth = TargetChar.MaxStaggerHealth;}
					if(TargetChar.StaggerHealth <= 0) {TargetChar.StaggerHealth = 0; TargetChar.UpdateSpriteState(2);}
					TargetChar.GetChild(5).GetChild(1).GetChild<ProgressBar>(1).Value = (float)TargetChar.StaggerHealth/(float)TargetChar.MaxStaggerHealth;
					break;
				case "MaxStamina":
					if(Effect[2] == "Inc") {TargetChar.MaxStamina += Convert.ToInt32(ValueB);}
					if(Effect[2] == "Dec") {TargetChar.MaxStamina -= Convert.ToInt32(ValueB);}
					if(Effect[2] == "Set") {TargetChar.MaxStamina = Convert.ToInt32(ValueB);}
					if(Effect[2] == "Mlt") {TargetChar.MaxStamina = Convert.ToInt32((float)TargetChar.MaxStamina*ValueB);}
					if(Effect[2] == "Div") {TargetChar.MaxStamina = Convert.ToInt32((float)TargetChar.MaxStamina/ValueB);}
					break;
				case "SpeedDie":
					if(Effect[2] == "Inc") {TargetChar.SpeedDieCount += Convert.ToInt32(ValueB);}
					if(Effect[2] == "Dec") {TargetChar.SpeedDieCount -= Convert.ToInt32(ValueB);}
					if(Effect[2] == "Set") {TargetChar.SpeedDieCount = Convert.ToInt32(ValueB);}
					if(Effect[2] == "Mlt") {TargetChar.SpeedDieCount = Convert.ToInt32((float)TargetChar.SpeedDieCount*ValueB);}
					if(Effect[2] == "Div") {TargetChar.SpeedDieCount = Convert.ToInt32((float)TargetChar.SpeedDieCount/ValueB);}
					break;
				case "EmotionPoints":
					if(Effect[2] == "Inc") {TargetChar.EmotionPoints += Convert.ToInt32(ValueB);}
					if(Effect[2] == "Dec") {TargetChar.EmotionPoints -= Convert.ToInt32(ValueB);}
					if(Effect[2] == "Set") {TargetChar.EmotionPoints = Convert.ToInt32(ValueB);}
					if(Effect[2] == "Mlt") {TargetChar.EmotionPoints = Convert.ToInt32((float)TargetChar.EmotionPoints*ValueB);}
					if(Effect[2] == "Div") {TargetChar.EmotionPoints = Convert.ToInt32((float)TargetChar.EmotionPoints/ValueB);}
					break;
				case "Draw":
					TargetChar.DrawCard(Convert.ToInt32(ValueB));
					break;
				case "Discard":
					while(TargetChar.Hand.Count > 0 && ValueB > 0)
					{TargetChar.Hand.RemoveAt(Rand.Next(0,TargetChar.Hand.Count));}
					break;
				default:
					if(Effect[1].Length>6 && Effect[1].Substring(0, 7) == "Status.")
					{TargetChar.ChangeStatusValue(Effect[1].Substring(7), ValueB, Effect[2]);}
					break;
			}
		}
		else if(TargetDice != null)
		{
			switch(Effect[2])
			{
				case "Inc": TargetDice.Value += Convert.ToInt32(ValueB); break;
				case "Dec": TargetDice.Value -= Convert.ToInt32(ValueB); break;
				case "Set": TargetDice.Value = Convert.ToInt32(ValueB); break;
				case "Mlt": TargetDice.Value = Convert.ToInt32((float)TargetDice.Value*ValueB); break;
				case "Div": TargetDice.Value = Convert.ToInt32((float)TargetDice.Value/ValueB); break;
			}
		}
	}
	
	//Connection Inputs
	private void CharacterPressed(int Team, int Index)
	{
		if(GetChild(4).GetChild(Team).GetChild<Character>(Index).Health > 0)
		{
			if(CardBeingSet && SettingDice != null && !GetChild<Node2D>(7).Visible)
			{
				if(GetChild(4).GetChild(Team).GetChild<Character>(Index) != SettingDice.GetParent().GetParent().GetParent<Character>() && SettingDice.SlotCard != null)
				{
					SettingDice.AssignTarget(GetChild(4).GetChild(Team).GetChild<Character>(Index), SettingDice.SlotCard);
					SettingDice = null;
					CardBeingSet = false;
					GetChild<Node2D>(6).Hide();
					GetChild<Node2D>(5).Hide();
				}
			}
			else
			{
				InfoTarget = GetChild(4).GetChild(Team).GetChild<Character>(Index);
				RefreshInfo();
			}
		}
	}
	
	private void InfoVisibilityToggleButtonPressed()
	{if(InfoTarget != null) {GetChild<Node2D>(2).Visible = !(GetChild<Node2D>(2).Visible);}}
	
	private void MoreInfoButtonPressed()
	{GetChild<Node2D>(8).Visible = !(GetChild<Node2D>(8).Visible);}
	
	//Speed Dice Inputs
	public void SpeedDiceClicked(SpeedDice TargetDice)
	{
		GetChild(10).GetChild(0).GetChild<AudioStreamPlayer>(0).Play();
		if(!CardBeingSet)
		{
			if(Array.IndexOf(PlayerTeam, TargetDice.GetParent().GetParent().GetParent<Character>()) != -1)
			{
				CardBeingSet = true;
				SettingDice = TargetDice;
				GetChild<Node2D>(7).Show();
				RefreshCardSelectionMenu();
			}
		}
		else if(SettingDice != null && !GetChild<Node2D>(7).Visible)
		{
			if(SettingDice.GetParent() != TargetDice.GetParent())
			{
				SettingDice.AssignTarget(TargetDice.GetParent().GetParent().GetParent<Character>(), SettingDice.SlotCard);
				CardBeingSet = false;
				SettingDice = null;
			}
		}
	}
	
	public void SpeedDiceHovered(SpeedDice TargetDice)
	{
		if(TargetDice.SlotCard != null)
		{
			GetChild<Node2D>(6).Show();
			GetChild<Node2D>(5).Show();
			GetChild<CombatCard>(5).CopyCombatCard(TargetDice.SlotCard);
			GetChild<CombatCard>(5).Refresh();
		}
		else
		{
			GetChild<Node2D>(6).Hide();
			GetChild<Node2D>(5).Hide();
		}
	}
	public void SpeedDiceUnhovered(SpeedDice TargetDice)
	{
		if(SettingDice != null)
		{
			if(SettingDice.SlotCard != null)
			{
				GetChild<CombatCard>(5).CopyCombatCard(SettingDice.SlotCard);
				GetChild<CombatCard>(5).Refresh();
			}
			else
			{
				GetChild<Node2D>(6).Hide();
				GetChild<Node2D>(5).Hide();
			}
		}
		else
		{
			GetChild<Node2D>(6).Hide();
			GetChild<Node2D>(5).Hide();
		}
	}
	
	//Music auto-changing
	private void _MusicTrack_Finished(int Index)
	{
		if(Index<GetChild(10).GetChild(3).GetChildCount()-1) //Checks track isn't the last one in the list
		{ //Plays next track in list
			GetChild(10).GetChild(3).GetChild<AudioStreamPlayer>(Index+1).Play();
			GetChild<Label>(11).Text = "Current Track: "+GetChild(10).GetChild(3).GetChild<AudioStreamPlayer>(Index+1).Stream.ResourceName;
		}
		else
		{ //Loops back to first track in list
			GetChild(10).GetChild(3).GetChild<AudioStreamPlayer>(0).Play();
			GetChild<Label>(11).Text = "Current Track: "+GetChild(10).GetChild(3).GetChild<AudioStreamPlayer>(0).Stream.ResourceName;
		}
		TriggerPhaseConditionals("SONGEND");
	}
	
	
	//CardSelectionMenu Inputs
	private void _CardSelection_CardSelected(int Index)
	{
		if(Index < SettingDice.GetParent().GetParent().GetParent<Character>().Hand.Count)
		{
			if(SettingDice.GetParent().GetParent().GetParent<Character>().Hand[Index].StaminaCost <= SettingDice.GetParent().GetParent().GetParent<Character>().Stamina)
			{
				if(SettingDice.SlotCard != null)
				{
					SettingDice.GetParent().GetParent().GetParent<Character>().Hand.Add(SettingDice.SlotCard);
					SettingDice.GetParent().GetParent().GetParent<Character>().Stamina += SettingDice.SlotCard.StaminaCost;
				}
				SettingDice.SlotCard = SettingDice.GetParent().GetParent().GetParent<Character>().Hand[Index];
				SettingDice.GetParent().GetParent().GetParent<Character>().Hand.RemoveAt(Index);
				SettingDice.GetParent().GetParent().GetParent<Character>().Stamina -= SettingDice.SlotCard.StaminaCost;
				RefreshCardSelectionMenu();
			}
		}
	}
	private void _CardSelection_Confirm()
	{
		if(SettingDice.SlotCard != null)
		{
			GetChild<Node2D>(7).Hide();
			GetChild<Node2D>(6).Show();
			GetChild<Node2D>(5).Show();
			GetChild<CombatCard>(5).CopyCombatCard(SettingDice.SlotCard);
			GetChild<CombatCard>(5).Refresh();
		}
	}
	private void _CardSelection_Cancel()
	{
		if(SettingDice.Target != null) {SettingDice.Clear();}
		else if(SettingDice.Slotcard != null) {SettingDice.GetParent().GetParent().GetParent<Character>().Hand.Add(SettingDice.SlotCard); SettingDice.GetParent().GetParent().GetParent<Character>().Stamina += SettingDice.SlotCard.StaminaCost; SettingDice.SlotCard = null; }
		CardBeingSet = false;
		SettingDice = null;
		GetChild<Node2D>(7).Hide();
	}
}
