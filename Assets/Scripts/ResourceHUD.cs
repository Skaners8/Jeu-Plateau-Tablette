using UnityEngine;
using System.Collections.Generic;

public class ResourceHUD : MonoBehaviour
{
    [Header("Player 1 Prefabs")]
    public GameObject player1ResourceSegmentPrefab; // Prefab for Player 1's segments
    public GameObject player1MineralsIconPrefab;    // Prefab for Player 1's minerals icon
    public GameObject player1FoodIconPrefab;        // Prefab for Player 1's food icon
    public GameObject player1PointsIconPrefab;      // Prefab for Player 1's points icon

    [Header("Player 2 Prefabs")]
    public GameObject player2ResourceSegmentPrefab; // Prefab for Player 2's segments
    public GameObject player2MineralsIconPrefab;    // Prefab for Player 2's minerals icon
    public GameObject player2FoodIconPrefab;        // Prefab for Player 2's food icon
    public GameObject player2PointsIconPrefab;      // Prefab for Player 2's points icon

    [Header("Player 3 Prefabs")]
    public GameObject player3ResourceSegmentPrefab; // Prefab for Player 3's segments
    public GameObject player3MineralsIconPrefab;    // Prefab for Player 3's minerals icon
    public GameObject player3FoodIconPrefab;        // Prefab for Player 3's food icon
    public GameObject player3PointsIconPrefab;      // Prefab for Player 3's points icon

    [Header("Player 4 Prefabs")]
    public GameObject player4ResourceSegmentPrefab; // Prefab for Player 4's segments
    public GameObject player4MineralsIconPrefab;    // Prefab for Player 4's minerals icon
    public GameObject player4FoodIconPrefab;        // Prefab for Player 4's food icon
    public GameObject player4PointsIconPrefab;      // Prefab for Player 4's points icon

    [Header("General Settings")]
    public Camera mainCamera;                // Reference to the main camera
    public int currentTurn = 0;              // Current turn index

    [Header("Bar Layout Settings")]
    public Vector2 mineralsBarOffset = new Vector2(-300, 200); // Offset for the minerals bar
    public Vector2 foodBarOffset = new Vector2(-300, 150);     // Offset for the food bar
    public Vector2 pointsBarOffset = new Vector2(-300, 100);   // Offset for the points bar

    public Vector2 firstSegmentOffset = new Vector2(10, 0);  // Offset for the first segment
    public Vector2 segmentSpacing = new Vector2(10, 0);      // Offset between segments
    public Vector3 segmentScale = new Vector3(1f, 1f, 1f);   // Scale for each segment
    public Vector2 iconOffset = new Vector2(-30, 0);         // Offset for the resource icon relative to the first segment
    public Vector3 iconScale = new Vector3(1.5f, 1.5f, 1.5f); // Scale for the resource icon

    private Transform mineralsBarContainer;
    private Transform foodBarContainer;
    private Transform pointsBarContainer;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Create resource bar containers
        mineralsBarContainer = CreateBarContainer("MineralsBar");
        foodBarContainer = CreateBarContainer("FoodBar");
        pointsBarContainer = CreateBarContainer("PointsBar");

        InitializeResourceBars();
    }

    private void LateUpdate()
    {
        UpdateHUD();
        UpdateBarPositions();
    }

    // Create a container for a resource bar
    private Transform CreateBarContainer(string name)
    {
        GameObject barContainer = new GameObject(name);
        barContainer.transform.SetParent(this.transform);
        RectTransform rect = barContainer.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        return barContainer.transform;
    }

    // Initialize the resource bars
    void InitializeResourceBars()
    {
        Player currentPlayer = GameManager.Instance?.GetCurrentPlayer();
        int playerIndex = GameManager.Instance?.currentPlayerIndex ?? 0;

        GameObject segmentPrefab = GetPlayerSpecificPrefab(playerIndex, "segment");
        GameObject mineralsIconPrefab = GetPlayerSpecificPrefab(playerIndex, "minerals");
        GameObject foodIconPrefab = GetPlayerSpecificPrefab(playerIndex, "food");
        GameObject pointsIconPrefab = GetPlayerSpecificPrefab(playerIndex, "points");

        CreateResourceBar(mineralsBarContainer, 30, mineralsIconPrefab, segmentPrefab);   // Assuming 30 max minerals
        CreateResourceBar(foodBarContainer, 30, foodIconPrefab, segmentPrefab);          // Assuming 30 max food
        CreateResourceBar(pointsBarContainer, 30, pointsIconPrefab, segmentPrefab);      // Assuming 30 max points
    }

    // Retrieve specific prefab for a player based on type
    private GameObject GetPlayerSpecificPrefab(int playerIndex, string type)
    {
        switch (type)
        {
            case "segment":
                return playerIndex switch
                {
                    0 => player1ResourceSegmentPrefab,
                    1 => player2ResourceSegmentPrefab,
                    2 => player3ResourceSegmentPrefab,
                    3 => player4ResourceSegmentPrefab,
                    _ => null
                };
            case "minerals":
                return playerIndex switch
                {
                    0 => player1MineralsIconPrefab,
                    1 => player2MineralsIconPrefab,
                    2 => player3MineralsIconPrefab,
                    3 => player4MineralsIconPrefab,
                    _ => null
                };
            case "food":
                return playerIndex switch
                {
                    0 => player1FoodIconPrefab,
                    1 => player2FoodIconPrefab,
                    2 => player3FoodIconPrefab,
                    3 => player4FoodIconPrefab,
                    _ => null
                };
            case "points":
                return playerIndex switch
                {
                    0 => player1PointsIconPrefab,
                    1 => player2PointsIconPrefab,
                    2 => player3PointsIconPrefab,
                    3 => player4PointsIconPrefab,
                    _ => null
                };
            default:
                return null;
        }
    }

    // Create a resource bar with the appropriate segment offsets
    void CreateResourceBar(Transform container, int maxSegments, GameObject iconPrefab, GameObject segmentPrefab)
    {
        // Clear any existing segments
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // Add the icon to the left of the first segment
        GameObject icon = Instantiate(iconPrefab, container);
        RectTransform iconRect = icon.GetComponent<RectTransform>();
        iconRect.anchoredPosition = firstSegmentOffset + iconOffset;
        iconRect.localScale = iconScale;

        Vector2 currentPosition = firstSegmentOffset;

        for (int i = 0; i < maxSegments; i++)
        {
            GameObject segment = Instantiate(segmentPrefab, container);
            RectTransform segmentRect = segment.GetComponent<RectTransform>();

            segmentRect.anchoredPosition = currentPosition;
            segmentRect.localScale = segmentScale;
            currentPosition += segmentSpacing;

            segment.SetActive(false); // Start with all segments inactive
        }
    }

    // Update the positions of the bars relative to the camera
    void UpdateBarPositions()
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(mainCamera.transform.position);

        UpdateBarPosition(mineralsBarContainer, screenPos, mineralsBarOffset);
        UpdateBarPosition(foodBarContainer, screenPos, foodBarOffset);
        UpdateBarPosition(pointsBarContainer, screenPos, pointsBarOffset);
    }

    void UpdateBarPosition(Transform container, Vector3 screenPos, Vector2 offset)
    {
        RectTransform containerRect = container.GetComponent<RectTransform>();
        containerRect.position = screenPos + new Vector3(offset.x, offset.y, 0);
    }

    // Update the HUD based on current resource values
    public void UpdateHUD()
    {
        if (GameManager.Instance == null) return;

        Player currentPlayer = GameManager.Instance.GetCurrentPlayer();
        if (currentPlayer == null) return;

        UpdateResourceSegments(mineralsBarContainer, currentPlayer.minerals);
        UpdateResourceSegments(foodBarContainer, currentPlayer.food);
        UpdateResourceSegments(pointsBarContainer, currentPlayer.points);
    }

    // Show or hide segments based on the player's current resource amount
    void UpdateResourceSegments(Transform container, int currentValue)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            container.GetChild(i).gameObject.SetActive(i < currentValue);
        }
    }
}
