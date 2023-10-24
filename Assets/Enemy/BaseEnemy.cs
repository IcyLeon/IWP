using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : Characters
{
    protected float Ratio;

    protected virtual void Awake()
    {
        Level = 1;
        healthBarScript = Instantiate(AssetManager.GetInstance().EnemyHealthUIPrefab).GetComponent<HealthBarScript>();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        CurrentHealth = GetMaxHealth();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        healthBarScript.transform.position = transform.position + Vector3.up * 2f;
    }

    public override float GetMaxHealth()
    {
        return BaseMaxHealth * (1 + Ratio);
    }
}
