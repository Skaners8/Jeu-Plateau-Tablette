using UnityEngine;
using UnityEngine.UI;

public class ResourceHUD : MonoBehaviour
{
    public GameObject resourceSegmentPrefab; // Prefab for the small segment image
    public Transform mineralsBarContainer;   // Parent object for the minerals segments
    public Transform foodBarContainer;       // Parent object for the food segments
    public Transform pointsBarContainer;

    public Vector2 firstSegmentOffset = new Vector2(10, 0);  // Offset for the first segment
    public Vector2 segmentSpacing = new Vector2(10, 0);      // Offset between segments
    public Vector3 segmentScale = new Vector3(1f, 1f, 1f);   // Scale for each segment (default is 1)

    private PlayerActions playerActions;

    private void Start()
    {
        playerActions = FindObjectOfType<PlayerActions>();
        InitializeResourceBars();
        ApplyBarOffsets(); // Apply spacing between the resource bars
    }

    private void Update()
    {
        UpdateHUD();
    }

    // Initialize the resource bars
    void InitializeResourceBars()
    {
        CreateResourceBar(mineralsBarContainer, 30);   // Assuming 30 max minerals
        CreateResourceBar(foodBarContainer, 30);       // Assuming 30 max food
        CreateResourceBar(pointsBarContainer, 30);      // Assuming 30 max points
    }

    // Apply an offset between each resource bar container
    void ApplyBarOffsets()
    {
        // Position the resource bars with vertical spacing
        Vector3 basePosition = mineralsBarContainer.localPosition;

        foodBarContainer.localPosition = basePosition + new Vector3(0, -30f, 0);
    }

    // Create a resource bar with the appropriate segment offsets
    void CreateResourceBar(Transform container, int maxSegments)
    {
        // Clear any existing segments
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        Vector2 currentPosition = firstSegmentOffset;

        for (int i = 0; i < maxSegments; i++)
        {
            GameObject segment = Instantiate(resourceSegmentPrefab, container);
            RectTransform segmentRect = segment.GetComponent<RectTransform>();

            // Position and scale the first segment
            if (i == 0)
            {
                segmentRect.anchoredPosition = firstSegmentOffset;
            }
            else
            {
                // Position each segment based on the previous one
                segmentRect.anchoredPosition = currentPosition;
            }

            // Apply the scale to the segment
            segmentRect.localScale = segmentScale;

            // Update the position for the next segment (shift by segmentSpacing)
            currentPosition += segmentSpacing;

            segment.SetActive(false); // Start with all segments inactive
        }
    }

    // Update the HUD based on current resource values
    public void UpdateHUD()
    {
        // Update segments based on the player's current resources and points
        UpdateResourceSegments(mineralsBarContainer, playerActions.minerals);
        UpdateResourceSegments(foodBarContainer, playerActions.food);
        UpdateResourceSegments(pointsBarContainer, playerActions.points);
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