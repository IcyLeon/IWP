using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AprilData
{
    private April april;
    public LineRenderer connectedLineRenderer;
    public MarkerData m_MarkerData;

    public void UpdateConnections()
    {
        if (m_MarkerData.GetMarkerEnemyDataList().Count > 1)
        {
            if (connectedLineRenderer == null)
            {
                connectedLineRenderer = april.GetConnectionLine();
            }
        }
        else
        {
            if (connectedLineRenderer)
                april.DestroyLine();
        }

        if (!connectedLineRenderer)
            return;

        connectedLineRenderer.positionCount = m_MarkerData.GetMarkerEnemyDataList().Count;
        for (int i = 0; i < m_MarkerData.GetMarkerEnemyDataList().Count; i++)
        {
            KeyValuePair<MarkerEnemyData, IDamage> itemPair = m_MarkerData.GetMarkerEnemyDataList().ElementAt(i);
            if (m_MarkerData.GetMarkerEnemyDataList().TryGetValue(itemPair.Key, out IDamage IDamage)) {
                connectedLineRenderer.SetPosition(i, IDamage.GetPointOfContact());
            }
        }
    }

    public void UpdateMarkerData()
    {
        m_MarkerData.UpdateMarkerData();
    }
    public void RemoveAllMarkers()
    {
        m_MarkerData.RemoveAllMarkers();

        if (connectedLineRenderer)
            april.DestroyLine();
    }

    public AprilData(April april)
    {
        this.april = april;
        m_MarkerData = new MarkerData();
    }
}
