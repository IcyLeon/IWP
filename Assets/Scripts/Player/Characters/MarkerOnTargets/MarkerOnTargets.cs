using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MarkerOnTargets : MonoBehaviour
{
    private MarkerEnemyData markerEnemyData;

    public void SetMarkerData(MarkerEnemyData m)
    {
        markerEnemyData = m;
    }

    public void DestroySelf()
    {
        if (this == null)
            return;

        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (markerEnemyData == null || markerEnemyData.isTimeOut())
        {
            DestroySelf();
            return;
        }

        markerEnemyData.Update();
    }
}
