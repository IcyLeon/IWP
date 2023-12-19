using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowIdleState : BowControlState
{
    private float threasHold_Charged;

    public BowIdleState(PlayerCharacterState pcs) : base(pcs)
    {
        threasHold_Charged = 0;
    }

    public override void Enter()
    {
        base.Enter();
        threasHold_Charged = 0;
    }

    public override void Update()
    {
        base.Update();
        UpdateBowAimThresHold();
    }

    public override void ChargeHold()
    {
        if (threasHold_Charged > 0.25f)
        {
            GetBowCharactersState().ChangeState(GetBowCharactersState().bowAimState);
            return;
        }
        threasHold_Charged += Time.deltaTime;
    }

    public override void ChargeTrigger()
    {
        GetBowCharactersState().GetBowCharacters().LaunchBasicAttack();
        threasHold_Charged = 0;
    }

    private void UpdateBowAimThresHold()
    {
        if (Input.GetMouseButton(1))
        {
            GetBowCharactersState().ChangeState(GetBowCharactersState().bowAimState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        threasHold_Charged = 0;
    }
}
