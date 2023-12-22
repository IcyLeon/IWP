using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerEnemyData
{
    private GameObject Marker;
    private CharacterData characterData;
    private IDamage damageTarget;

    public MarkerEnemyData(CharacterData characterData, IDamage damageTarget)
    {
        this.characterData = characterData;
        this.damageTarget = damageTarget;
    }

    public void Update()
    {
        if (Marker)
        {
            Marker.transform.position = damageTarget.GetPointOfContact();
        }
    }
}
