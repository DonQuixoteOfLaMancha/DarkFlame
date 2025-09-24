using Godot;
using System;
using System.Collections.Generic;

public static class DataStorageManager
{
	/*
	STRUCTURE:
	==TYPES STORED==
	 @TYPE HEADER@
	  <Property> : #VALUE#
	 @-TYPE FOOTER-@
	==-TYPES STORED-==
	*/
	// This script makes some use of information from https://docs.godotengine.org/en/latest/tutorials/io/runtime_file_loading_and_saving.html
	
	//Stores my own format, DonTxt, to a given file path (text file with custom extension dontxt)
	public static void StoreDonTxt(string FilePath, string Type, string DonTxt, string ReplacementCheckProperty = "", string ReplacementCheckValue = "")
	{
		string ExistingData  = ParseDonTxt(FilePath, Type);
		ExistingData = RemoveDonTxtEntryFromProperty(Type, ExistingData, ReplacementCheckProperty, ReplacementCheckValue);
		using var DataFile = FileAccess.Open(FilePath, FileAccess.ModeFlags.Write);
		string Content = "=="+Type+"==\n"+CombineDonTxt(ExistingData,DonTxt)+"\n==-"+Type+"-==";
		
		DataFile.StoreString(Content);
		DataFile.Flush();
	}
	
	//Reads a given file path and strips it to only include the first set of entries of the given type
	//where sets are defined between the == headers and footer
	public static string ParseDonTxt(string FilePath, string Type)
	{
		string DonTxt = "";
		if(FileAccess.FileExists(FilePath))
		{
			using var DataFile = FileAccess.Open(FilePath, FileAccess.ModeFlags.Read);
			string FileContents = DataFile.GetAsText();
			int StartIndex = -1;
			int EndIndex = -1;
			int SubStartIndex = -1;
			int SubEndIndex = -1;
			for(int Index = 0; Index<FileContents.Length; Index++)
			{
				if(Index < FileContents.Length-1)
				{
					if(FileContents.Substring(Index,2) == "==")
					{
						if(SubStartIndex == -1) {SubStartIndex = Index;}
						else {SubEndIndex = Index;}
					}
				}
				if(SubEndIndex != -1)
				{
					if(FileContents.Substring(SubStartIndex, SubEndIndex-SubStartIndex) == "=="+Type && StartIndex == -1)
					{StartIndex = SubEndIndex + 4;}
					else if(FileContents.Substring(SubStartIndex, SubEndIndex-SubStartIndex) == "==-"+Type+"-" && StartIndex != -1)
					{EndIndex = SubStartIndex-1;}
					SubStartIndex = -1; SubEndIndex = -1;
				}
				if(StartIndex != -1 && EndIndex != -1)
				{break;}
			}
			if(StartIndex > -1 && EndIndex > StartIndex) {DonTxt = FileContents.Substring(StartIndex,EndIndex-StartIndex);}
		}
		return DonTxt;
	}
	
	//Combines 2 DonTxt strings (This clearly doesn't really need to be a function but maybe I'll change it later)
	public static string CombineDonTxt(string DonTxt1, string DonTxt2)
	{
		if(DonTxt1 != "" && DonTxt2 != "") {return "\t"+DonTxt1+"\n"+DonTxt2;}
		else if(DonTxt1 == "") {return DonTxt2;}
		return DonTxt1;
	}
	
	//Converts an array of Properties and their Values (Data) to DonTxt
	public static string ConvertToDonTxt(string Type, string[,] PropertiesData)
	{
		string DonTxt = "";
		DonTxt += "\t@"+Type+"@\n";
		for(int Index = 0; Index<PropertiesData.GetLength(0); Index++)
		{
			if(PropertiesData[Index,1] == null || PropertiesData[Index,1] == "") {PropertiesData[Index, 1] = " ";}
			DonTxt += "\t\t<"+PropertiesData[Index,0]+"> : #"+PropertiesData[Index,1]+"#\n";
		}
		DonTxt += "\t@-"+Type+"-@";
		
		return DonTxt;
	}
	
	//Converts DonTxt to an array of Properties and their Values (Data)
	public static string[,] ConvertToPropertiesData(string DonTxt)
	{
		List<string[]> PropertiesDataList = new List<string[]>();
		string[] IndividualPrptyDta = new string[2];
		bool OpenProperty = false;
		bool OpenData = false;
		int PropertyOpen = -1;
		int PropertyClose = -1;
		int DataOpen = -1;
		int DataClose = -1;
		int Depth = 0;
		
		for(int Index = 0; Index<DonTxt.Length; Index++)
		{
			char IndexChar = DonTxt[Index];
			if(!OpenProperty && !OpenData)
			{
				if(IndexChar == '<') //Checks for the opening of a property declaration
				{PropertyOpen = Index+1; OpenProperty = true;}
				else if (IndexChar == '#') //Checks for the opening of a piece of data
				{DataOpen = Index+1; OpenData = true;}
			}
			else if(OpenProperty)
			{
				if(IndexChar == '>') //Checks for the closing of a property declaration
				{PropertyClose = Index; IndividualPrptyDta[0] = DonTxt.Substring(PropertyOpen, PropertyClose-PropertyOpen); OpenProperty = false;}
			}
			else if(OpenData)
			{
					if(((IndexChar == '<' && Depth == 0) || Index == DonTxt.Length-1) && DataClose>DataOpen) 
					{
						if(DataClose < DataOpen) {DataClose = Index;}
						IndividualPrptyDta[1] = DonTxt.Substring(DataOpen, DataClose-DataOpen);
						PropertiesDataList.Add([IndividualPrptyDta[0],IndividualPrptyDta[1]]);
						OpenData = false;
						PropertyOpen = Index+1;
						OpenProperty = true;
						continue;
					}
					if(Index<DonTxt.Length-1) {if(DonTxt.Substring(Index,2) == "@-") {Depth--; continue;}} //Prevents issues where there is dontxt within dontxt
					if(Index>0) {if(DonTxt.Substring(Index-1,2) == "-@") {Depth--; continue;}} //related to above
					if(IndexChar == '@') {Depth++; continue;} //related to above
					if(IndexChar == '#') {DataClose = Index; continue;} //Checks for the end of a piece of data
			}
		}
		string[,] PropertiesData = new string[PropertiesDataList.Count,2];
		for(int Index = 0; Index<PropertiesData.GetLength(0); Index++)
		{PropertiesData[Index,0] = PropertiesDataList[Index][0]; PropertiesData[Index,1] = PropertiesDataList[Index][1];}
		
		return PropertiesData;
	}
	
	//Gets the number of individual objects stored in a DonTxt string
	public static int GetNumberOfDonTxtEntries(string Type, string DonTxt)
	{
		int EntryCount = 0;
		for(int Index = 0; Index<DonTxt.Length-2-Type.Length; Index++)
		{if(DonTxt.Substring(Index,2+Type.Length) == "@"+Type+"@") {EntryCount++;}}
		return EntryCount;
	}
	
	//Gets an individual object stored in DonTxt
	public static string GetDonTxtEntryFromIndex(string Type, string DonTxt, int TargetIndex)
	{
		int CurrentEntryIndex = -1;
		int EntryOpen = -1;
		int EntryClose = -1;
		for(int Index = 0; Index<DonTxt.Length; Index++)
		{
			if(DonTxt.Substring(Index,2+Type.Length) == "@"+Type+"@")
			{
				CurrentEntryIndex++;
				if(CurrentEntryIndex == TargetIndex)
				{EntryOpen = Index;}
			}
			else if(DonTxt.Substring(Index,4+Type.Length) == "@-"+Type+"-@" && CurrentEntryIndex == TargetIndex)
			{
				EntryClose = Index+4+Type.Length;
				break;
			}
		}
		string TargetEntry = DonTxt.Substring(EntryOpen, EntryClose-EntryOpen);
		return TargetEntry;
	}
	
	//Removes a given index
	public static string RemoveDonTxtEntryAtIndex(string Type, string DonTxt, int TargetIndex)
	{
		if(TargetIndex < GetNumberOfDonTxtEntries(Type, DonTxt))
		{return DonTxt.Replace(GetDonTxtEntryFromIndex(Type, DonTxt, TargetIndex), "");}
		return DonTxt;
	}
	
	//Removes the first entry in which the value of the given property matches the given value
	public static string RemoveDonTxtEntryFromProperty(string Type, string DonTxt, string Property, string Value)
	{
		string[,] PropertiesData;
		for(int TargetIndex = 0; TargetIndex < GetNumberOfDonTxtEntries(Type, DonTxt); TargetIndex++)
		{
			PropertiesData = ConvertToPropertiesData(GetDonTxtEntryFromIndex(Type, DonTxt, TargetIndex));
			for(int PropertyIndex = 0; PropertyIndex < PropertiesData.GetLength(0); PropertyIndex++)
			{
				if(PropertiesData[PropertyIndex, 0] == Property)
				{
					if(PropertiesData[PropertyIndex, 1] == Value)
					{
						return DonTxt.Replace(GetDonTxtEntryFromIndex(Type, DonTxt, TargetIndex), "");
					}
					break;
				}
			}
		}
		return DonTxt;
	}
}
