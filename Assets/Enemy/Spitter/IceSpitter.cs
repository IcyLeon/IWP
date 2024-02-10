using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IceSpitter : Spitter
{
    [SerializeField] int NoOfIceShards;

    protected override void ShootFireBall()
    {
        Vector3 dir = (GetPlayerLocation() - FireEmitter.position).normalized;

        for (int i = 0; i < NoOfIceShards; i++)
        {
            Fireball fireBall = Instantiate(FireBallPrefab, FireEmitter.position, Quaternion.identity).GetComponent<Fireball>();
            fireBall.SetElement(elemental);
            Rigidbody r = fireBall.GetComponent<Rigidbody>();
            r.velocity = GetDirection(dir, i) * 15f;
        }
        CurrentAttackRate = Random.Range(DefaultAttackRate, DefaultAttackRate + 0.8f);
    }

    private Vector3 GetDirection(Vector3 direction, int i)
    {
        float angle = 10f;

        Vector3 dir = Quaternion.Euler(0, ((-angle / 2) * (NoOfIceShards - 1)) + (i * angle), 0) * direction;
        dir.y = direction.y;
        dir.Normalize();

        return dir;
    }
}
