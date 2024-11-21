using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    [Header("Deck Settings")]
    public Transform cardParent; // The parent where the card should be placed
    public RectTransform discardZoneRectTransform; // Reference to the discard zone
    public Transform deckPosition; // The transform where the deck is located
    public int maxDeckSize = 30; // Maximum number of cards in the deck

    [Header("Deck Composition")]
    public Text deckCountText; // UI Text to display the number of remaining cards
    public List<CardEntry> initialDeck = new List<CardEntry>(); // Initial deck composition

    [Header("Deck Prefabs")]
    public GameObject[] prefabList; // Array of prefabs for different cards
    public int[] prefabCardIDs; // Array of corresponding IDs for each prefab
    private Dictionary<int, GameObject> cardPrefabs = new Dictionary<int, GameObject>(); // Dictionary to map ID to prefab

    [System.Serializable]
    public class CardEntry
    {
        public int cardID; // The ID of the card
        public int quantity; // Number of cards of this type in the deck
    }

    private List<int> cardPool = new List<int>(); // Pool of card IDs in the deck

    private void Awake()
    {
        // Populate the dictionary with prefab mappings
        if (prefabList.Length != prefabCardIDs.Length)
        {
            Debug.LogError("Prefab list and Card ID list must have the same length!");
            return;
        }

        for (int i = 0; i < prefabList.Length; i++)
        {
            cardPrefabs[prefabCardIDs[i]] = prefabList[i];
        }
    }

    private void Start()
    {
        InitializeDeck();
        UpdateDeckCountUI();
    }

    // Initialize the deck based on the initialDeck configuration
    private void InitializeDeck()
    {
        foreach (var entry in initialDeck)
        {
            for (int i = 0; i < entry.quantity; i++)
            {
                if (cardPool.Count < maxDeckSize)
                {
                    cardPool.Add(entry.cardID);
                }
                else
                {
                    Debug.LogWarning("Deck size exceeds the maximum allowed size.");
                    break;
                }
            }
        }

        ShuffleDeck();
    }

    // Shuffle the card pool
    private void ShuffleDeck()
    {
        for (int i = 0; i < cardPool.Count; i++)
        {
            int randomIndex = Random.Range(0, cardPool.Count);
            int temp = cardPool[i];
            cardPool[i] = cardPool[randomIndex];
            cardPool[randomIndex] = temp;
        }
    }

    // Draw a new card and place it in the specified target parent
    public void DrawNewCard(Transform targetParent)
    {
        if (cardPool.Count == 0)
        {
            Debug.LogWarning("The deck is empty. No cards to draw.");
            return;
        }

        // Get the next card ID from the pool
        int cardID = cardPool[0];
        cardPool.RemoveAt(0);

        // Find the correct prefab for this card ID
        GameObject selectedPrefab;
        if (!cardPrefabs.TryGetValue(cardID, out selectedPrefab))
        {
            Debug.LogError($"No prefab found for card ID: {cardID}");
            return;
        }

        // Instantiate the appropriate prefab at the deck position
        GameObject newCard = Instantiate(selectedPrefab, deckPosition.position, Quaternion.identity);

        // Set the card as a child of the target parent
        newCard.transform.SetParent(targetParent, worldPositionStays: true);

        // Animate the card moving from the deck to its target position (slot)
        Vector3 finalPosition = targetParent.position;
        newCard.transform.DOMove(finalPosition, 0.8f).OnComplete(() =>
        {
            // Set the card's local position relative to the new parent
            newCard.transform.localPosition = Vector3.zero;

            // Get the Card script component from the new card
            Card newCardScript = newCard.GetComponent<Card>();

            // Assign the card ID
            newCardScript.cardID = cardID;

            // Set up references for the new card
            newCardScript.discardZoneRectTransform = discardZoneRectTransform;
            newCardScript.deckManager = this;

            // Re-enable the card interactions after the movement is done
            newCardScript.enabled = true;
        });

        // Reset the local scale of the new card to ensure it appears at the correct size
        newCard.transform.localScale = Vector3.one;

        // Temporarily disable interaction on the card until the movement completes
        Card cardScript = newCard.GetComponent<Card>();
        cardScript.enabled = false;

        // Update the deck count UI
        UpdateDeckCountUI();
    }

    // Update the deck count in the UI
    private void UpdateDeckCountUI()
    {
        if (deckCountText != null)
        {
            deckCountText.text = $"Deck: {cardPool.Count} Cards";
        }
    }

    // Get the number of cards of a specific ID remaining in the deck
    public int GetCardCount(int cardID)
    {
        int count = 0;
        foreach (int id in cardPool)
        {
            if (id == cardID)
            {
                count++;
            }
        }
        return count;
    }
}
