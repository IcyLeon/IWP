using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MageFireBall : MonoBehaviour
{
    private Elemental Elemental;
    private bool MoveToTarget = false;
    private MageEnemy MageEnemy;
    [SerializeField] Rigidbody rb;
    private float Speed = 30f;
    [SerializeField] GameObject FireAreaOfEffectPrefab;
    [SerializeField] GameObject ExplosionPSPrefab;

    public void Init(float timer, MageEnemy mageEnemy)
    {
        MageEnemy = mageEnemy;
        StartCoroutine(TimerCountDown(timer));
    }

    private IEnumerator TimerCountDown(float timer)
    {
        MoveToTarget = false;
        yield return new WaitForSeconds(timer);
        transform.SetParent(null);
        MoveToTarget = true;
        rb.velocity = (MageEnemy.GetPlayerLocation() - transform.position).normalized * Speed;
        StartCoroutine(TimeOut());
    }

    private IEnumerator TimeOut()
    {
        yield return new WaitForSeconds(8f);
        DestroyFireBall();
    }

    private void DestroyFireBall()
    {
        MageEnemy.NukePlayer(rb.position, 2.5f, MageEnemy.GetATK() * 0.75f);
        ParticleSystem PS = Instantiate(ExplosionPSPrefab, rb.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(PS.gameObject, PS.main.duration);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (MageEnemy != null)
        {
            if (MageEnemy.IsDead())
                DestroyFireBall();
        }
        else
        {
            DestroyFireBall();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || !MoveToTarget)
            return;

        if (other.GetComponent<IDamage>() != null)
            return;

        Vector3 position = other.ClosestPointOnBounds(transform.position);
        position.y += 0.01f;
        FireAreaOfEffect FireAreaOfEffect = Instantiate(FireAreaOfEffectPrefab, position, FireAreaOfEffectPrefab.transform.localRotation).GetComponent<FireAreaOfEffect>();
        FireAreaOfEffect.Init(MageEnemy);
        MageEnemy.AddToFireAreaOfEffectList(FireAreaOfEffect);
        DestroyFireBall();
    }

}
