using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerEnemyData
{
    private float Timer;
    private float MaxTimer;
    private MarkerOnTargets Marker;
    private IDamage damageTarget;

    public MarkerEnemyData(IDamage damageTarget, float MaxTimer)
    {
        this.damageTarget = damageTarget;
        this.MaxTimer = MaxTimer;
        Timer = this.MaxTimer;
    }

    public void InitTargets(MarkerOnTargets m)
    {
        Marker = m;
        if (Marker != null)
        {
            Marker.SetMarkerData(this);
        }
    }

    public IDamage GetIDamageObj()
    {
        return damageTarget;
    }
    public void SetTimer(float time)
    {
        Timer = time;
    }

    public float GetTimer()
    {
        return Timer;
    }

    public float GetMaxTimer()
    {
        return MaxTimer;
    }

    public bool isTimeOut()
    {
        return Timer <= 0;
    }

    public MarkerOnTargets GetMarker()
    {
        return Marker;
    }
    public void Update()
    {
        if (GetMarker())
        {
            GetMarker().transform.position = damageTarget.GetPointOfContact();
            Timer -= Time.deltaTime;
        }
    }
}
