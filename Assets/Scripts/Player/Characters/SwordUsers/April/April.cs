using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class April : SwordCharacters
{
    [SerializeField] GameObject ShieldPrefab;
    private GameObject Shield;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void UpdateCoordinateAttack()
    {
    }

    public override bool CoordinateAttackEnded()
    {
        return true;
    }

    public override bool CoordinateCanShoot()
    {
        return false;
    }
}
