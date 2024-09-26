using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class GameState
{
    public int currentTurn; // Le tour actuel
    public int victoryPoints; // Points de victoire du joueur
    public List<string> colonizedPlanets; // Liste des planètes colonisées
}

public class GameSaveManager : MonoBehaviour
{
    private string saveFilePath;

    private void Start()
    {
        saveFilePath = Application.persistentDataPath + "/gamesave.dat";
    }

    // Sauvegarder l'état de la partie
    public void SaveGame(int currentTurn, int victoryPoints, List<string> colonizedPlanets)
    {
        GameState gameState = new GameState();
        gameState.currentTurn = currentTurn;
        gameState.victoryPoints = victoryPoints;
        gameState.colonizedPlanets = colonizedPlanets;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveFilePath);
        bf.Serialize(file, gameState);
        file.Close();

        Debug.Log("Partie sauvegardée !");
    }

    // Charger l'état de la partie
    public GameState LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(saveFilePath, FileMode.Open);
            GameState loadedGameState = (GameState)bf.Deserialize(file);
            file.Close();

            Debug.Log("Partie chargée !");
            return loadedGameState;
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde trouvée !");
            return null;
        }
    }

    // Vérifier si une sauvegarde existe
    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    // Effacer la sauvegarde
    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Sauvegarde effacée !");
        }
    }
}