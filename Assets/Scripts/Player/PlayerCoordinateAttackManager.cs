using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoordinateAttackManager : MonoBehaviour
{
    private List<PlayerCharacters> PlayerCharactersList;
    public static event Action OnCoordinateAttack;

    public static void 
        CallCoordinateAttack()
    {
        OnCoordinateAttack?.Invoke();
    }

    private void Awake()
    {
        PlayerCharactersList = new();
    }

    public void Subscribe(PlayerCharacters c)
    {
        if (c == null)
            return;

        if (IsExisted(c) == null) {
            PlayerCharactersList.Add(c);
        }
    }

    // Update is called once per frame
    void Update()
    {
        RemoveWhenCoordinateAttackEnded();
    }

    private void RemoveWhenCoordinateAttackEnded()
    {
        for (int i = 0; i < PlayerCharactersList.Count; i++)
        {
            ICoordinateAttack ca = PlayerCharactersList[i].GetComponent<ICoordinateAttack>();
            if (ca != null)
            {
                ca.UpdateCoordinateAttack();
                if (ca.CoordinateAttackEnded())
                {
                    PlayerCharactersList.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    private PlayerCharacters IsExisted(PlayerCharacters characters)
    {
        for (int i = 0; i < PlayerCharactersList.Count; i++)
        {
            PlayerCharacters c = PlayerCharactersList[i];
            if (c.GetPlayersSO() == characters.GetPlayersSO())
            {
                return c;
            }
        }
        return null;
    }
}
