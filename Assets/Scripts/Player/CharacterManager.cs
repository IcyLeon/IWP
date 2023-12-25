using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


[Serializable]
public class CharacterInfo
{
    public GameObject CharacterPrefab;
    public PlayerCharacterSO playersSO;
}

public class CharacterManager : MonoBehaviour
{
    private static CharacterManager instance;
    [SerializeField] List<CharacterInfo> charactersInfo = new();
    private PlayerManager playerManager;
    public event Action OnSpawnCharacters;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public PlayerManager GetPlayerManager()
    {
        return playerManager;
    }
    public void SetPlayerManager(PlayerManager pm)
    {
        playerManager = pm;
    }

    public CharacterInfo GetCharacterInfo(PlayerCharacterSO playersSO)
    {
        for (int i = 0; i < charactersInfo.Count; i++)
        {
            if (charactersInfo[i].playersSO == playersSO)
                return charactersInfo[i];
        }
        return null;
    }

    public List<CharacterInfo> GetCharacterList()
    {
        return charactersInfo;
    }

    public static CharacterManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<CharacterManager>();
        }
        return instance;
    }

    public void SpawnCharacters(List<CharacterData> ChararacterDataList)
    {
        for (int i = 0; i < ChararacterDataList.Count; i++)
        {
            CharacterInfo characterInfo = GetCharacterInfo(ChararacterDataList[i].GetItemSO() as PlayerCharacterSO);
            PlayerCharacters CurrentCharacter = Instantiate(characterInfo.CharacterPrefab, GetPlayerManager().transform).GetComponent<PlayerCharacters>();
            CurrentCharacter.SetCharacterData(ChararacterDataList[i]);
            GetPlayerManager().AddPlayerCharactersList(CurrentCharacter);
        }
        OnSpawnCharacters?.Invoke();
    }
}
