using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalOrb : MonoBehaviour
{
    private Elemental elemental;
    private PlayerManager pm;
    private float coneAngle = 30f;
    bool canTravel = false;
    [SerializeField] Rigidbody rb;
    private float maxForce = 15f;
    private float currentForce = 0f;
    private float accelerationTime = 2f;

    void Start()
    {
        rb.velocity = maxForce * 0.5f * RandomVectorInCone();
    }

    public void SetSource(PlayerManager PM)
    {
        pm = PM;
    }
    Vector3 RandomVectorInCone()
    {
        Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0, coneAngle), Random.onUnitSphere);
        Vector3 randomDirection = randomRotation * Vector3.up;
        return randomDirection.normalized;
    }

    void MoveTowardsPlayer()
    {
        Vector3 collider = pm.GetCurrentCharacter().GetComponent<CapsuleCollider>().bounds.center;
        Vector3 direction = (collider - rb.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        currentForce = Mathf.Lerp(currentForce, maxForce, Time.deltaTime / accelerationTime);
        rb.MoveRotation(rotation);
        rb.MovePosition(rb.position + direction * currentForce * Time.deltaTime);
    }


    private void FixedUpdate()
    {
        MoveTowardsPlayer();
    }


    private void OnTriggerEnter(Collider collision)
    {
        PlayerCharacters playerCharacters = collision.transform.GetComponent<PlayerCharacters>();
        if (playerCharacters != null)
        {
            for (int i = 0; i < pm.GetCharactersOwnedList().Count; i++)
            {
                CharacterData characterData = pm.GetCharactersOwnedList()[i];

                if (characterData.GetPlayerCharacterSO().Elemental == elemental)
                    characterData.AddorRemoveCurrentEnergyBurstCost(2.5f);
                else
                    characterData.AddorRemoveCurrentEnergyBurstCost(1f);
            }
            Destroy(gameObject);
        }
    }
}