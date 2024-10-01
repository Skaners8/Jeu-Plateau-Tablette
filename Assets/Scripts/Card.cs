using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // For Image component
using DG.Tweening;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 offset;
    private Vector3 originalPosition;
    public bool isSelected = false;
    public bool isDragging = false;
    public bool isFlipped = false;
    public float clickTime = 0f;
    public float clickDurationThreshold = 0.2f;
    private bool clickValid = true;

    public GameObject invisibleCard;
    public GameObject visibleCard;

    public Image rectoImage;
    public Image versoImage;
    private Image visibleImage;
    private Vector3 lastPosition;
    private float originalZPosition;

    // Tilt and oscillation parameters
    public float tiltAmount = 40f;
    public float oscillatorVelocity = 0f;
    private float displacement = 0f;
    public float spring = 150f;
    public float damp = 20f; // Increase damping to stop oscillation faster
    public float velocityThreshold = 0.01f; // Minimum velocity to stop oscillation
    public float tiltDamping = 5f;
    public float velocityMultiplier = 2f;

    public Shadow shadowScript;
    public RectTransform discardZoneRectTransform;
    public DeckManager deckManager;
    public bool isInDiscardZone = false;

    private bool isMovingToSlot = false; // New flag to control slot movement

    private void Start()
    {
        originalPosition = visibleCard.transform.localPosition;
        visibleImage = visibleCard.GetComponent<Image>();

        if (visibleImage == null)
        {
            Debug.LogError("L'Image est manquante sur l'enfant visible: " + visibleCard.name);
            return;
        }

        visibleImage.sprite = rectoImage.sprite;
        originalZPosition = visibleCard.transform.position.z;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!clickValid) return;

        isDragging = true;
        offset = visibleCard.transform.position - Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3 newPosition = visibleCard.transform.position;
        newPosition.z = 10;
        visibleCard.transform.position = newPosition;
        lastPosition = visibleCard.transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Handle the card's position update
        Vector3 newPos = Camera.main.ScreenToWorldPoint(eventData.position) + offset;
        newPos.z = 10;
        visibleCard.transform.position = newPos;

        // Update the tilt based on movement speed, but don't affect position
        float cardSpeedX = (visibleCard.transform.position.x - lastPosition.x) / Time.deltaTime;
        oscillatorVelocity -= cardSpeedX * velocityMultiplier; // Reversed tilt direction

        lastPosition = visibleCard.transform.position;

        if (RectTransformUtility.RectangleContainsScreenPoint(discardZoneRectTransform, eventData.position, Camera.main))
        {
            isInDiscardZone = true;
        }
        else
        {
            isInDiscardZone = false;
        }
    }

 private void Update()
    {
        // Handle oscillator rotation for tilt independently of dragging
        float force = -spring * displacement - damp * oscillatorVelocity;
        oscillatorVelocity += force * Time.deltaTime;
        displacement += oscillatorVelocity * Time.deltaTime;

        // Clamp tilt rotation
        float rotationZ = Mathf.Clamp(displacement, -tiltAmount, tiltAmount);
        visibleCard.transform.rotation = Quaternion.Euler(0, 0, rotationZ);

        // Check if the velocity is below the threshold, stop the oscillation
        if (Mathf.Abs(oscillatorVelocity) < velocityThreshold)
        {
            oscillatorVelocity = 0f; // Stop oscillation
            displacement = 0f;
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (isMovingToSlot) return; // Prevent interaction during slot movement
        clickTime = Time.time;
        clickValid = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isMovingToSlot) return; // Prevent interaction during slot movement

        if (Time.time - clickTime < clickDurationThreshold && !isDragging)
        {
            if (!isSelected)
            {
                visibleCard.transform.DOLocalMoveY(originalPosition.y + 0.75f, 0.2f);
                isSelected = true;
            }
            else
            {
                visibleCard.transform.DOLocalMove(originalPosition, 0.2f);
                isSelected = false;
            }

            // Temporarily disable interaction during flip
            this.enabled = false;

            // Handle card flipping
            if (!isFlipped)
            {
                visibleCard.transform.DOScaleX(visibleCard.transform.localScale.x * 0.1f, 0.25f).OnComplete(() =>
                {
                    visibleImage.sprite = versoImage.sprite;
                    visibleImage.raycastTarget = true;
                    visibleCard.transform.DOScaleX(visibleCard.transform.localScale.x * 10f, 0.25f).OnComplete(() =>
                    {
                        // Re-enable interaction after flip
                        this.enabled = true;
                    });
                });
                isFlipped = true;
            }
            else
            {
                visibleCard.transform.DOScaleX(visibleCard.transform.localScale.x * 0.1f, 0.25f).OnComplete(() =>
                {
                    visibleImage.sprite = rectoImage.sprite;
                    visibleImage.raycastTarget = true;
                    visibleCard.transform.DOScaleX(visibleCard.transform.localScale.x * 10f, 0.25f).OnComplete(() =>
                    {
                        // Re-enable interaction after flip
                        this.enabled = true;
                    });
                });
                isFlipped = false;
            }
        }
        else
        {
            clickValid = false;
        }

        isDragging = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        if (isInDiscardZone)
        {
            DiscardCard();
        }
        else
        {
            if (!isSelected)
            {
                visibleCard.transform.DOLocalMove(Vector3.zero, 0.5f);
            }
            else
            {
                visibleCard.transform.DOLocalMove(originalPosition + new Vector3(0, 0.75f, 0), 0.5f);
            }

            Vector3 newPosition = visibleCard.transform.position;
            newPosition.z = originalZPosition;
            visibleCard.transform.position = newPosition;
        }
    }

    public void DiscardCard()
    {
        visibleCard.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            deckManager.DrawNewCard(this.transform.parent);
        });
    }

// Method to start card movement with tilt even when not dragging
    public void MoveCardToPosition(Vector3 targetPosition)
    {
        lastPosition = visibleCard.transform.position;

        // Use DOTween to smoothly move the card, but leave tilt to Update
        visibleCard.transform.DOMove(targetPosition, 0.8f).OnUpdate(() =>
        {
            float cardSpeedX = (visibleCard.transform.position.x - lastPosition.x) / Time.deltaTime;
            oscillatorVelocity -= cardSpeedX * velocityMultiplier;
            lastPosition = visibleCard.transform.position;
        });
    }


}
