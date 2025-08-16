using Godot;
using System;
using System.Collections.Generic;

public partial class BattleSelectionMenu : Node2D
{
	int PageNum = 0;
	int MaxPage = 0;
	int Filter = 0;
	string SearchTerm = "";
	int Selected = 0;
	public List<Battle> FullBattleList = new List<Battle>();
	List<Battle> FilteredBattleList = new List<Battle>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetChild(2).GetChild(0).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[0];
		GetChild(2).GetChild(1).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[1];
		GetChild(2).GetChild(2).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[2];
		GetChild(2).GetChild(3).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[3];
		GetChild(2).GetChild(4).GetChild<Button>(1).Icon = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[4];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Custom Internal Functions
	
	public void Refresh(List<Battle> BattleList)
	{
		FullBattleList = BattleList;
		Filter = 0;
		PageNum = 0;
		UpdateFilter();
		_Battle_Selected(0);
	}
	
	public void UpdateFilter()
	{
		FilteredBattleList.Clear();
		for(int Index = 0; Index<FullBattleList.Count; Index++)
		{if((FullBattleList[Index].DiffGrade == Filter || Filter == 0) && FullBattleList[Index].Name.ToLower().Contains(SearchTerm.ToLower()))
			{FilteredBattleList.Add(FullBattleList[Index]);}}
		MaxPage = FilteredBattleList.Count/12;
		PageNum = 0;
		Selected = 0;
		SetPage();
	}
	
	public void SetPage()
	{
		GetChild(4).GetChild(1).GetChild<Label>(2).Text = Convert.ToString(PageNum);
		int Offset = PageNum*12;
		for(int Index = 0; Index<12; Index++)
		{
			Node2D Leaf = GetChild(4).GetChild(0).GetChild<Node2D>(Index);
			if(Offset+Index<FilteredBattleList.Count)
			{
				Leaf.Show();
				Leaf.GetChild<Label>(3).Text = FilteredBattleList[Offset+Index].Name;
				
				Leaf.GetChild<Label>(7).Hide();
				switch(FilteredBattleList[Offset+Index].DiffGrade)
				{
					case 1: Leaf.GetChild<Polygon2D>(0).Color = new Color("2e8799"); break;
					case 2: Leaf.GetChild<Polygon2D>(0).Color = new Color("660a0a"); break;
					case 3: Leaf.GetChild<Polygon2D>(0).Color = new Color("195e80"); break;
					case 4: Leaf.GetChild<Polygon2D>(0).Color = new Color("080a99"); break;
					case 5: Leaf.GetChild<Polygon2D>(0).Color = new Color("997a1f"); break;
					default:
						Leaf.GetChild<Polygon2D>(0).Color = new Color("cccccc");
						string Num = Convert.ToString(FilteredBattleList[Offset+Index].DiffGrade);
						Leaf.GetChild<Label>(7).Text = $"{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}\n{Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}  {Num}";
						Leaf.GetChild<Label>(7).Show();
						break;
				}
				if(FilteredBattleList[Offset+Index].DiffGrade >= 0 && FilteredBattleList[Offset+Index].DiffGrade < 5)
				{
					Leaf.GetChild<Sprite2D>(5).Texture = GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[FilteredBattleList[Offset+Index].DiffGrade];
					if(GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[FilteredBattleList[Offset+Index].DiffGrade] != null)
					{Leaf.GetChild<Sprite2D>(5).Scale = new Vector2((float)50/GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[FilteredBattleList[Offset+Index].DiffGrade].GetWidth(), (float)30/GetTree().GetRoot().GetChild<GameManager>(0).DiffGradeTextures[FilteredBattleList[Offset+Index].DiffGrade].GetHeight());}
				}
				
				Texture2D IconTexture = FilteredBattleList[Offset+Index].Icon;
				Leaf.GetChild<Sprite2D>(2).Texture = IconTexture;
				if(IconTexture == null) {continue;}
				Leaf.GetChild<Node2D>(2).Scale = new Vector2((float)60/IconTexture.GetWidth(),(float)37.5/IconTexture.GetHeight());
			}
			else {Leaf.Hide();}
		}
	}
	
	// Connection Inputs
	
	public void _Begin_Pressed()
	{
		if(Selected < FilteredBattleList.Count) {GetParent().GetParent().GetParent<GameManager>().StartBattle(FilteredBattleList[Selected]);}
	}
	
	private void _Battle_Selected(int Index)
	{
		Selected = (12*PageNum)+Index;
		if(Selected < FilteredBattleList.Count)
		{
			Node Info = GetChild(3);
			Info.GetChild<Label>(3).Text = FilteredBattleList[Selected].Name;
			Texture2D IconTexture = null;
			if(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(FilteredBattleList[Selected].ImgPath)) != -1) {IconTexture = ResourceLoadUtils.LoadTexture2DFromImage(((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImages[((SceneTree)Engine.GetMainLoop()).Root.GetChild<GameManager>(0).PreloadedImagePaths.FindIndex(x => x.Equals(FilteredBattleList[Selected].ImgPath))], ((SceneTree)Engine.GetMainLoop()).Root);}
			Info.GetChild<Sprite2D>(2).Texture = IconTexture;
			if(IconTexture != null) {Info.GetChild<Sprite2D>(2).Scale = new Vector2((float)159.5/IconTexture.GetWidth(), (float)147.5/IconTexture.GetHeight());}
			Info.GetChild<Label>(4).Text = "Grade: "+FilteredBattleList[Selected].DiffGrade;
			Info.GetChild<Label>(5).Text = "Encounters: "+FilteredBattleList[Selected].Encounters.Count;
			Info.GetChild<Label>(6).Text = "Author: "+FilteredBattleList[Selected].Author;
			Info.GetChild<Label>(7).Text = "Description:\n"+FilteredBattleList[Selected].Description;
		}
	}
	
	private void _GradeFilter_Pressed(int Grade)
	{
		if(Grade != Filter) {Filter = Grade;}
		else {Filter = 0;}
		for(int Index = 0; Index<5; Index++)
		{
			if(Index+1 == Filter) {GetChild(2).GetChild(Index).GetChild<Node2D>(2).Show();}
			else {GetChild(2).GetChild(Index).GetChild<Node2D>(2).Hide();}
		}
		UpdateFilter();
	}
	
	private void _PageChange_Pressed(bool LeftRight)
	{
		if(LeftRight && PageNum>0) {PageNum--;}
		else if (!LeftRight && PageNum < MaxPage) {PageNum++;}
		SetPage();
	}
	
	private void _SearchInput_Changed()
	{
		if(SearchTerm.ToLower() != GetChild(5).GetChild<TextEdit>(1).Text.ToLower())
		{SearchTerm = GetChild(5).GetChild<TextEdit>(1).Text.ToLower(); UpdateFilter();}
	}
}
