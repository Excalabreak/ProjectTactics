using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Original Tutorial Author: Queble 
 * Last Updated: [08/13/2025]
 * [holds data from jsons]
 */

public partial class StaticData : Node
{
    protected Dictionary<string, Dictionary<string,Variant>> dataDictionary;

    [Export] protected string _dataFilePath;

    /// <summary>
    /// calls to load all data
    /// </summary>
    public override void _Ready()
    {
        LoadJsonFile(_dataFilePath);
    }

    /// <summary>
    /// loads file json to item data (will change later)
    /// </summary>
    /// <param name="filePath">path to json file</param>
    protected void LoadJsonFile(string filePath)
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
        dataDictionary = JSON.Data.AsGodotDictionary<string, Dictionary<string, Variant>>();
    }

    /// <summary>
    /// gets the data of key in the dictionary
    /// </summary>
    /// <param name="key">key of dictionary</param>
    /// <returns>data of key</returns>
    public Dictionary<string, Variant> GetData(string key)
    {
        if (!dataDictionary.ContainsKey(key))
        {
            GD.Print("key not found: " + key);
            return null;
        }
        return dataDictionary[key];
    }
}
