using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCoordinateAttackManager : MonoBehaviour
{
    [SerializeField] Transform CoordinateAttackPivot;
    private List<PlayerCharacters> PlayerCharactersList;

    private void Awake()
    {
        PlayerCharactersList = new();
    }
    private void Start()
    {
        CharacterManager.GetInstance().GetPlayerController().OnChargeTrigger += OnCoordinateAttack;
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
        Debug.Log(PlayerCharactersList.Count);
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

    private void OnCoordinateAttack()
    {
        for (int i = 0; i < PlayerCharactersList.Count; i++)
        {
            switch(PlayerCharactersList[i])
            {
                case Amber amber when amber != null:
                    if (amber.CoordinateCanShoot())
                    {
                        CoordinateAttack ca = Instantiate(AssetManager.GetInstance().CoordinateAttackPrefab, GetCoordinateAttackPivot()).GetComponent<CoordinateAttack>();
                        ca.SetCharacterData(amber.GetCharacterData());
                    }
                    break;
            }
        }
    }


    public Transform GetCoordinateAttackPivot()
    {
        return CoordinateAttackPivot;
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
