using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DamageTextTMP;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] WorldTextSO worldTextSO;
    private RectTransform RT;
    private Vector3 AnimationVelocity;
    private Vector3 position;

    private void Start()
    {
        RT = GetComponent<RectTransform>();
    }
    // Start is called before the first frame update
    public void SpawnText(Vector3 pos, Elemental elemental, string text)
    {
        position = pos;
        DamageTextTMP.text = text;
        DamageTextTMP.color = worldTextSO.GetColor(elemental);
        StartCoroutine(WorldTextAnim());
    }

    private void Update()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(position);
        RT.anchoredPosition = new Vector2(pos.x, pos.y);
    }
    private IEnumerator WorldTextAnim()
    {
        float AnimationTime = 0.2f;
        float ElaspedTime = 0f;
        transform.localScale = Vector3.one * 5f;
        Vector3 target = Vector3.one;

        while (transform.localScale != target)
        {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, target, ref AnimationVelocity, AnimationTime - ElaspedTime);
            ElaspedTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(FadeOutAnimation());
    }


    private IEnumerator FadeOutAnimation()
    {
        canvasGroup.alpha = 1f;
        float ElaspedTime = 0f;
        float AnimationTime = 1f;
        yield return new WaitForSeconds(1.5f);
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, ElaspedTime / AnimationTime);
            ElaspedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
