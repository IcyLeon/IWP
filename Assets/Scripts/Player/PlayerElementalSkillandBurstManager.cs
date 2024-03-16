using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerElementalSkillandBurstManager
{
    private Dictionary<PlayerCharacterSO, PlayerCharacters> PlayerElementalSkillStateList;
    private Dictionary<PlayerCharacterSO, PlayerCharacters> PlayerElementalBurstStateList;
    private Dictionary<PlayerCharacterSO, PlayerCharacters> PlayerCharacterList;
    public static event Action OnCoordinateAttack;

    public static void CallCoordinateAttack()
    {
        OnCoordinateAttack?.Invoke();
    }

    // Start is called before the first frame update
    public PlayerElementalSkillandBurstManager()
    {
        PlayerElementalSkillStateList = new();
        PlayerElementalBurstStateList = new();
        PlayerCharacterList = new();
    }

    public void Subscribe(PlayerCharacters p)
    {
        PlayerCharacterList.Add(p.GetPlayersSO(), p);
    }

    public void SubscribeSkillsState(PlayerCharacters c)
    {
        if (c == null)
            return;

        if (IsSkillStateExisted(c) == null)
        {
            PlayerElementalSkillStateList.Add(c.GetPlayersSO(), c);
        }
    }


    public void SubscribeBurstState(PlayerCharacters c)
    {
        if (c == null)
            return;

        if (IsBurstStateExisted(c) == null)
        {
            PlayerElementalBurstStateList.Add(c.GetPlayersSO(), c);
        }
    }

    private PlayerCharacters IsSkillStateExisted(PlayerCharacters characters)
    {
        if (PlayerElementalSkillStateList.TryGetValue(characters.GetPlayersSO(), out PlayerCharacters playerCharacters))
        {
            return playerCharacters;
        }
        return null;
    }

    private PlayerCharacters IsBurstStateExisted(PlayerCharacters characters)
    {
        if (PlayerElementalBurstStateList.TryGetValue(characters.GetPlayersSO(), out PlayerCharacters playerCharacters))
        {
            return playerCharacters;
        }
        return null;
    }

    // Update is called once per frame
    public void UpdateAll()
    {
        UpdateESkills();
        UpdateBurstSkills();
        UpdateCharactersEverytime();
    }

    private void UpdateCharactersEverytime()
    {
        for (int i = 0; i < PlayerCharacterList.Count; i++)
        {
            KeyValuePair<PlayerCharacterSO, PlayerCharacters> PlayerCharacterkeyValuePair = PlayerCharacterList.ElementAt(i);
            PlayerCharacterkeyValuePair.Value.UpdateEveryTime();
        }
    }

    private void UpdateESkills()
    {
        for (int i = 0; i < PlayerElementalSkillStateList.Count; i++)
        {
            KeyValuePair<PlayerCharacterSO, PlayerCharacters> PlayerCharacterkeyValuePair = PlayerElementalSkillStateList.ElementAt(i);
            if (PlayerElementalSkillStateList.TryGetValue(PlayerCharacterkeyValuePair.Key, out PlayerCharacters PC))
            {
                ISkillsBurstManager ca = PC.GetComponent<ISkillsBurstManager>();
                if (ca != null)
                {
                    ca.UpdateISkills();
                    if (ca.IsISkillsEnded())
                    {
                        PlayerElementalSkillStateList.Remove(PlayerCharacterkeyValuePair.Key);
                        i--;
                    }
                }
            }
        }
    }

    private void UpdateBurstSkills()
    {
        for (int i = 0; i < PlayerElementalBurstStateList.Count; i++)
        {
            KeyValuePair<PlayerCharacterSO, PlayerCharacters> PlayerCharacterkeyValuePair = PlayerElementalBurstStateList.ElementAt(i);
            if (PlayerElementalBurstStateList.TryGetValue(PlayerCharacterkeyValuePair.Key, out PlayerCharacters PC))
            {
                ISkillsBurstManager ca = PC.GetComponent<ISkillsBurstManager>();
                if (ca != null)
                {
                    ca.UpdateIBursts();
                    if (ca.IsIBurstEnded())
                    {
                        PlayerElementalBurstStateList.Remove(PlayerCharacterkeyValuePair.Key);
                        i--;
                    }
                }
            }
        }
    }
}
