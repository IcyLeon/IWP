using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessageNotification : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI MessageText;
    [SerializeField] float animationDuration;
    [SerializeField] CanvasGroup canvasGroup;
    private Coroutine Popout;

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
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        while (!Mathf.Approximately(canvasGroup.alpha, endAlpha))
        {
            float normalizedTime = elapsedTime / animationDuration;
            float t = Mathf.SmoothStep(0f, 1f, normalizedTime);

            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        if (!fadeIn)
        {
            Destroy(gameObject);
        }

        yield return new WaitForSeconds(2.5f);
        StartCoroutine(AnimatePopup(false));
    }
}
