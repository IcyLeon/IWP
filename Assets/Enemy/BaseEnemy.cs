using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : Characters
{
    protected float Ratio;
    [SerializeField] CharactersSO EnemysSO;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override float GetMaxHealth()
    {
        return base.GetMaxHealth() * (1 + Ratio);
    }
}
