using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementsIndicator : MonoBehaviour
{
    [SerializeField] Transform ElementParent;
    [SerializeField] GameObject ElementImagePrefab;
    [SerializeField] CanvasGroup CanvasGroup;
    private IDamage IDamage;
    private Coroutine FadeOutCoroutine;

    private void Update()
    {
        if (IDamage == null)
        {
            FadeOut();
        }
        else
        {
            if (IDamage.GetElementalReaction().GetElementList().Count == 0)
            {
                FadeOut();
            }
        }
    }

    private void FadeOut()
    {
        if (FadeOutCoroutine == null)
            FadeOutCoroutine = StartCoroutine(FadeOutAnimation());
    }
    private IEnumerator FadeOutAnimation()
    {
        CanvasGroup.alpha = 1f;
        float ElaspedTime = 0f;
        float AnimationTime = 1f;
        yield return new WaitForSeconds(0.35f);
        while (CanvasGroup.alpha > 0)
        {
            CanvasGroup.alpha = Mathf.Lerp(CanvasGroup.alpha, 0f, ElaspedTime / AnimationTime);
            ElaspedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    private IEnumerator FadeInAnimation()
    {
        CanvasGroup.alpha = 0f;
        float ElaspedTime = 0f;
        float AnimationTime = 0.4f;
        while (CanvasGroup.alpha < 1)
        {
            CanvasGroup.alpha = Mathf.Lerp(CanvasGroup.alpha, 1f, ElaspedTime / AnimationTime);
            ElaspedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void SetIDamage(IDamage IDamage)
    {
        this.IDamage = IDamage;
        if (IDamage.GetElementalReaction() != null)
        {
            IDamage.GetElementalReaction().onElementChanged += OnElementChanged;
            OnElementChanged(true);
        }
    }

    private void OnElementChanged(bool isChanged)
    {
        if (IDamage != null && isChanged) {
            if (FadeOutCoroutine != null)
            {
                StopCoroutine(FadeOutCoroutine);
                FadeOutCoroutine = null;
            }

            foreach(RectTransform GO in ElementParent)
                Destroy(GO.gameObject);

            if (IDamage.GetElementalReaction() != null)
            {
                for (int i = 0; i < IDamage.GetElementalReaction().GetElementList().Count; i++)
                {
                    Elements e = IDamage.GetElementalReaction().GetElementList()[i];
                    ElementalColorSO.ElementalInfo ElementalInfo = ElementalReactionsManager.GetInstance().GetElementalColorSO().GetElementalInfo(e.GetElements());

                    if (ElementalInfo != null)
                    {
                        GameObject go = Instantiate(ElementImagePrefab, ElementParent);
                        go.GetComponent<Image>().sprite = ElementalInfo.ElementSprite;
                        StartCoroutine(FadeInAnimation());
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (IDamage != null)
        {
            foreach (Transform GO in ElementParent)
                Destroy(GO.gameObject);

            if (IDamage.GetElementalReaction() != null)
                IDamage.GetElementalReaction().onElementChanged -= OnElementChanged;
        }
    }
}
