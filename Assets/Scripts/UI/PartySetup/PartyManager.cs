using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] GameObject PartyInfoPrefab;
    private Dictionary<CharacterData, PartyInfo> PlayerCharacterDisplay_Dictionary;

    private void Awake()
    {
        PlayerCharacterDisplay_Dictionary = new();
        CharacterManager.OnSpawnCharacters += OnSpawnCharacters;
    }

    private void OnDestroy()
    {
        CharacterManager.OnSpawnCharacters -= OnSpawnCharacters;
    }

    void OnSpawnCharacters(List<CharacterData> CharacterDataList)
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

        for(int i = 0; i < CharacterDataList.Count; i++)
        {
            CharacterData characterData = CharacterDataList[i];
            PartyInfo partyInfo = Instantiate(PartyInfoPrefab, transform).GetComponent<PartyInfo>();
            partyInfo.SetCharacterData(characterData);
            partyInfo.SetKeyText((i + 1).ToString());
            PlayerCharacterDisplay_Dictionary.Add(characterData, partyInfo);
        }

    }
}
