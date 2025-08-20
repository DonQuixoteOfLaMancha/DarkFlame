using Godot;
using System;
using System.Collections.Generic;

public partial class Character : Node2D
{
	static Random RNumGen = new Random();
	bool ExistsInTree = false;
	
	public string Name = "Roland";
	
	public int[] CosmeticAttributes = [0,0,0,0];
	public int HairRed = 255; public int HairGreen = 255; public int HairBlue = 255;
	public int SpriteState = 0; //Sprite states go from 0-8: Idle, Moving, Damage, Slash, Pierce, Blunt, Block, Evade, Dead
	public bool Orientation = false; //false is left-facing, true is right-facing
	
	public CharacterCard CharCard = new CharacterCard();
	public SpriteFrames CharCardSprites;
	
	
	public int MaxHealth = 15; public int MaxStaggerHealth = 15; private int maxstamina = 3;
	public int Health = 15; public int StaggerHealth = 15; private int stamina = 3;
	
	public int MaxStamina
	{
		get {return maxstamina;}
		set
		{
			maxstamina = value;
			try
			{
				if(GetChildCount() > 6)
				{
					if(GetChild(5).GetChild(4).GetChildCount() < maxstamina)
					{
						var StaminaScene = GD.Load<PackedScene>("res://Scenes/ObjectScenes/Stamina.tscn");
						for(int Index = GetChild(5).GetChild(4).GetChildCount(); Index<maxstamina; Index++)
						{
							var StaminaInstance = StaminaScene.Instantiate();
							 GetChild(5).GetChild(4).AddChild(StaminaInstance);
						}
					}
					else if(GetChild(5).GetChild(4).GetChildCount() > maxstamina)
					{
						for(int Index = maxstamina; Index < GetChild(5).GetChild(4).GetChildCount(); Index++)
						{
							GetChild(5).GetChild(4).GetChild(Index).QueueFree();
						}
					}
				}
			}
			catch{}
		}
	}
	public int Stamina
	{
		get {return stamina;}
		set
		{
			stamina = value;
			if(stamina > maxstamina) {stamina = maxstamina;}
			if(stamina < 0) {stamina = 0;}
			try
			{
				if(GetChildCount() > 6)
				{
					for(int Index = 0; Index < GetChild(5).GetChild(4).GetChildCount(); Index++)
					{
						GetChild(5).GetChild(4).GetChild<Stamina>(Index).Active = (Index < stamina);
					}
				}
			}
			catch{}
		}
	}
	
	
	public int EmotionPoints = 0; public int EmotionLevel = 0;
	public int DrawAmount = 1;
	public int SpeedDieCount = 1;
	
	public int CardsUsedEncounter = 0;
	public int CardsUsedBattle = 0;
	public List<CombatCard> UniqueCardsUsed = new List<CombatCard>();
	public bool Singleton = false;
	
	public bool Staggered = false;
	private List<StatusEffect> QueuedStatuses = new List<StatusEffect>();
	public List<StatusEffect> StatusEffects = new List<StatusEffect>();
	public CombatCard[] Deck = new CombatCard[9]; public List<CombatCard> ActiveDeck= new List<CombatCard>(); public List<CombatCard> Hand = new List<CombatCard>();
	
	int UpdateStatusesNextFrame = 0;
	
	public Character()
	{
		try{CharCardSprites = ResourceLoadUtils.LoadSpriteSheetFromImage(1792,1792,1,0,((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImages[((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(CharCard.SpriteSheetSource))], ((SceneTree)Engine.GetMainLoop()).Root);}
		catch{CharCardSprites = ResourceLoadUtils.LoadSpriteSheet(1792,1792,1,0,CharCard.SpriteSheetSource, null);}
	}
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ExistsInTree = true;
		LoadCardSprites();
		LoadAccessorySprites();
		Refresh();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(UpdateStatusesNextFrame == 2) {UpdateStatusesNextFrame--;}
		else if(UpdateStatusesNextFrame == 1)
		{
			StatusEffects.Clear();
			for(int Index = 0; Index < GetChild(5).GetChild(3).GetChildCount(); Index++)
			{
				if(Index < GetChild(5).GetChild(3).GetChildCount()-QueuedStatuses.Count)
				{
					StatusEffects.Add(GetChild(5).GetChild(3).GetChild<StatusEffect>(Index));
				}
				else
				{
					GetChild(5).GetChild(3).GetChild<StatusEffect>(Index).CopyStatusEffect(QueuedStatuses[Index-(GetChild(5).GetChild(3).GetChildCount()-QueuedStatuses.Count)]);
					GetChild(5).GetChild(3).GetChild<StatusEffect>(Index).Count = QueuedStatuses[Index-(GetChild(5).GetChild(3).GetChildCount()-QueuedStatuses.Count)].Count;
					StatusEffects.Add(GetChild(5).GetChild(3).GetChild<StatusEffect>(Index));
				}
				GetChild(5).GetChild(3).GetChild<StatusEffect>(Index).Position = new Vector2(Index*100, 0);
				GetChild(5).GetChild(3).GetChild<StatusEffect>(Index).Refresh();
			}
			QueuedStatuses.Clear();
			UpdateStatusesNextFrame--;
		}
	}
	
	// SpriteSheet Setting Methods
	
	public void LoadCardSprites()
	{
		try
		{
			if(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).LoadedSpriteSheetPaths.Contains(CharCard.SpriteSheetSource))
			{
				CharCardSprites = ((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).LoadedSpriteSheets[((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).LoadedSpriteSheetPaths.FindIndex(x => x.Equals(CharCard.SpriteSheetSource))];
			}
			else
			{
				CharCardSprites = ResourceLoadUtils.LoadSpriteSheetFromImage(1792,1792,1,0,((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImages[((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(CharCard.SpriteSheetSource))], ((SceneTree)Engine.GetMainLoop()).Root);
				((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).LoadedSpriteSheetPaths.Add(CharCard.SpriteSheetSource);
				((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).LoadedSpriteSheets.Add(CharCardSprites);
			}
		}
		catch
		{
			CharCardSprites = ResourceLoadUtils.LoadSpriteSheet(1792,1792,1,0,CharCard.SpriteSheetSource,null);
			try
			{
				((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).LoadedSpriteSheetPaths.Add(CharCard.SpriteSheetSource);
				((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).LoadedSpriteSheets.Add(CharCardSprites);
			}
			catch{}
		}
		if(CharCardSprites != null) {GetChild<AnimatedSprite2D>(0).SetSpriteFrames(CharCardSprites); GetChild<AnimatedSprite2D>(0).SetAnimation("SpriteSheet"); GetChild<AnimatedSprite2D>(0).Stop();} 
	}
	private void LoadAccessorySprites()
	{
		SpriteFrames CurSprtFrm = ResourceLoadUtils.LoadSpriteSheet(750,1200,1,1,"user://Content/DefaultContent/Images/CharacterSpriteElements/BackHairSheet.png", ((SceneTree)Engine.GetMainLoop()).Root);
		if(CurSprtFrm != null) {GetChild<AnimatedSprite2D>(1).SetSpriteFrames(CurSprtFrm); GetChild<AnimatedSprite2D>(1).Stop();}
		CurSprtFrm = ResourceLoadUtils.LoadSpriteSheet(750,1200,1,1,"user://Content/DefaultContent/Images/CharacterSpriteElements/FrontHairSheet.png", ((SceneTree)Engine.GetMainLoop()).Root);
		if(CurSprtFrm != null) {GetChild<AnimatedSprite2D>(2).SetSpriteFrames(CurSprtFrm); GetChild<AnimatedSprite2D>(2).Stop();}
		CurSprtFrm = ResourceLoadUtils.LoadSpriteSheet(200,120,1,1,"user://Content/DefaultContent/Images/CharacterSpriteElements/MouthSheet.png", ((SceneTree)Engine.GetMainLoop()).Root);
		if(CurSprtFrm != null) {GetChild<AnimatedSprite2D>(3).SetSpriteFrames(CurSprtFrm); GetChild<AnimatedSprite2D>(3).Stop();}
		CurSprtFrm = ResourceLoadUtils.LoadSpriteSheet(220,100,1,1,"user://Content/DefaultContent/Images/CharacterSpriteElements/EyesSheet.png", ((SceneTree)Engine.GetMainLoop()).Root);
		if(CurSprtFrm != null) {GetChild<AnimatedSprite2D>(4).SetSpriteFrames(CurSprtFrm); GetChild<AnimatedSprite2D>(4).Stop();}
	}
	
	//CIF
	public void Refresh()
	{
		UpdateCosmeticAttributes();
		UpdateHairColour();
		UpdateOrientation();
		LoadCardSprites();
		GetChild<AnimatedSprite2D>(0).SetSpriteFrames(CharCardSprites);
		GetChild<AnimatedSprite2D>(0).Stop(); GetChild<AnimatedSprite2D>(0).SetAnimation("SpriteSheet");
		UpdateSpriteState();
	}
	
	public void SetCosmeticAttribute(int AttributeIndex, int SpriteIndex)
	{
		CosmeticAttributes[AttributeIndex] = SpriteIndex;
		if(ExistsInTree) {UpdateCosmeticAttributes();}
	}
	public void UpdateCosmeticAttributes()
	{
		GetChild<AnimatedSprite2D>(1).Frame = CosmeticAttributes[0];
		GetChild<AnimatedSprite2D>(2).Frame = CosmeticAttributes[1];
		GetChild<AnimatedSprite2D>(3).Frame = CosmeticAttributes[2];
		GetChild<AnimatedSprite2D>(4).Frame = CosmeticAttributes[3];
	}
	
	public void SetHairColour(int InputRed = -1, int InputGreen = -1, int InputBlue = -1)
	{
		if(InputRed>-1){HairRed = InputRed;}
		if(InputGreen>-1){HairGreen = InputGreen;}
		if(InputBlue>-1){HairBlue = InputBlue;}
		if(ExistsInTree) {UpdateHairColour();}
	}
	public void UpdateHairColour()
	{
		GetChild<AnimatedSprite2D>(1).Modulate = new Color(HairRed/255f, HairGreen/255f, HairBlue/255f);
		GetChild<AnimatedSprite2D>(2).Modulate = new Color(HairRed/255f, HairGreen/255f, HairBlue/255f);
	}
	
	public void UpdateOrientation()
	{
		if(Orientation)
		{
			GetChild<AnimatedSprite2D>(0).FlipH = true;
			GetChild<AnimatedSprite2D>(1).FlipH = true;
			GetChild<AnimatedSprite2D>(1).SetPosition(new Vector2(1042-CharCard.BackHairOrigins[SpriteState, 0], CharCard.BackHairOrigins[SpriteState, 1]));
			GetChild<AnimatedSprite2D>(2).FlipH = true;
			GetChild<AnimatedSprite2D>(2).SetPosition(new Vector2(1042-CharCard.FrontHairOrigins[SpriteState, 0], CharCard.FrontHairOrigins[SpriteState, 1]));
			GetChild<AnimatedSprite2D>(3).FlipH = true;
			GetChild<AnimatedSprite2D>(3).SetPosition(new Vector2(1592-CharCard.MouthOrigins[SpriteState, 0], CharCard.MouthOrigins[SpriteState, 1]));
			GetChild<AnimatedSprite2D>(4).FlipH = true;
			GetChild<AnimatedSprite2D>(4).SetPosition(new Vector2(1572-CharCard.EyeOrigins[SpriteState, 0], CharCard.EyeOrigins[SpriteState, 1]));
		}
		else
		{
			GetChild<AnimatedSprite2D>(0).FlipH = false;
			GetChild<AnimatedSprite2D>(1).FlipH = false;
			GetChild<AnimatedSprite2D>(1).SetPosition(new Vector2(CharCard.BackHairOrigins[SpriteState, 0], CharCard.BackHairOrigins[SpriteState, 1]));
			GetChild<AnimatedSprite2D>(2).FlipH = false;
			GetChild<AnimatedSprite2D>(2).SetPosition(new Vector2(CharCard.FrontHairOrigins[SpriteState, 0], CharCard.FrontHairOrigins[SpriteState, 1]));
			GetChild<AnimatedSprite2D>(3).FlipH = false;
			GetChild<AnimatedSprite2D>(3).SetPosition(new Vector2(CharCard.MouthOrigins[SpriteState, 0], CharCard.MouthOrigins[SpriteState, 1]));
			GetChild<AnimatedSprite2D>(4).FlipH = false;
			GetChild<AnimatedSprite2D>(4).SetPosition(new Vector2(CharCard.EyeOrigins[SpriteState, 0], CharCard.EyeOrigins[SpriteState, 1]));
		}
	}
	
	public void UpdateSpriteState(int InputState = -1)
	{
		if(InputState != -1) {SpriteState = InputState;}
		GetChild<AnimatedSprite2D>(0).Frame = SpriteState;
		
		if(CharCard.BackHairOrigins[SpriteState, 0] >= 1792 || CharCard.BackHairOrigins[SpriteState, 1] >= 1792) {GetChild<AnimatedSprite2D>(1).Hide();}
		else {GetChild<AnimatedSprite2D>(1).Show();}
		if(CharCard.FrontHairOrigins[SpriteState, 0] >= 1792 || CharCard.FrontHairOrigins[SpriteState, 1] >= 1792) {GetChild<AnimatedSprite2D>(2).Hide();}
		else {GetChild<AnimatedSprite2D>(2).Show();}
		if(CharCard.MouthOrigins[SpriteState, 0] >= 1792 || CharCard.MouthOrigins[SpriteState, 1] >= 1792) {GetChild<AnimatedSprite2D>(3).Hide();}
		else {GetChild<AnimatedSprite2D>(3).Show();}
		if(CharCard.EyeOrigins[SpriteState, 0] >= 1792 || CharCard.EyeOrigins[SpriteState, 1] >= 1792) {GetChild<AnimatedSprite2D>(4).Hide();}
		else {GetChild<AnimatedSprite2D>(4).Show();}
		
		UpdateOrientation();
	}
	
	public void ChangeStatusValue(string Identifier, float Value, string Operand)
	{
		StatusEffect TargetStatus;
		if(StatusEffects.FindIndex(x => x.Identifier.Equals(Identifier)) != -1)
		{TargetStatus = StatusEffects[StatusEffects.FindIndex(x => x.Identifier.Equals(Identifier))];}
		else if(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).LoadedStatusEffects.FindIndex(x => x.Identifier.Equals(Identifier)) != -1)
		{
			var StatusEffectScene = GD.Load<PackedScene>("res://Scenes/ObjectScenes/StatusEffect.tscn");
			var StatusEffectInstance = StatusEffectScene.Instantiate();
			GetChild(5).GetChild(3).AddChild(StatusEffectInstance);
			QueuedStatuses.Add(new StatusEffect());
			QueuedStatuses[QueuedStatuses.Count-1].CopyStatusEffect(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).LoadedStatusEffects[((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).LoadedStatusEffects.FindIndex(x => x.Identifier.Equals(Identifier))]);
			TargetStatus = QueuedStatuses[QueuedStatuses.Count-1];
			UpdateStatusesNextFrame = 2;
		}
		else {return;}
		
		switch(Operand)
		{
			case "Inc":
				TargetStatus.Count += Convert.ToInt32(Value);
				break;
			case "Dec":
				TargetStatus.Count -= Convert.ToInt32(Value);
				break;
			case "Set":
				TargetStatus.Count = Convert.ToInt32(Value);
				break;
			case "Mlt":
				TargetStatus.Count = Convert.ToInt32((float)TargetStatus.Count*Value);
				break;
			case "Div":
				TargetStatus.Count = Convert.ToInt32((float)TargetStatus.Count/Value);
				break;
		}
	}
	
	
	public void CopyCharacter(Character InputCharacter)
	{
		if(InputCharacter != null)
		{
			Name = InputCharacter.Name;
			CosmeticAttributes = InputCharacter.CosmeticAttributes;
			HairRed = InputCharacter.HairRed; HairGreen = InputCharacter.HairGreen; HairBlue = InputCharacter.HairBlue;
			CharCard = InputCharacter.CharCard;
			CharCardSprites = InputCharacter.CharCardSprites;
			MaxStamina = InputCharacter.MaxStamina;
			Deck = InputCharacter.Deck;
		}
	}
	
	
	public void Reset()
	{
		Refresh();
		GetChild<Node2D>(5).Show();
		UpdateSpriteState(0);
		MaxHealth = CharCard.Health; Health = MaxHealth;
		MaxStaggerHealth = CharCard.StaggerHealth; StaggerHealth = MaxStaggerHealth;
		GetChild(5).GetChild(0).GetChild<ProgressBar>(1).Value = (float)Health/(float)MaxHealth;
		GetChild(5).GetChild(1).GetChild<ProgressBar>(1).Value = (float)StaggerHealth/(float)MaxStaggerHealth;
		MaxStamina = 3; Stamina = MaxStamina;
		Staggered = false;
		SpeedDieCount = 1;
		EmotionLevel = 0; EmotionPoints = 0;
		foreach(StatusEffect RemoveStatus in StatusEffects) {RemoveStatus.QueueFree();}
		StatusEffects.Clear();
		if(Health <= 0) {Death();}
		
		CardsUsedEncounter = 0;
		CardsUsedBattle = 0;
		UniqueCardsUsed.Clear();
		
		Singleton = true;
		for(int Index = 0; Index < 9; Index++)
		{
			if(Deck[Index] != null)
			{
				if(Array.FindIndex(Deck, x => x.Name.Equals(Deck[Index].Name)) != -1 && Array.FindIndex(Deck, x => x.Name.Equals(Deck[Index].Name)) != Index)
				{
					Singleton = false;
					break;
				}
			}
		}
	}
	
	public void NewTurn()
	{
		switch(EmotionLevel)
		{
			case 0:
				if(EmotionPoints >= 3)
				{EmotionLevel++; EmotionPoints = 0; MaxStamina++; Stamina = MaxStamina; TriggerPassivesAndStatuses("EMOTIONLVLCHANGE");}
				break;
			case 1:
				if(EmotionPoints >= 3)
				{EmotionLevel++; EmotionPoints = 0; MaxStamina++; Stamina = MaxStamina; TriggerPassivesAndStatuses("EMOTIONLVLCHANGE");}
				break;
			case 2:
				if(EmotionPoints >= 5)
				{EmotionLevel++; EmotionPoints = 0; MaxStamina++; Stamina = MaxStamina; TriggerPassivesAndStatuses("EMOTIONLVLCHANGE");}
				break;
			case 3:
				if(EmotionPoints >= 7)
				{EmotionLevel++; EmotionPoints = 0; SpeedDieCount++; MaxStamina++; Stamina = MaxStamina; TriggerPassivesAndStatuses("EMOTIONLVLCHANGE");}
				break;
			case 4:
				if(EmotionPoints >= 9)
				{EmotionLevel++; EmotionPoints = 0; DrawAmount++; MaxStamina ++; Stamina = MaxStamina; TriggerPassivesAndStatuses("EMOTIONLVLCHANGE");}
				break;
		}
		
		if(Stamina < MaxStamina) {Stamina++;}
		DrawCard(DrawAmount);
		for(int Index = 0; Index<StatusEffects.Count; Index++)
		{
			if(StatusEffects[Index] != null)
			{
				if(StatusEffects[Index].AutoDecrement)
				{
					if(StatusEffects[Index].Count == 1)
					{
						StatusEffects[Index].Count--;
						Index--;
					}
					else {StatusEffects[Index].Count--;}
				}
			}
			else {StatusEffects.RemoveAt(Index); Index--;}
		}
		StatusEffects.Clear();
		for(int Index = 0; Index<GetChild(5).GetChild(3).GetChildCount(); Index++)
		{
			StatusEffects.Add(GetChild(5).GetChild(3).GetChild<StatusEffect>(Index));
		}
		UpdateStatusesNextFrame = 2;
	}
	
	public void DrawCard(int Amount)
	{
		int Num;
		for(int i = 0; i<Amount; i++)
		{
			if(ActiveDeck.Count == 0) {break;}
			Num = RNumGen.Next(0,ActiveDeck.Count);
			Hand.Add(ActiveDeck[Num]);
			ActiveDeck.RemoveAt(Num);
		}
	}
	
	public void TriggerPassivesAndStatuses(string TriggerIdentifier, SpeedDice OwnerSpeedDice = null)
	{
		string[] PassiveTriggers = new string[] {"BATTLESTART","ENCOUNTERSTART","TURNSTART","COMBATSTART","COMBATEND", "SONGEND", "ONROLL", "ONHIT", "ONDAMAGED", "CLASHWIN", "CLASHLOSE", "CLASHDRAW", "EMOTIONLVLCHANGE", "ONCARDUSE", "ONALLYDEATH", "ONENEMYDEATH"};
		string[] StatusTriggers = new string[] {"TURNSTART","COMBATSTART","COMBATEND", "SONGEND", "ONROLL", "ONHIT", "ONDAMAGED", "CLASHWIN", "CLASHLOSE", "CLASHDRAW", "ONCARDUSE", "ONALLYDEATH", "ONENEMYDEATH"};
		
		if(Array.IndexOf(PassiveTriggers, TriggerIdentifier) != -1)
		{
			for(int Index = 0; Index < CharCard.PassiveList.Count; Index++)
			{
				if(CharCard.PassiveList[Index].Trigger == TriggerIdentifier)
				{
					if(CheckConditionalCondition(CharCard.PassiveList[Index].Condition, OwnerSpeedDice))
					{EnactConditionalEffect(CharCard.PassiveList[Index].Effect, OwnerSpeedDice);}
				}
			}
		}
		
		if(Array.IndexOf(StatusTriggers, TriggerIdentifier) != -1)
		{
			for(int Index = 0; Index < StatusEffects.Count; Index++)
			{
				if(StatusEffects[Index].Trigger == TriggerIdentifier)
				{
					if(CheckConditionalCondition(StatusEffects[Index].Condition, OwnerSpeedDice))
					{
						foreach(string[] Effect in StatusEffects[Index].EffectList)
						{EnactConditionalEffect(Effect, OwnerSpeedDice);}
					}
				}
			}
		}
	}
	private bool CheckConditionalCondition(string[] Condition, SpeedDice OwnerDice = null)
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
				TargetChar = this;
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
					if(OwnerDice != null) {if(GetParent().GetParent().GetParent<BattleSceneManager>().BattleChar_Index<OwnerDice.SlotCard.Dice.Count) {ValueA = OwnerDice.SlotCard.Dice[GetParent().GetParent().GetParent<BattleSceneManager>().BattleChar_Index].DiceType;}}
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
					catch{return false;}
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
	
	private void EnactConditionalEffect(string[] Effect, SpeedDice OwnerDice = null)
	{
		bool CharTargeted = false;
		Character TargetChar = this;
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
				TargetChar = this; CharTargeted = true;
				break;
			case "Target":
				if(OwnerDice != null) {TargetChar = OwnerDice.Target; CharTargeted = true;}
				else {return;}
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
				case "BattleUsedCards": ValueB = Convert.ToSingle(TargetChar.CardsUsedBattle); break;
				case "EncounterUsedCards": ValueB = Convert.ToSingle(TargetChar.CardsUsedEncounter); break;
				case "UniqueUsedCards": ValueB = Convert.ToSingle(TargetChar.UniqueCardsUsed.Count); break;
				default:
					if(Effect[3].Length>6 && Effect[3].Substring(0, 7) == "Status.")
					{
						if(TargetChar.StatusEffects.FindIndex(x => x.Identifier.Equals(Effect[3].Substring(7))) != -1)
						{ValueB = TargetChar.StatusEffects[TargetChar.StatusEffects.FindIndex(x => x.Identifier.Equals(Effect[3].Substring(7)))].Count;}
					}
					else
					{
						try{ValueB = Convert.ToInt32(Effect[3]);}
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
				case "BattleUsedCards":
					if(Effect[2] == "Inc") {TargetChar.CardsUsedBattle += Convert.ToInt32(ValueB);}
					if(Effect[2] == "Dec") {TargetChar.CardsUsedBattle -= Convert.ToInt32(ValueB);}
					if(Effect[2] == "Set") {TargetChar.CardsUsedBattle = Convert.ToInt32(ValueB);}
					if(Effect[2] == "Mlt") {TargetChar.CardsUsedBattle = Convert.ToInt32((float)TargetChar.CardsUsedBattle*ValueB);}
					if(Effect[2] == "Div") {TargetChar.CardsUsedBattle = Convert.ToInt32((float)TargetChar.CardsUsedBattle/ValueB);}
					break;
				case "EncounterUsedCards":
					if(Effect[2] == "Inc") {TargetChar.CardsUsedEncounter += Convert.ToInt32(ValueB);}
					if(Effect[2] == "Dec") {TargetChar.CardsUsedEncounter -= Convert.ToInt32(ValueB);}
					if(Effect[2] == "Set") {TargetChar.CardsUsedEncounter = Convert.ToInt32(ValueB);}
					if(Effect[2] == "Mlt") {TargetChar.CardsUsedEncounter = Convert.ToInt32((float)TargetChar.CardsUsedEncounter*ValueB);}
					if(Effect[2] == "Div") {TargetChar.CardsUsedEncounter = Convert.ToInt32((float)TargetChar.CardsUsedEncounter/ValueB);}
					break;
				case "Draw":
					GD.Print(TargetChar.Hand.Count);
					TargetChar.DrawCard(Convert.ToInt32(ValueB));
					GD.Print(TargetChar.Hand.Count);
					break;
				case "Discard":
					while(TargetChar.Hand.Count > 0 && ValueB > 0)
					{TargetChar.Hand.RemoveAt(RNumGen.Next(0,TargetChar.Hand.Count));}
					break;
				case "ResetUniqueCardTracker":
					TargetChar.UniqueCardsUsed.Clear();
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
	
	
	public int Damage(int Amount, int DamageType)
	{
		int AmountDealt = 0;
		switch(DamageType)
		{
			case 0: //Slash
				if(StaggerHealth > 0)
				{
					AmountDealt = Convert.ToInt32(Amount*CharCard.SlashRes);
					Health -= AmountDealt;
					StaggerHealth -= Convert.ToInt32(Amount*CharCard.SlashStagRes);
				}
				else
				{
					AmountDealt = Amount*2;
					Health -= AmountDealt;
					StaggerHealth -= Amount*2;
				}
				break;
			case 1: //Pierce
				if(StaggerHealth > 0)
				{
					AmountDealt = Convert.ToInt32(Amount*CharCard.PierceRes);
					Health -= AmountDealt;
					StaggerHealth -= Convert.ToInt32(Amount*CharCard.PierceStagRes);
				}
				else
				{
					AmountDealt = Amount*2;
					Health -= AmountDealt;
					StaggerHealth -= Amount*2;
				}
				break;
			case 2: //Blunt
				if(StaggerHealth > 0)
				{
					AmountDealt = Convert.ToInt32(Amount*CharCard.BluntRes);
					Health -= AmountDealt;
					StaggerHealth -= Convert.ToInt32(Amount*CharCard.BluntStagRes);
				}
				else
				{
					AmountDealt = Amount*2;
					Health -= AmountDealt;
					StaggerHealth -= Amount*2;
				}
				break;
			case 3: //Block/True STAGGER
				StaggerHealth -= Amount;
				break;
			case 4: //Evade (Does Nothing)
				break;
			case 5: //True NORMAL (Red)
				AmountDealt = Amount;
				Health -= Amount;
				break;
			case 6: //True STAGGER (White) (Deals same damage as block, but in a different way)
				AmountDealt = Amount;
				StaggerHealth -= Amount;
				break;
			case 7: //True ALL (Black)
				AmountDealt = Amount;
				Health -= Amount;
				StaggerHealth -= Amount;
				break;
			case 8: //True NORMAL Percentage (Pale)
				AmountDealt = (MaxHealth*Amount)/100;
				Health -= Amount;
				break;
		}
		if(Health <= 0) {Death();}
		else if(Health > MaxHealth) {Health = MaxHealth;}
		if(StaggerHealth <= 0) {StaggerHealth = 0; UpdateSpriteState(2);}
		else if(StaggerHealth > MaxStaggerHealth) {StaggerHealth = MaxStaggerHealth;}
		GetChild(5).GetChild(0).GetChild<ProgressBar>(1).Value = (float)Health/(float)MaxHealth;
		GetChild(5).GetChild(1).GetChild<ProgressBar>(1).Value = (float)StaggerHealth/(float)MaxStaggerHealth;
		if(AmountDealt > 0){TriggerPassivesAndStatuses("ONDAMAGED");}
		return AmountDealt;
	}
	
	public void Death()
	{
		Health = 0;
		UpdateSpriteState(8);
		GetChild<Node2D>(5).Hide();
		
		//Trigger On ___ death conditionals
		for(int Index = 0; Index < 5; Index++)
		{
			if(GetParent().GetChild<Character>(Index).Health > 0) {GetParent().GetChild<Character>(Index).TriggerPassivesAndStatuses("ONALLYDEATH");}
			if(GetParent().GetParent().GetChild(0) == GetParent() && GetParent().GetParent().GetChild(1).GetChild<Character>(Index).Health > 0)
			{GetParent().GetParent().GetChild(1).GetChild<Character>(Index).TriggerPassivesAndStatuses("ONENEMYDEATH");}
			else if(GetParent().GetParent().GetChild(0).GetChild<Character>(Index).Health > 0)
			{GetParent().GetParent().GetChild(0).GetChild<Character>(Index).TriggerPassivesAndStatuses("ONENEMYDEATH");}
		}
	}
	
	//Stuff for storing and loading data
	
	public string ConvertToDonTxt()
	{
		string[,] PropertiesData = new string[7,2];
		
		PropertiesData[0,0] = "Name"; PropertiesData[1,0] = "CosmeticAttributes"; PropertiesData[2,0] = "CharCard";
		PropertiesData[3,0] = "HairRed"; PropertiesData[4,0] = "HairGreen"; PropertiesData[5,0] = "HairBlue";
		PropertiesData[6,0] = "Deck";
		
		PropertiesData[0,1] = Name;
		PropertiesData[1,1] = "["+Convert.ToString(CosmeticAttributes[0])+","+Convert.ToString(CosmeticAttributes[1])+","+Convert.ToString(CosmeticAttributes[2])+","+Convert.ToString(CosmeticAttributes[3])+"]";
		PropertiesData[2,1] = CharCard.ConvertToDonTxt();
		PropertiesData[3,1] = Convert.ToString(HairRed); PropertiesData[4,1] = Convert.ToString(HairGreen); PropertiesData[5,1] = Convert.ToString(HairBlue);
		PropertiesData[6,1] = DeckToDonTxt();
		
		string DonTxtForm = DataStorageManager.ConvertToDonTxt("CHARACTER", PropertiesData);
		
		return DonTxtForm;
	}
	public void SetFromPropertiesData(string[,] PropertiesData)
	{
		for(int Index = 0; Index<PropertiesData.GetLength(0); Index++)
		{
			switch(PropertiesData[Index,0])
			{
				case "Name": Name = PropertiesData[Index,1]; break;
				case "CharCard": CharCard.SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(PropertiesData[Index,1])); break;
				case "CosmeticAttributes": CosmeticAttributes = StringToIntArray(PropertiesData[Index,1]); break;
				case "HairRed": HairRed = Convert.ToInt32(PropertiesData[Index,1]); break;//
				case "HairGreen": HairGreen = Convert.ToInt32(PropertiesData[Index,1]); break;
				case "HairBlue": HairBlue = Convert.ToInt32(PropertiesData[Index,1]); break;
				case "Deck": Deck = StringToDeckArray(PropertiesData[Index,1]); break; 
			}
		}
	}
	
	private string DeckToDonTxt()
	{
		string DeckTxt = "[";
		for(int Index = 0; Index<9; Index++)
		{
			if(Deck[Index] != null)
			{
				DeckTxt += Deck[Index].ConvertToDonTxt();
				if(Index<8) {DeckTxt += ",";}
			}
		}
		DeckTxt += "]";
		
		return DeckTxt;
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
	private int[] StringToIntArray(string InputString)
	{
		int[] OutputArray = new int[4];
		string[] Middleman = StringToArray(InputString);
		for(int Index = 0; Index<4; Index++) {OutputArray[Index] = Convert.ToInt32(Middleman[Index]);}
		return OutputArray;
	}
	
	private List<string> StringToList(string InputString)
	{
		List<string> OutputList = new List<string>();
		string WorkingString = "";
		int Depth = -1;
		foreach(char ConvChar in InputString)
		{
			if(ConvChar == '[') {Depth++; if(Depth==0){continue;}}
			if(ConvChar == ']') {Depth--;}
			if((ConvChar == ',' && Depth == 0) || (Depth == -1 && WorkingString != "")) {OutputList.Add(WorkingString); WorkingString = ""; continue;}
			WorkingString += ConvChar;
		}

		return OutputList;
	}
	private CombatCard[] StringToDeckArray(string InputString)
	{
		CombatCard[] OutputArray = new CombatCard[9];
		List<string> Middleman = StringToList(InputString);
		for(int Index = 0; Index<Middleman.Count; Index++)
		{
			CombatCard WorkingCard = new CombatCard();
			WorkingCard.SetFromPropertiesData(DataStorageManager.ConvertToPropertiesData(Middleman[Index]));
			OutputArray[Index] = WorkingCard;
		}
		
		return OutputArray;
	}
}
