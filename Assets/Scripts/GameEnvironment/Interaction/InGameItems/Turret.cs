using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : FriendlyKillers
{
    private float Damage;
    private float TurretRange;
    private IDamage Target;
    [SerializeField] GameObject TurretMuzzle;

    public TurretSO GetTurretSO()
    {
        TurretSO turretSO = GetFriendlyKillerSO() as TurretSO;
        return turretSO;
    }

    public override string InteractMessage()
    {
        return "Technician Turret";
    }

    public override bool IsDead()
    {
        return base.IsDead();
    }


    public override Elements TakeDamage(Vector3 position, Elements elements, float damageAmt)
    {
        Elements e = base.TakeDamage(position, elements, damageAmt);
        return e;
    }

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
}
