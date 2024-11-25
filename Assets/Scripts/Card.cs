using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // For Image component
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Verification etat")]
    private Vector3 offset;
    private Vector3 defaultPosition;
    public bool isSelected = false;
    public bool isDragging = false;
    public float clickTime = 0f;
    public float clickDurationThreshold = 0.2f;
    private bool clickValid = true;

    public GameObject visibleCard;
    public Image rectoImage; // Use only one side of the card
    private Image visibleImage;
    private Vector3 lastPosition;
    private float originalZPosition;

    // Add card ID to identify each card
    [Header("Card Data")]
    public int cardID;

    // Text display parameters
    [Header("Texte Carte")]
    public Text cardTitleText;
    public string titleText = "Card Title";
    public float letterDisplaySpeed = 0.05f;

    public Text cardDescriptionText;
    public string descriptionText = "Card Description";
    public float descriptionDisplaySpeed = 0.02f;

    private Coroutine titleDisplayCoroutine;
    private Coroutine descriptionDisplayCoroutine;

    [Header("Mouvement de la carte")]
    public float tiltAmount = 40f;
    public float oscillatorVelocity = 0f;
    private float displacement = 0f;
    public float spring = 150f;
    public float damp = 20f;
    public float velocityThreshold = 0.01f;
    public float tiltDamping = 5f;
    public float velocityMultiplier = 2f;

    [Header("Ombre et discard")]

    public Shadow shadowScript;
    public RectTransform discardZoneRectTransform;
    public GameObject discardZone; // Reference to the discard zone GameObject
    public DeckManager deckManager;
    public bool isInDiscardZone = false;

    private bool isMovingToSlot = false;

    // Static state to track if any card is being dragged
    private static bool isAnyCardDragging = false;

    private void Start()
    {
        defaultPosition = visibleCard.transform.localPosition;
        visibleImage = visibleCard.GetComponent<Image>();

        if (visibleImage == null)
        {
            Debug.LogError("L'Image est manquante sur l'enfant visible: " + visibleCard.name);
            return;
        }

        visibleImage.sprite = rectoImage.sprite; // Only use the rectoImage
        originalZPosition = visibleCard.transform.position.z;

        // Load card text from CardDataLoader
        var cardData = CardDataLoader.GetCardData(cardID);
        titleText = cardData.title;
        descriptionText = cardData.description;

        if (cardTitleText != null)
        {
            cardTitleText.text = ""; // Clear initial text
        }

        if (cardDescriptionText != null)
        {
            cardDescriptionText.text = ""; // Clear initial description
        }

        // Ensure the discard zone is hidden at the start
        if (discardZone != null)
        {
            discardZone.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!clickValid) return;

        isDragging = true;
        isAnyCardDragging = true; // Set global dragging state
        offset = visibleCard.transform.position - Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3 newPosition = visibleCard.transform.position;
        newPosition.z = 10;
        visibleCard.transform.position = newPosition;
        lastPosition = visibleCard.transform.position;

        // Hide text while dragging
        HideCardText();

        // Show the discard zone when dragging starts
        if (discardZone != null)
        {
            discardZone.SetActive(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 newPos = Camera.main.ScreenToWorldPoint(eventData.position) + offset;
        newPos.z = 10;
        visibleCard.transform.position = newPos;

        float cardSpeedX = (visibleCard.transform.position.x - lastPosition.x) / Time.deltaTime;
        oscillatorVelocity -= cardSpeedX * velocityMultiplier;

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
        if (isAnyCardDragging)
        {
            HideCardText();
        }

        float force = -spring * displacement - damp * oscillatorVelocity;
        oscillatorVelocity += force * Time.deltaTime;
        displacement += oscillatorVelocity * Time.deltaTime;

        float rotationZ = Mathf.Clamp(displacement, -tiltAmount, tiltAmount);
        visibleCard.transform.rotation = Quaternion.Euler(0, 0, rotationZ);

        if (Mathf.Abs(oscillatorVelocity) < velocityThreshold)
        {
            oscillatorVelocity = 0f;
            displacement = 0f;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isMovingToSlot) return;
        clickTime = Time.time;
        clickValid = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isMovingToSlot) return;

        if (Time.time - clickTime < clickDurationThreshold && !isDragging)
        {
            if (!isSelected)
            {
                visibleCard.transform.DOLocalMoveX(defaultPosition.x + 1f, 0.2f);
                isSelected = true;
                ShowCardText();
            }
            else
            {
                visibleCard.transform.DOLocalMove(defaultPosition, 0.2f);
                isSelected = false;
                HideCardText();
            }
        }

        isDragging = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        isAnyCardDragging = false;

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
                visibleCard.transform.DOLocalMove(defaultPosition + new Vector3(0.75f, 0, 0), 0.5f);
            }

            Vector3 newPosition = visibleCard.transform.position;
            newPosition.z = originalZPosition;
            visibleCard.transform.position = newPosition;
        }

        // Hide the discard zone when dragging ends
        if (discardZone != null)
        {
            discardZone.SetActive(false);
        }

        RestoreCardText();
    }

    private void ShowCardText()
    {
        if (titleDisplayCoroutine != null)
        {
            StopCoroutine(titleDisplayCoroutine);
        }
        if (descriptionDisplayCoroutine != null)
        {
            StopCoroutine(descriptionDisplayCoroutine);
        }

        titleDisplayCoroutine = StartCoroutine(DisplayTextLetterByLetter(cardTitleText, titleText, letterDisplaySpeed));
        descriptionDisplayCoroutine = StartCoroutine(DisplayTextLetterByLetter(cardDescriptionText, descriptionText, descriptionDisplaySpeed));
    }

    private void HideCardText()
    {
        if (titleDisplayCoroutine != null)
        {
            StopCoroutine(titleDisplayCoroutine);
        }
        if (descriptionDisplayCoroutine != null)
        {
            StopCoroutine(descriptionDisplayCoroutine);
        }

        if (cardTitleText != null) cardTitleText.text = "";
        if (cardDescriptionText != null) cardDescriptionText.text = "";
    }

    private void RestoreCardText()
    {
        if (isSelected)
        {
            ShowCardText();
        }
    }

    private IEnumerator DisplayTextLetterByLetter(Text targetTextComponent, string fullText, float speed)
    {
        if (targetTextComponent != null)
        {
            targetTextComponent.text = "";
            foreach (char letter in fullText.ToCharArray())
            {
                targetTextComponent.text += letter;
                yield return new WaitForSeconds(speed);
            }
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
}
