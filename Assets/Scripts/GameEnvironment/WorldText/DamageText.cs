using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class DamageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DamageTextTMP;
    [SerializeField] CanvasGroup canvasGroup;
    private Vector3 Offset;
    private RectTransform RT;
    private Vector3 AnimationVelocity;
    private Vector3 position;
    private Vector2 vel;

    private void Awake()
    {
        RT = GetComponent<RectTransform>();
        vel = Vector2.zero;
        Offset = Vector3.one * 1.5f;
        canvasGroup.alpha = 0f;
    }
    // Start is called before the first frame update
    public void SpawnText(Vector3 pos, Elemental elemental, string text)
    {
        DamageTextTMP.color = ElementalReactionsManager.GetInstance().GetElementalColorSO().GetElementalInfo(elemental).color;
        Init(pos, text);
        StartCoroutine(FadeInAnimation());
        StartCoroutine(WorldTextAnim(1.25f));
    }

    public void SpawnText(Vector3 pos, ElementalReactionState elementalReaction, string text)
    {
        DamageTextTMP.color = ElementalReactionsManager.GetInstance().GetElementalColorSO().GetElementalReactionInfo(elementalReaction).color;
        Init(pos, text);
        StartCoroutine(FadeInAnimation());
        StartCoroutine(WorldTextAnim(1f));
    }

    public void SpawnText(Vector3 pos, OthersState othersState, string text)
    {
        DamageTextTMP.color = ElementalReactionsManager.GetInstance().GetElementalColorSO().GetOthersInfo(othersState).color; ;
        Init(pos, text);
        StartCoroutine(FadeInAnimation());
        StartCoroutine(WorldTextAnim(1.25f));
        StartCoroutine(MoveUpAnimation());
    }


    private void Init(Vector3 pos, string text)
    {
        position = pos + new Vector3(Random.Range(-Offset.x, Offset.x), Random.Range(-Offset.y, Offset.y), Random.Range(-Offset.z, Offset.z));
        DamageTextTMP.text = text;

        UpdateText();
    }

    private void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(position);
        RT.anchoredPosition = new Vector2(pos.x, pos.y) + vel;
        DamageTextTMP.gameObject.SetActive(CameraManager.CheckIfInCameraView(position));
    }
    private IEnumerator WorldTextAnim(float targetSize)
    {
        float AnimationTime = 0.2f;
        float ElaspedTime = 0f;
        transform.localScale = Vector3.one * 3f;
        Vector3 target = Vector3.one * targetSize;

        while (transform.localScale != target)
        {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, target, ref AnimationVelocity, AnimationTime - ElaspedTime);
            ElaspedTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(FadeOutAnimation());
    }

    private IEnumerator MoveUpAnimation()
    {
        float ElaspedTime = 0f;
        float AnimationTime = 1f;
        while (ElaspedTime < AnimationTime)
        {
            vel.y += Time.deltaTime * 50f; 
            ElaspedTime += Time.deltaTime;
            yield return null;
        }
    }


    private IEnumerator FadeInAnimation()
    {
        float ElaspedTime = 0f;
        float AnimationTime = 0.15f;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1f, ElaspedTime / AnimationTime);
            ElaspedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutAnimation()
    {
        float ElaspedTime = 0f;
        float AnimationTime = 0.75f;
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(0.2f);
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0f, ElaspedTime / AnimationTime);
            ElaspedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
