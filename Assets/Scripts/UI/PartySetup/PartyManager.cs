using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] GameObject PartyInfoPrefab;
    private Dictionary<CharacterData, PartyInfo> PlayerCharacterDisplay_Dictionary;
    private CharacterManager characterManager;
    private InventoryManager inventoryManager;

    // Start is called before the first frame update
    void Start()
    {
        PlayerCharacterDisplay_Dictionary = new();
        characterManager = CharacterManager.GetInstance();
        characterManager.OnSpawnCharacters += OnSpawnCharacters;
        inventoryManager = InventoryManager.GetInstance();
    }

    private void OnDestroy()
    {
        characterManager.OnSpawnCharacters -= OnSpawnCharacters;
    }

    void OnSpawnCharacters()
    {
        for (int i = PlayerCharacterDisplay_Dictionary.Count - 1; i > 0; i--)
        {
            KeyValuePair<CharacterData, PartyInfo> itemPair = PlayerCharacterDisplay_Dictionary.ElementAt(i);
            if (PlayerCharacterDisplay_Dictionary.TryGetValue(itemPair.Key, out PartyInfo partyInfo))
            {
                Destroy(partyInfo.gameObject);
                PlayerCharacterDisplay_Dictionary.Remove(itemPair.Key);
            }
        }
        PlayerCharacterDisplay_Dictionary.Clear();

        for(int i = 0; i < inventoryManager.GetCharactersOwnedList().Count; i++)
        {
            CharacterData characterData = inventoryManager.GetCharactersOwnedList()[i];
            PartyInfo partyInfo = Instantiate(PartyInfoPrefab, transform).GetComponent<PartyInfo>();
            partyInfo.SetCharacterData(characterData);
            partyInfo.SetKeyText((i + 1).ToString());
            PlayerCharacterDisplay_Dictionary.Add(characterData, partyInfo);
        }

    }
}
