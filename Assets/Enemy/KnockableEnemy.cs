using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnockback
{
    bool isApplyingKnockBack();
    void ApplyKnockBack(Vector3 force);
}

public class KnockableEnemy : BaseEnemy, IKnockback
{
    private Coroutine KnockBackCoroutine;

    public void ApplyKnockBack(Vector3 force)
    {
        if (KnockBackCoroutine != null)
        {
            StopCoroutine(KnockBackCoroutine);
        }
        KnockBackCoroutine = StartCoroutine(KnockBack(force));
    }

    private IEnumerator KnockBack(Vector3 force)
    {
        yield return null;
        GetNavMeshAgent().enabled = false;
        rb.AddForce(force);
        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => rb.velocity.magnitude < 0.05f);
        yield return new WaitForSeconds(0.25f);
        GetNavMeshAgent().enabled = true;
        GetNavMeshAgent().velocity = Vector3.zero;
        GetNavMeshAgent().Warp(transform.position);
        KnockBackCoroutine = null;
    }

    public bool isApplyingKnockBack()
    {
        return KnockBackCoroutine != null;
    }
}
