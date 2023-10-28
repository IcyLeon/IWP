using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordCharacters : PlayerCharacters
{
    [SerializeField] protected Sword SwordREF;
    protected int BasicAttackPhase;
    protected int AttackLayer;
    private float TimeOutTimer;
    private float AttackAnimeElapsed;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ResetBasicAttacks();

        if (GetCharacterData() != null)
            GetSword().SetSwordCharacterWield(this);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public Sword GetSword()
    {
        return SwordREF;
    }

    public void ResetBasicAttacks()
    {
        BasicAttackPhase = 0;
        AttackAnimeElapsed = 0;
        Animator.SetInteger("AttackPhase", BasicAttackPhase);
    }

    protected override void ChargeTrigger()
    {
        if (GetPlayerController().GetGroundStatus() != GroundStatus.GROUND)
            return;


        BasicAttackPhase++;
        if (BasicAttackPhase > 2)
        {
            ResetBasicAttacks();
        }
        AttackAnimeElapsed = 0;

        Animator.SetInteger("AttackPhase", BasicAttackPhase);
        Animator.SetTrigger("Attack");
    }
}
