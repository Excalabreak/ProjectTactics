using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: Queble 
 * Last Updated: [08/12/2025]
 * [holds data from jsons]
 */

public partial class StaticData : Node
{
    public Dictionary<string, Dictionary<string,Variant>> itemData;

    private string _dataFilePath = "res://StaticData/TestData.json";

    /// <summary>
    /// calls to load all data
    /// </summary>
    public override void _Ready()
    {
        LoadJsonFile(_dataFilePath);

        foreach (var item in itemData)
        {
            GD.Print("KEY " + item.Key + ":");
            foreach (var value in itemData[item.Key])
            {
                GD.Print(value.Key + ": " + value.Value);
            }
        }
    }

    /// <summary>
    /// loads file json to item data (will change later)
    /// </summary>
    /// <param name="filePath">path to json file</param>
    private void LoadJsonFile(string filePath)
    {
        if (!FileAccess.FileExists(filePath))
        {
            GD.Print("path doesn't exist: " + filePath);
            return;
        }

        FileAccess dataFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        string jsonString = dataFile.GetAsText();

        Json JSON = new Json();
        Error error = JSON.Parse(jsonString);

        if (error != Error.Ok)
        {
            GD.Print("error when parsing: " + filePath);
            return;
        }
        itemData = JSON.Data.AsGodotDictionary<string, Dictionary<string, Variant>>();
    }
}
