using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveFood : RecoveryFood
{
    public override void Use(int Useamount)
    {
        if (!GetCharacterData().IsDead())
        {
            AssetManager.GetInstance().OpenMessageNotification("Object can only be used on a fallen character");
            return;
        }
        base.Use(Useamount);
    }
    public ReviveFood(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
    }
}
