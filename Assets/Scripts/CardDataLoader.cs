using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CardDataLoader : MonoBehaviour
{
    public static Dictionary<int, (string title, string description)> CardData = new Dictionary<int, (string title, string description)>();

    private void Awake()
    {
        LoadCardData();
    }

    private void LoadCardData()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("CardData");
        if (csvFile == null)
        {
            Debug.LogError("CardData.csv not found in Resources folder.");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++) // Skip header line
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');
            if (fields.Length < 3) continue;

            int id = int.Parse(fields[0]);
            string title = fields[1].Trim();
            string description = fields[2].Trim();

            CardData[id] = (title, description);
        }
    }

    public static (string title, string description) GetCardData(int id)
    {
        if (CardData.TryGetValue(id, out var data))
        {
            return data;
        }

        Debug.LogError($"No card data found for ID {id}");
        return ("Unknown Title", "Unknown Description");
    }
}
