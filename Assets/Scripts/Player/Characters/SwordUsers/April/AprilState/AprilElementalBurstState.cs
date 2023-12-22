using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AprilElementalBurstState : SwordElementalBurstState
{

    public AprilState GetAprilState()
    {
        return (AprilState)GetSwordCharactersState();
    }

    public AprilElementalBurstState(PlayerCharacterState pcs) : base(pcs)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GetSwordCharactersState().GetSwordCharacters().GetSwordModel().gameObject.SetActive(false);
    }

    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();

    }

    private void SetMarkersOnEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(GetPlayerCharacterState().GetPlayerCharacters().GetPlayerManager().GetPlayerOffsetPosition().position, GetPlayerCharacterState().GetPlayerCharacters().GetUltiRange(), LayerMask.GetMask("Entity"));
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamage dmg = colliders[i].GetComponent<IDamage>();
            if (dmg != null)
            {
                MarkerEnemyData m = new MarkerEnemyData(GetPlayerCharacterState().GetPlayerCharacters().GetCharacterData(), dmg);
                GetAprilState().aprilData.MarkerEnemyDataList.Add(m);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        GetSwordCharactersState().GetSwordCharacters().GetSwordModel().gameObject.SetActive(true);

    }
}
