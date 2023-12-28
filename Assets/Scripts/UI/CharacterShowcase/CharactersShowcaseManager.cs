using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharactersShowcaseManager : MonoBehaviour
{
    private static CharactersShowcaseManager instance;
    [SerializeField] CharacterInfoContentManager CharacterInfoContentManager;
    [SerializeField] CharactersArtifactsInfoContentManager CharactersArtifactsInfoContentManager;
    [SerializeField] EnhanceCharactersManager EnhanceCharactersManager;
    [SerializeField] GameObject CharactersIconPrefab;
    [SerializeField] TextMeshProUGUI CharacterNameText;
    [SerializeField] Image CharacterElemental;
    [SerializeField] Transform CharacterCharacterIconPlacementTransform;
    private Dictionary<CharacterIconButton, CharacterData> charactersDictionary;
    private CharacterIconButton SelectedCharacterIcon;
    private InventoryManager inventoryManager;

    private void Awake()
    {
        instance = this;
    }

    public static CharactersShowcaseManager GetInstance()
    {
        return instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        charactersDictionary = new();
        inventoryManager = InventoryManager.GetInstance();
        inventoryManager.GetPlayerStats().OnPlayerCharactersAdd += OnPlayerCharactersAdd;

        for (int i = 0; i < inventoryManager.GetPlayerStats().GetCharactersOwnedList().Count; i++)
        {
            OnPlayerCharactersAdd(inventoryManager.GetPlayerStats().GetCharactersOwnedList()[i]);
        }

        if (charactersDictionary.Count > 0)
        {
            KeyValuePair<CharacterIconButton, CharacterData> itemPair = charactersDictionary.ElementAt(0);
            OnCharacterButtonClick(itemPair.Key);
        }
    }

    void ResetAllIconButton()
    {
        foreach(var icon in charactersDictionary.Keys)
        {
            Color color = icon.GetComponent<Image>().color;
            color.a = 0.05f;
            icon.GetComponent<Image>().color = color;
        }
    }
    void OnPlayerCharactersAdd(CharacterData characterData)
    {
        CharacterIconButton CharacterIconButton = Instantiate(CharactersIconPrefab, CharacterCharacterIconPlacementTransform).GetComponent<CharacterIconButton>();
        CharacterIconButton.SetCharacterData(characterData);
        CharacterIconButton.OnCharacterButtonClick += OnCharacterButtonClick;
        charactersDictionary.Add(CharacterIconButton, CharacterIconButton.GetCharacterData());
    }

    public void RefreshCharacter()
    {
        OnCharacterButtonClick(SelectedCharacterIcon);
    }

    void OnCharacterButtonClick(CharacterIconButton c)
    {
        SelectedCharacterIcon = c;
        if (SelectedCharacterIcon)
        {
            ResetAllIconButton();
            Color color = SelectedCharacterIcon.GetComponent<Image>().color;
            color.a = 1f;
            SelectedCharacterIcon.GetComponent<Image>().color = color;

            ElementalColorSO.ElementalInfo e = ElementalReactionsManager.GetInstance().GetElementalColorSO().GetElementalInfo(SelectedCharacterIcon.GetCharacterData().GetPlayerCharacterSO().Elemental);
            CharacterNameText.text =  e.elementName + " / " + SelectedCharacterIcon.GetCharacterData().GetPlayerCharacterSO().ItemName;
            CharacterElemental.sprite = e.ElementSprite;

            CharacterInfoContentManager.Init(this, SelectedCharacterIcon.GetCharacterData());
            CharactersArtifactsInfoContentManager.Init(this, SelectedCharacterIcon.GetCharacterData());
            EnhanceCharactersManager.Init(this, SelectedCharacterIcon.GetCharacterData());
        }
    }

    public EnhanceCharactersManager GetEnhanceCharactersManager()
    {
        return EnhanceCharactersManager;
    }
    public CharacterIconButton GetSelectedCharacterData()
    {
        return SelectedCharacterIcon;
    }
}
