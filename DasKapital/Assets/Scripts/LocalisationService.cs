using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LocalisationService : MonoBehaviour
{
    public static LocalisationService instance;
    public string filePath = "Assets/Texts/fr.csv";
    public Dictionary<string, string> localisationLines = new Dictionary<string, string>();
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new System.Exception($"Too many {this} instances");
        }

        LoadFile(filePath);
    }

    void LoadFile(string _path)
    {
        TextAsset asset = (TextAsset)Resources.Load(filePath, typeof(TextAsset));
        string baseText = asset.text;
        string[] texts = baseText.Split("\n" [0]);

        for (var i = 0; i < texts.Length; i++)
        {
            string[] line = texts[i].Split(new string[] {","}, 2, System.StringSplitOptions.None);
            line[1] = line[1].Remove(line[1].Length - 1);
            line[1] = line[1].Trim(new System.Char[] {'"', ' '});
            if (line[1].Contains("*"))
            {
                string[] segmentedLine = line[1].Split(new string[] {"*"}, 3, System.StringSplitOptions.None);
                string key = line[0].Replace("SCE", "AST");
                line[1] = $"{segmentedLine[0]}<link=\"{key}\"><b><color=#1F6EBA>{segmentedLine[1]}*</color></b></link>{segmentedLine[2]}";
            }
            localisationLines.Add(line[0], line[1]);
        }
    }

    public string Translate(string _key)
    {
        return localisationLines[_key];
    }
}
