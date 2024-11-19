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

    // Text display parameters
    [Header("Texte Carte")]
    public Text cardTitleText; // Reference to the Text component for the title
    public string titleText = "Card Title"; // The full title to display
    public float letterDisplaySpeed = 0.05f; // Speed for displaying letters

    public Text cardDescriptionText; // Reference to the Text component for the description
    public string descriptionText = "Card Description"; // The full description to display
    public float descriptionDisplaySpeed = 0.02f; // Speed for displaying description letters

    private Coroutine titleDisplayCoroutine;
    private Coroutine descriptionDisplayCoroutine;

    // Tilt and oscillation parameters
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
    public DeckManager deckManager;
    public bool isInDiscardZone = false;

    private bool isMovingToSlot = false;

    // Static state to track if any card is being dragged
    private static bool isAnyCardDragging = false;

    // Static list to track all cards
    private static List<Card> allCards = new List<Card>();

    private void Start()
    {
        defaultPosition = visibleCard.transform.localPosition;
        visibleImage = visibleCard.GetComponent<Image>();

        if (visibleImage == null)
        {
            Debug.LogError("L'Image est manquante sur l'enfant visible: " + visibleCard.name);
            return;
        }

        visibleImage.sprite = rectoImage.sprite;
        originalZPosition = visibleCard.transform.position.z;

        if (cardTitleText != null)
        {
            cardTitleText.text = ""; // Clear initial text
        }

        if (cardDescriptionText != null)
        {
            cardDescriptionText.text = ""; // Clear initial description
        }

        // Add this card to the static list of all cards
        allCards.Add(this);
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
        HideAllTexts();
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
        // Hide all texts when any card is being dragged
        if (isAnyCardDragging)
        {
            HideCardText();
        }

        // Handle oscillator rotation for tilt independently of dragging
        float force = -spring * displacement - damp * oscillatorVelocity;
        oscillatorVelocity += force * Time.deltaTime;
        displacement += oscillatorVelocity * Time.deltaTime;

        // Apply displacement to the rotation
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
                ShowCardText(); // Show text when selected by the player
            }
            else
            {
                visibleCard.transform.DOLocalMove(defaultPosition, 0.2f);
                isSelected = false;
                HideCardText(); // Hide text when deselected
            }

            this.enabled = false;

            if (!isFlipped)
            {
                visibleCard.transform.DOScaleX(visibleCard.transform.localScale.x * 0.1f, 0.25f).OnComplete(() =>
                {
                    visibleImage.sprite = versoImage.sprite;
                    visibleImage.raycastTarget = true;
                    visibleCard.transform.DOScaleX(visibleCard.transform.localScale.x * 10f, 0.25f).OnComplete(() =>
                    {
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
        isAnyCardDragging = false; // Reset global dragging state

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

        // Restore all texts after dragging
        RestoreAllTexts();
    }

    private void ShowCardText()
    {
        if (isAnyCardDragging) return; // Don't show text if any card is dragging

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

        if (cardTitleText != null)
        {
            cardTitleText.text = "";
        }

        if (cardDescriptionText != null)
        {
            cardDescriptionText.text = "";
        }
    }

    private void HideAllTexts()
    {
        foreach (Card card in allCards)
        {
            if (card.gameObject.activeInHierarchy) // Skip inactive cards
            {
                card.HideCardText();
            }
        }
    }


    private void RestoreAllTexts()
    {
        foreach (Card card in allCards)
        {
            if (card.gameObject.activeInHierarchy && card.isSelected) // Skip inactive cards
            {
                card.ShowCardText();
            }
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


    public void MoveCardToPosition(Vector3 targetPosition)
    {
        lastPosition = visibleCard.transform.position;
        visibleCard.transform.DOMove(targetPosition, 0.8f).OnUpdate(() =>
        {
            float cardSpeedX = (visibleCard.transform.position.x - lastPosition.x) / Time.deltaTime;
            oscillatorVelocity -= cardSpeedX * velocityMultiplier;
            lastPosition = visibleCard.transform.position;
        });
    }
}