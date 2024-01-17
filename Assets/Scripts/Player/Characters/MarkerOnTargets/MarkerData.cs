using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MarkerData
{
    private Dictionary<IDamage, MarkerEnemyData> MarkerEnemyDataList;

    public Dictionary<IDamage, MarkerEnemyData> GetMarkerEnemyDataList()
    {
        return MarkerEnemyDataList;
    }
    public MarkerData()
    {
        MarkerEnemyDataList = new();
    }

    public MarkerEnemyData GetMarkerEnemyData(IDamage d)
    {

        if (MarkerEnemyDataList.ContainsKey(d))
        {
            return MarkerEnemyDataList[d];
        }

        return null;
    }
    public void UpdateMarkerData()
    {
        for (int i = GetMarkerEnemyDataList().Count - 1; i >= 0; i--)
        {
            KeyValuePair<IDamage, MarkerEnemyData> itemPair = GetMarkerEnemyDataList().ElementAt(i);
            if (MarkerEnemyDataList.TryGetValue(itemPair.Key, out MarkerEnemyData MarkerEnemyData))
            {
                if (MarkerEnemyData.isTimeOut())
                {
                    MarkerEnemyData.GetMarker().DestroySelf();
                    GetMarkerEnemyDataList().Remove(itemPair.Key);
                }
                if (MarkerEnemyData == null)
                {
                    MarkerEnemyData.GetMarker().DestroySelf();
                    GetMarkerEnemyDataList().Remove(itemPair.Key);
                }
                else
                {
                    if (itemPair.Key.IsDead())
                    {
                        MarkerEnemyData.GetMarker().DestroySelf();
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
            KeyValuePair<IDamage, MarkerEnemyData> itemPair = GetMarkerEnemyDataList().ElementAt(i);
            if (GetMarkerEnemyDataList().TryGetValue(itemPair.Key, out MarkerEnemyData MarkerEnemyData))
            {
                MarkerEnemyData.GetMarker().DestroySelf();
                GetMarkerEnemyDataList().Remove(itemPair.Key);
            }
        }
        GetMarkerEnemyDataList().Clear();
    }

    public MarkerEnemyData AddMarkerToEnemy(IDamage dmg, float timer)
    {

        if (GetMarkerEnemyDataList().TryGetValue(dmg, out MarkerEnemyData MarkerEnemyData))
        {
            MarkerEnemyData.SetTimer(MarkerEnemyData.GetMaxTimer());
        }
        else
        {
            MarkerEnemyData m = new MarkerEnemyData(dmg, timer);
            GetMarkerEnemyDataList().Add(m.GetIDamageObj(), m);
            return m;
        }
        return null;

    }
}
