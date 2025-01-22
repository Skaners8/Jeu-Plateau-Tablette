using UnityEngine;
using System.Collections.Generic;

public class ResourceHUD : MonoBehaviour
{
    [Header("Player 1 Prefabs")]
    public GameObject player1SegmentPrefab;  // Prefab for Player 1's segments
    public GameObject player1IconPrefab;     // Prefab for Player 1's icon

    [Header("Player 2 Prefabs")]
    public GameObject player2SegmentPrefab;  // Prefab for Player 2's segments
    public GameObject player2IconPrefab;     // Prefab for Player 2's icon

    [Header("Player 3 Prefabs")]
    public GameObject player3SegmentPrefab;  // Prefab for Player 3's segments
    public GameObject player3IconPrefab;     // Prefab for Player 3's icon

    [Header("Player 4 Prefabs")]
    public GameObject player4SegmentPrefab;  // Prefab for Player 4's segments
    public GameObject player4IconPrefab;     // Prefab for Player 4's icon

    [Header("General Settings")]
    public Camera mainCamera;                // Reference to the main camera

    [Header("Bar Layout Settings")]
    public Vector2 mineralsBarOffset = new Vector2(-300, 200); // Offset for the minerals bar
    public Vector2 foodBarOffset = new Vector2(-300, 150);     // Offset for the food bar
    public Vector2 pointsBarOffset = new Vector2(-300, 100);   // Offset for the points bar

    public Vector2 firstSegmentOffset = new Vector2(10, 0);  // Offset for the first segment
    public Vector2 segmentSpacing = new Vector2(10, 0);      // Offset between segments
    public Vector2 iconPosition = new Vector2(0, 400);       // Position for the independent icon
    public Vector3 segmentScale = new Vector3(1f, 1f, 1f);   // Scale for each segment
    public Vector3 iconScale = new Vector3(1.5f, 1.5f, 1.5f); // Scale for the icon

    private Transform mineralsBarContainer;
    private Transform foodBarContainer;
    private Transform pointsBarContainer;

    private GameObject currentIcon; // Independent icon for the current player

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        mineralsBarContainer = CreateBarContainer("MineralsBar");
        foodBarContainer = CreateBarContainer("FoodBar");
        pointsBarContainer = CreateBarContainer("PointsBar");

        if (mineralsBarContainer == null || foodBarContainer == null || pointsBarContainer == null)
        {
            Debug.LogError("One or more resource bar containers were not initialized properly.");
        }

        UpdateBarsForCurrentPlayer();
    }
    private void Awake()
    {
        // Initialize the bar containers
        mineralsBarContainer = CreateBarContainer("MineralsBar");
        foodBarContainer = CreateBarContainer("FoodBar");
        pointsBarContainer = CreateBarContainer("PointsBar");
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

    public void UpdateBarsForCurrentPlayer()
    {
        int playerIndex = GameManager.Instance?.currentPlayerIndex ?? 0;

        GameObject segmentPrefab = GetPlayerSegmentPrefab(playerIndex);
        GameObject iconPrefab = GetPlayerIconPrefab(playerIndex);

        if (mineralsBarContainer == null || foodBarContainer == null || pointsBarContainer == null)
        {
            Debug.LogError("Resource bar containers are not initialized.");
            return;
        }

        if (segmentPrefab == null)
        {
            Debug.LogError($"No segment prefab assigned for player {playerIndex + 1}.");
            return;
        }

        UpdateResourceBar(mineralsBarContainer, 30, segmentPrefab);
        UpdateResourceBar(foodBarContainer, 30, segmentPrefab);
        UpdateResourceBar(pointsBarContainer, 30, segmentPrefab);

        UpdateIndependentIcon(iconPrefab);
    }


    private GameObject GetPlayerSegmentPrefab(int playerIndex)
    {
        GameObject prefab = playerIndex switch
        {
            0 => player1SegmentPrefab,
            1 => player2SegmentPrefab,
            2 => player3SegmentPrefab,
            3 => player4SegmentPrefab,
            _ => null
        };

        if (prefab == null)
        {
            Debug.LogError($"Segment prefab for player {playerIndex + 1} is not assigned.");
        }

        return prefab;
    }

    private GameObject GetPlayerIconPrefab(int playerIndex)
    {
        GameObject prefab = playerIndex switch
        {
            0 => player1IconPrefab,
            1 => player2IconPrefab,
            2 => player3IconPrefab,
            3 => player4IconPrefab,
            _ => null
        };

        if (prefab == null)
        {
            Debug.LogError($"Icon prefab for player {playerIndex + 1} is not assigned.");
        }

        return prefab;
    }
    // Update a resource bar with the appropriate segment offsets
    private void UpdateResourceBar(Transform container, int maxSegments, GameObject segmentPrefab)
    {
        // Clear any existing children
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

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

    // Update the independent icon for the current player
    private void UpdateIndependentIcon(GameObject iconPrefab)
    {
        if (currentIcon != null)
        {
            Destroy(currentIcon);
        }

        if (iconPrefab != null)
        {
            currentIcon = Instantiate(iconPrefab, this.transform);
            RectTransform iconRect = currentIcon.GetComponent<RectTransform>();
            iconRect.SetParent(this.transform, false);
            iconRect.anchoredPosition = iconPosition;
            iconRect.localScale = iconScale;
        }
    }

    // Update the positions of the bars relative to the camera
    private void UpdateBarPositions()
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(mainCamera.transform.position);

        UpdateBarPosition(mineralsBarContainer, screenPos, mineralsBarOffset);
        UpdateBarPosition(foodBarContainer, screenPos, foodBarOffset);
        UpdateBarPosition(pointsBarContainer, screenPos, pointsBarOffset);
    }

    private void UpdateBarPosition(Transform container, Vector3 screenPos, Vector2 offset)
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
    private void UpdateResourceSegments(Transform container, int currentValue)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            container.GetChild(i).gameObject.SetActive(i < currentValue);
        }
    }
}
