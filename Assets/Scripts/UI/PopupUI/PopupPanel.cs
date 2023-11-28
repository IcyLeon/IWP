using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupPanel : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI MessageText;
    [SerializeField] float animationDuration;
    private Vector2 TargetSize;
    private RectTransform RectTransform;
    private Vector2 OriginalSize;
    private Coroutine Popout;

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        TargetSize = RectTransform.sizeDelta;
        OriginalSize = TargetSize / 1.1f;
    }
    private void Start()
    {
        RectTransform.sizeDelta = OriginalSize;
    }

    public void SetMessage(string text)
    {
        MessageText.text = text;
        ShowPopup();
    }


    private void ShowPopup()
    {
        if (Popout != null)
        {
            StopCoroutine(Popout);
        }

        Popout = StartCoroutine(AnimatePopup(true));
    }

    private IEnumerator AnimatePopup(bool fadeIn)
    {
        float elapsedTime = 0f;
        Vector2 startSize = fadeIn ? OriginalSize : TargetSize;
        Vector2 endSize = fadeIn ? TargetSize : OriginalSize;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        while (elapsedTime < animationDuration)
        {
            float normalizedTime = elapsedTime / animationDuration;
            float t = Mathf.SmoothStep(0f, 1f, normalizedTime);

            RectTransform.sizeDelta = Vector2.Lerp(startSize, endSize, t);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (!fadeIn)
        {
            Destroy(gameObject);
        }

        float timer = 0f;
        float waitTime = 2.5f;

        while (timer < waitTime)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        StartCoroutine(AnimatePopup(false));
    }
}
