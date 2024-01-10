using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipItems : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] CharactersShowcaseManager CharactersShowcaseManager;
    private CharacterData currentCharacterREF;
    [SerializeField] TextMeshProUGUI EquipTxt;
    private Item itemREF;

    public void SetItemREF(Item item)
    {
        itemREF = item;
        if (itemREF == null || (itemREF is not UpgradableItems && itemREF is not GadgetItem))
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        switch(itemREF.GetItemSO().GetCategory())
        {
            case Category.ARTIFACTS:
                Artifacts artifacts = itemREF as Artifacts;
                if (artifacts != null)
                {
                    Artifacts ExistArtifacts = currentCharacterREF.CheckIfArtifactTypeExist(artifacts.GetArtifactType());

                    if (artifacts.GetCharacterEquipped() == null)
                    {
                        artifacts.SetEquippedCharacter(currentCharacterREF);
                        currentCharacterREF.GetEquippedArtifactsList().Add(artifacts);
                    }
                    else
                    {
                        if (ExistArtifacts != null)
                        {
                            if (artifacts.GetCharacterEquipped() != ExistArtifacts.GetCharacterEquipped())
                            {

                                artifacts.GetCharacterEquipped().GetEquippedArtifactsList().Remove(artifacts);
                                ExistArtifacts.GetCharacterEquipped().GetEquippedArtifactsList().Remove(ExistArtifacts);

                                CharacterData temp = artifacts.GetCharacterEquipped();
                                artifacts.SetEquippedCharacter(ExistArtifacts.GetCharacterEquipped());
                                ExistArtifacts.SetEquippedCharacter(temp);

                                artifacts.GetCharacterEquipped().GetEquippedArtifactsList().Add(artifacts);
                                ExistArtifacts.GetCharacterEquipped().GetEquippedArtifactsList().Add(ExistArtifacts);
                                return;
                            }
                            else
                            {
                                currentCharacterREF.GetEquippedArtifactsList().Remove(ExistArtifacts);
                                ExistArtifacts.SetEquippedCharacter(null);
                            }
                        }
                        else
                        {
                            artifacts.GetCharacterEquipped().GetEquippedArtifactsList().Remove(artifacts);
                            artifacts.SetEquippedCharacter(currentCharacterREF);
                            currentCharacterREF.GetEquippedArtifactsList().Add(artifacts);

                        }
                    }

                    if (ExistArtifacts != null)
                    {
                        currentCharacterREF.GetEquippedArtifactsList().Remove(ExistArtifacts);
                        ExistArtifacts.SetEquippedCharacter(null);
                    }



                    if (currentCharacterREF != null)
                    {
                        Debug.Log(currentCharacterREF.GetActualMaxHealth(currentCharacterREF.GetLevel()));
                        currentCharacterREF.SetHealth(currentCharacterREF.GetPreviousHealthRatio() * currentCharacterREF.GetActualMaxHealth(currentCharacterREF.GetLevel()));
                    }
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CharactersShowcaseManager.GetSelectedCharacterData())
            currentCharacterREF = CharactersShowcaseManager.GetSelectedCharacterData().GetCharacterData();

        if (itemREF != null)
        {
            switch (itemREF.GetItemSO().GetCategory())
            {
                case Category.ARTIFACTS:
                    Artifacts artifacts = itemREF as Artifacts;
                    if (artifacts != null)
                    {
                        if (artifacts.GetCharacterEquipped() == null)
                        {
                            EquipTxt.text = "Equip";
                        }
                        else
                        {
                            EquipTxt.text = "Remove";
                            Artifacts ExistArtifacts = currentCharacterREF.CheckIfArtifactTypeExist(artifacts.GetArtifactType());
                            if (ExistArtifacts != null)
                            {
                                if (artifacts.GetCharacterEquipped() != ExistArtifacts.GetCharacterEquipped())
                                {
                                    EquipTxt.text = "Switch";
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }
}
