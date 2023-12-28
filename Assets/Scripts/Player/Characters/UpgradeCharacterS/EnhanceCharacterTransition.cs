using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnhanceCharacterTransition : MonoBehaviour, IPointerClickHandler
{
    private EnhanceCharactersManager enhanceCharactersManager;
    [SerializeField] GameObject CharacterShowcaseCanvas;
    [SerializeField] GameObject ItemShowcaseCanvas;

    public void OnPointerClick(PointerEventData eventData)
    {
        SpawnUI();
    }

    void Start()
    {
        enhanceCharactersManager = CharactersShowcaseManager.GetInstance().GetEnhanceCharactersManager();
    }
    void SpawnUI()
    {
        ItemShowcaseCanvas.transform.GetChild(0).gameObject.SetActive(false);
        CharacterShowcaseCanvas.transform.GetChild(0).gameObject.SetActive(false);
        enhanceCharactersManager.OpenEnhanceCharacterCanvas();
    }
}
