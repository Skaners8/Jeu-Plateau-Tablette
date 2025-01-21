using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public float hoverScale = 1.1f; // Scale on hover
    public float clickScale = 0.9f; // Scale on click
    public float animationDuration = 0.2f; // Duration of scaling animation
    public AudioClip hoverSound;
    public AudioClip clickSound;
    private Vector3 originalScale;
    private AudioSource audioSource;
    private Coroutine currentCoroutine;

    private void Start()
    {
        originalScale = transform.localScale;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScaleAnimation(hoverScale);
        PlaySound(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScaleAnimation(1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        StartScaleAnimation(clickScale, () => StartScaleAnimation(1f));
        PlaySound(clickSound);
    }

    private void StartScaleAnimation(float targetScale, System.Action onComplete = null)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(ScaleAnimation(targetScale, onComplete));
    }

    private IEnumerator ScaleAnimation(float targetScale, System.Action onComplete = null)
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = originalScale * targetScale;
        float timeElapsed = 0f;

        while (timeElapsed < animationDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / animationDuration;
            transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            yield return null;
        }

        transform.localScale = endScale;
        onComplete?.Invoke();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
