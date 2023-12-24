using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerElementalSkillandBurstManager : MonoBehaviour
{
    private List<PlayerCharacters> PlayerElementalSkillStateList;
    private List<PlayerCharacters> PlayerElementalBurstStateList;
    private List<PlayerCharacters> PlayerCharacterList;

    // Start is called before the first frame update
    void Start()
    {
        PlayerElementalSkillStateList = new();
        PlayerElementalBurstStateList = new();
        PlayerCharacterList = new();
    }

    public void Subscribe(PlayerCharacters p)
    {
        PlayerCharacterList.Add(p);
    }

    public void SubscribeSkillsState(PlayerCharacters c)
    {
        if (c == null)
            return;

        if (IsSkillStateExisted(c) == null)
        {
            PlayerElementalSkillStateList.Add(c);
        }
    }


    public void SubscribeBurstState(PlayerCharacters c)
    {
        if (c == null)
            return;

        if (IsBurstStateExisted(c) == null)
        {
            PlayerElementalBurstStateList.Add(c);
        }
    }

    private PlayerCharacters IsSkillStateExisted(PlayerCharacters characters)
    {
        for (int i = 0; i < PlayerElementalSkillStateList.Count; i++)
        {
            PlayerCharacters c = PlayerElementalSkillStateList[i];
            if (c.GetPlayersSO() == characters.GetPlayersSO())
            {
                return c;
            }
        }
        return null;
    }

    private PlayerCharacters IsBurstStateExisted(PlayerCharacters characters)
    {
        for (int i = 0; i < PlayerElementalBurstStateList.Count; i++)
        {
            PlayerCharacters c = PlayerElementalBurstStateList[i];
            if (c.GetPlayersSO() == characters.GetPlayersSO())
            {
                return c;
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateESkills();
        UpdateBurstSkills();
        UpdateCharactersEverytime();
    }

    void UpdateCharactersEverytime()
    {
        for (int i = 0; i < PlayerCharacterList.Count; i++)
        {
            PlayerCharacterList[i].UpdateEveryTime();
        }
    }

    void UpdateESkills()
    {
        for (int i = 0; i < PlayerElementalSkillStateList.Count; i++)
        {
            ISkillsBurstManager ca = PlayerElementalSkillStateList[i].GetComponent<ISkillsBurstManager>();
            if (ca != null)
            {
                ca.UpdateISkills();
                if (ca.ISkillsEnded())
                {
                    PlayerElementalSkillStateList.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    void UpdateBurstSkills()
    {
        for (int i = 0; i < PlayerElementalBurstStateList.Count; i++)
        {
            ISkillsBurstManager ca = PlayerElementalBurstStateList[i].GetComponent<ISkillsBurstManager>();
            if (ca != null)
            {
                ca.UpdateIBursts();
                if (ca.IBurstEnded())
                {
                    PlayerElementalBurstStateList.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
