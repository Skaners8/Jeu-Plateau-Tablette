using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {
    public int currentTurn;
    public bool isPlayerTurn;

    void Start() {
        currentTurn = 0;
        isPlayerTurn = true; // Commence par le joueur
    }

    void EndTurn() {
        isPlayerTurn = !isPlayerTurn;
        currentTurn++;
        // Déclencher les actions pour l'IA ou le joueur en fonction du tour
    }
}