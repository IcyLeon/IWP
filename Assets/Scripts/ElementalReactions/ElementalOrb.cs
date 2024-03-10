using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class ElementalOrb : MonoBehaviour
{
    private Elemental elemental;
    private PlayerManager pm;
    private float coneAngle = 60f;
    bool canTravel = false;
    [SerializeField] Rigidbody rb;
    private float maxForce = 15f;
    private float currentForce = 0f;
    private float accelerationTime = 2.25f;

    void Start()
    {
        rb.velocity = maxForce * 0.6f * RandomVectorInCone();
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


    private void Update()
    {
        currentForce = Mathf.Lerp(currentForce, maxForce, Time.deltaTime / accelerationTime);
        rb.position = Vector3.MoveTowards(rb.position, pm.GetPlayerOffsetPosition().position, currentForce * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collider)
    {
        PlayerCharacters playerCharacters = collider.transform.GetComponent<PlayerCharacters>();
        if (playerCharacters != null)
        {
            for (int i = 0; i < pm.GetCharactersOwnedList().Count; i++)
            {
                CharacterData characterData = pm.GetCharactersOwnedList()[i];

                if (!characterData.IsDead())
                {
                    if (characterData.GetPlayerCharacterSO().Elemental == elemental)
                        characterData.AddorRemoveCurrentEnergyBurstCost(2.5f);
                    else
                        characterData.AddorRemoveCurrentEnergyBurstCost(1f);
                }
            }
            Destroy(gameObject);
        }
    }
}