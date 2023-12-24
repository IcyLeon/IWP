using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarkerData
{
    private Dictionary<MarkerEnemyData, IDamage> MarkerEnemyDataList;

    public Dictionary<MarkerEnemyData, IDamage> GetMarkerEnemyDataList()
    {
        return MarkerEnemyDataList;
    }
    public MarkerData()
    {
        MarkerEnemyDataList = new();
    }

    public MarkerEnemyData GetMarkerEnemyData(IDamage dmg)
    {
        for (int i = GetMarkerEnemyDataList().Count - 1; i >= 0; i--)
        {
            KeyValuePair<MarkerEnemyData, IDamage> itemPair = GetMarkerEnemyDataList().ElementAt(i);
            if (GetMarkerEnemyDataList().TryGetValue(itemPair.Key, out IDamage IDamage))
            {
                if (dmg == IDamage)
                    return itemPair.Key;
            }
        }
        return null;
    }
    public void UpdateMarkerData()
    {
        for (int i = GetMarkerEnemyDataList().Count - 1; i >= 0; i--)
        {
            KeyValuePair<MarkerEnemyData, IDamage> itemPair = GetMarkerEnemyDataList().ElementAt(i);
            if (MarkerEnemyDataList.TryGetValue(itemPair.Key, out IDamage IDamage))
            {
                if (itemPair.Key.isTimeOut())
                {
                    itemPair.Key.GetMarker().DestroySelf();
                    GetMarkerEnemyDataList().Remove(itemPair.Key);
                }
                if (IDamage == null)
                {
                    itemPair.Key.GetMarker().DestroySelf();
                    GetMarkerEnemyDataList().Remove(itemPair.Key);
                }
                else
                {
                    if (IDamage.IsDead())
                    {
                        itemPair.Key.GetMarker().DestroySelf();
                        GetMarkerEnemyDataList().Remove(itemPair.Key);
                    }
                }
            }
        }
    }

    public void RemoveAllMarkers()
    {
        for (int i = GetMarkerEnemyDataList().Count - 1; i >= 0; i--)
        {
            KeyValuePair<MarkerEnemyData, IDamage> itemPair = GetMarkerEnemyDataList().ElementAt(i);
            if (GetMarkerEnemyDataList().TryGetValue(itemPair.Key, out IDamage IDamage))
            {
                itemPair.Key.GetMarker().DestroySelf();
                GetMarkerEnemyDataList().Remove(itemPair.Key);
            }
        }
        GetMarkerEnemyDataList().Clear();
    }

    public void AddMarkerToEnemy(MarkerEnemyData m)
    {
        if (GetMarkerEnemyDataList().TryGetValue(m, out IDamage IDamage))
        {
            m.SetTimer(m.GetMaxTimer());
        }
        else
        {
            GetMarkerEnemyDataList().Add(m, m.GetIDamageObj());
        }
    }
}
