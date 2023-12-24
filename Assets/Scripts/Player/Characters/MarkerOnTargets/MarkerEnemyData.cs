using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerEnemyData
{
    private float Timer;
    private float MaxTimer;
    private MarkerOnTargets Marker;
    private CharacterData characterData;
    private IDamage damageTarget;

    public MarkerEnemyData(MarkerOnTargets m, CharacterData characterData, IDamage damageTarget, float MaxTimer)
    {
        Marker = m;
        this.characterData = characterData;
        this.damageTarget = damageTarget;
        this.MaxTimer = MaxTimer;
        Timer = this.MaxTimer;

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
