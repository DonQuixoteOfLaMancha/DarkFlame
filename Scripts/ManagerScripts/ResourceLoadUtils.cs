using Godot;
using System;

public static class ResourceLoadUtils 
{
	//https://github.com/godotengine/godot/issues/18367#issuecomment-383521520
	public static Texture2D LoadTexture2D(string FilePath, Node Root = null, int OriginX = 0, int OriginY = 0, int XSize = 0, int YSize = 0)
	{
		if(FileAccess.FileExists(FilePath))
		{
			try
				{
					var ImgBytes = FileAccess.GetFileAsBytes(FilePath);
					Image Img = new Image();
					Img.LoadPngFromBuffer(ImgBytes);
					if(XSize != 0 && YSize != 0)
					{
						Godot.Rect2I Section = new Godot.Rect2I(OriginX, OriginY, XSize, YSize);
						Img = Img.GetRegion(Section);
					}
					Texture2D ImgTexture = ImageTexture.CreateFromImage(Img);
					
					return ImgTexture;
				}
			catch
				{
					if(Root != null) {Root.GetChild(0).GetChild<Console>(1).WriteToConsole("An error occured when loading the file at the path "+FilePath);}
					return null;
				}
		}
		else {if(Root != null){Root.GetChild(0).GetChild<Console>(1).WriteToConsole("Could not find a file at the path "+FilePath);} return null;}
	}
	
	public static SpriteFrames LoadSpriteSheet(int XSize, int YSize, int XSeparator, int YSeparator, string FilePath, Node Root = null)
	{
		//For some reason the seperator doesn't work? I get the black bars still visible, I think it's a Godot issue
		//though cause it also happens with the normal import, could also be with the way the images get saved maybe?
		//I already checked if there were any parts of the line where it shouldn't be. there weren't any
		SpriteFrames SpriteFrameSet = new SpriteFrames();
		SpriteFrameSet.AddAnimation("SpriteSheet");
		Image Img = new Image();
		var ImgBytes = Img.GetData();
		if(FileAccess.FileExists(FilePath)) {ImgBytes = FileAccess.GetFileAsBytes(FilePath);}
		else {if(Root != null){Root.GetChild(0).GetChild<Console>(1).WriteToConsole("Could not find a file at the path "+FilePath);} return SpriteFrameSet;}
		try
		{
			Img.LoadPngFromBuffer(ImgBytes);
			int Width = Img.GetWidth();
			int Height = Img.GetHeight();
			int NumX = (Width/(XSize+XSeparator))+1;
			int NumY = (Height/(YSize+YSeparator))+1;
			if(XSeparator == 0) {NumX--;}
			if(YSeparator == 0) {NumY--;}
			
			for(int Y = 0; Y<NumY; Y++)
			{
				for(int X = 0; X<NumX; X++)
				{
					Image WorkingImage = new Image();
					WorkingImage.CopyFrom(Img);
					Godot.Rect2I Section = new Godot.Rect2I(X*(XSize+XSeparator-1), Y*(YSize+YSeparator-1), XSize, YSize);
					WorkingImage = WorkingImage.GetRegion(Section);
					Texture2D Frame = ImageTexture.CreateFromImage(WorkingImage);
					SpriteFrameSet.AddFrame("SpriteSheet", Frame);
				}
			}
			return SpriteFrameSet;
		}
		catch
		{
			if(Root != null) {Root.GetChild(0).GetChild<Console>(1).WriteToConsole("An error occured when loading the file at the path "+FilePath);}
			return null;
		}
	}
	
	
	//Above modified to work with existing images so that I can preload the images
	public static Texture2D LoadTexture2DFromImage(Image Img, Node Root = null, int OriginX = 0, int OriginY = 0, int XSize = 0, int YSize = 0)
	{
		try
			{
				if(XSize != 0 && YSize != 0)
				{
					Godot.Rect2I Section = new Godot.Rect2I(OriginX, OriginY, XSize, YSize);
					Img = Img.GetRegion(Section);
				}
				Texture2D ImgTexture = ImageTexture.CreateFromImage(Img);
				
				return ImgTexture;
			}
		catch
		{
			if(Root != null) {Root.GetChild(0).GetChild<Console>(1).WriteToConsole("An error occured when converting an image to a texture2d");}
			return null;
		}
	}
	
	public static SpriteFrames LoadSpriteSheetFromImage(int XSize, int YSize, int XSeparator, int YSeparator, Image Img, Node Root = null)
	{
		//For some reason the seperator doesn't work? I get the black bars still visible, I think it's a Godot issue
		//though cause it also happens with the normal import, could also be with the way the images get saved maybe?
		//I already checked if there were any parts of the line where it shouldn't be. there weren't any
		SpriteFrames SpriteFrameSet = new SpriteFrames();
		SpriteFrameSet.AddAnimation("SpriteSheet");
		try
		{
			int Width = Img.GetWidth();
			int Height = Img.GetHeight();
			int NumX = (Width/(XSize+XSeparator))+1;
			int NumY = (Height/(YSize+YSeparator))+1;
			if(XSeparator == 0) {NumX--;}
			if(YSeparator == 0) {NumY--;}
			
			for(int Y = 0; Y<NumY; Y++)
			{
				for(int X = 0; X<NumX; X++)
				{
					Image WorkingImage = new Image();
					WorkingImage.CopyFrom(Img);
					Godot.Rect2I Section = new Godot.Rect2I(X*(XSize+XSeparator-1), Y*(YSize+YSeparator-1), XSize, YSize);
					WorkingImage = WorkingImage.GetRegion(Section);
					Texture2D Frame = ImageTexture.CreateFromImage(WorkingImage);
					SpriteFrameSet.AddFrame("SpriteSheet", Frame);
				}
			}
			return SpriteFrameSet;
		}
		catch
		{
			if(Root != null) {Root.GetChild(0).GetChild<Console>(1).WriteToConsole("An error occured whilst converting an image to a spritesheet");}
			return null;
		}
	}
}
