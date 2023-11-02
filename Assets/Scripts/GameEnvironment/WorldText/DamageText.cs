using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DamageTextTMP;
    [SerializeField] CanvasGroup canvasGroup;
    private Vector3 Offset;
    private RectTransform RT;
    private Vector3 AnimationVelocity;
    private Vector3 position;

    private void Awake()
    {
        RT = GetComponent<RectTransform>();
        Offset = Vector3.one * 0.25f;
    }
    // Start is called before the first frame update
    public void SpawnText(Vector3 pos, Elemental elemental, string text)
    {
        DamageTextTMP.color = ElementalReactionsManager.GetInstance().GetElementalColorSO().GetElementalInfo(elemental).color;
        Init(pos, text);
        StartCoroutine(WorldTextAnim(1.55f, 1.75f));
    }

    public void SpawnText(Vector3 pos, ElementalReactionState elementalReaction, string text)
    {
        DamageTextTMP.color = ElementalReactionsManager.GetInstance().GetElementalColorSO().GetElementalReactionInfo(elementalReaction).color;
        Init(pos, text);
        StartCoroutine(WorldTextAnim(0.8f, 1.2f));
    }

    private void Init(Vector3 pos, string text)
    {
        position = pos + new Vector3(Random.Range(-Offset.x, Offset.x), Random.Range(-Offset.y, Offset.y), Random.Range(-Offset.z, Offset.z));
        DamageTextTMP.text = text;
    }

    private void Update()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(position);
        RT.anchoredPosition = new Vector2(pos.x, pos.y);
    }
    private IEnumerator WorldTextAnim(float Min, float Max)
    {
        float AnimationTime = 0.2f;
        float ElaspedTime = 0f;
        transform.localScale = Vector3.one * 7.5f;
        Vector3 target = Vector3.one * Random.Range(Min, Max);

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
        float AnimationTime = 0.8f;
        yield return new WaitForSeconds(0.3f);
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, ElaspedTime / AnimationTime);
            ElaspedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
