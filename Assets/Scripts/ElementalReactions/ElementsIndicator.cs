using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementsIndicator : MonoBehaviour
{
    [SerializeField] Transform ElementParent;
    [SerializeField] GameObject ElementImagePrefab;
    [SerializeField] CanvasGroup CanvasGroup;
    private Characters Characters;

    private void Update()
    {
        if (Characters == null)
        {
            StartCoroutine(FadeOutAnimation());
        }
        else
        {
            if (Characters.GetElementalReaction().GetElementList().Count == 0)
            {
                StartCoroutine(FadeOutAnimation());
            }
        }
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

    public void SetCharacters(Characters characters)
    {
        Characters = characters;
        Characters.GetElementalReaction().onElementChanged += OnElementChanged;
        OnElementChanged(true);
    }

    private void OnElementChanged(bool isChanged)
    {
        if (Characters != null && isChanged) {
            foreach(RectTransform GO in ElementParent)
                Destroy(GO.gameObject);

            for (int i = 0; i < Characters.GetElementalReaction().GetElementList().Count; i++)
            {
                Elements e = Characters.GetElementalReaction().GetElementList()[i];
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

    private void OnDestroy()
    {
        if (Characters != null)
        {
            foreach (Transform GO in ElementParent)
                Destroy(GO.gameObject);
            Characters.GetElementalReaction().onElementChanged -= OnElementChanged;
        }
    }
}
