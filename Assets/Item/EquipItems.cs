using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipItems : MonoBehaviour, IPointerClickHandler
{
    private Characters currentCharacterREF;
    [SerializeField] TextMeshProUGUI EquipTxt;
    private Item itemREF;

    public void SetItemREF(Item item)
    {
        itemREF = item;
        if (itemREF == null || !(itemREF is UpgradableItems))
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        switch(itemREF.GetCategory)
        {
            case Category.ARTIFACTS:
                Artifacts artifacts = itemREF as Artifacts;
                if (artifacts != null)
                {
                    Artifacts ExistArtifacts = currentCharacterREF.CheckIfArtifactTypeExist(artifacts.GetArtifactType());

                    if (artifacts.GetCharacter() == null)
                    {
                        artifacts.SetEquippedCharacter(currentCharacterREF);
                        currentCharacterREF.GetEquippedArtifactsList().Add(artifacts);
                    }
                    else
                    {
                        currentCharacterREF.GetEquippedArtifactsList().Remove(artifacts);
                        artifacts.SetEquippedCharacter(null);
                    }

                    if (ExistArtifacts != null)
                    {
                        currentCharacterREF.GetEquippedArtifactsList().Remove(ExistArtifacts);
                        ExistArtifacts.SetEquippedCharacter(null);
                    }
                }
                break;
        }
    }


    // Update is called once per frame
    void Update()
    {
        currentCharacterREF = CharacterManager.GetInstance().GetCurrentCharacter();

        if (itemREF != null)
        {
            switch (itemREF.GetCategory)
            {
                case Category.ARTIFACTS:
                    Artifacts artifacts = itemREF as Artifacts;
                    if (artifacts != null)
                    {
                        if (artifacts.GetCharacter() == null)
                        {
                            EquipTxt.text = "Equip";
                        }
                        else
                        {
                            EquipTxt.text = "Remove";
                        }
                    }
                    break;
            }
        }
    }
}
