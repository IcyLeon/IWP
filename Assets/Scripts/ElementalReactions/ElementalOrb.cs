using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalOrb : MonoBehaviour
{
    private Elemental elemental;
    private CharacterManager characterManager;
    private float coneAngle = 30f;
    bool canTravel = false;
    [SerializeField] Rigidbody rb;
    private float maxForce = 25f;
    private float CurrentForce = 0f;
    private float accelerationTime = 3f;

    void Start()
    {
        rb.AddForce(25f * RandomVectorInCone(), ForceMode.VelocityChange);
        characterManager = CharacterManager.GetInstance();
    }


    // Function to generate a random vector within a cone shape
    Vector3 RandomVectorInCone()
    {
        // Get a random rotation within the cone angle
        Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0, coneAngle), Random.onUnitSphere);

        // Generate a random direction within the cone
        Vector3 randomDirection = randomRotation * Vector3.up;

        return randomDirection.normalized;
    }

    private void Update()
    {
        if (!canTravel)
        {
            if (!IsMovingUp())
                canTravel = true;
        }
    }
    void MoveTowardsPlayer()
    {
        Vector3 collider = CharacterManager.GetInstance().GetCurrentCharacter().GetComponent<CapsuleCollider>().bounds.center;
        Vector3 direction = (collider - transform.position).normalized;
        CurrentForce = Mathf.Lerp(CurrentForce, maxForce, Time.deltaTime / accelerationTime);
        rb.MovePosition(rb.position + direction * CurrentForce * Time.deltaTime);
    }


    private void FixedUpdate()
    {
        if (canTravel)
            MoveTowardsPlayer();

        if (IsMovingUp())
        {
            DecelerateVertically();
        }
    }

    private void DecelerateVertically()
    {
        Vector3 playerVerticalVelocity = GetVerticalVelocity();
        rb.AddForce(-playerVerticalVelocity * 8f, ForceMode.Acceleration);
    }

    private bool IsMovingUp(float minimumVelocity = 0.1f)
    {
        return GetVerticalVelocity().y > minimumVelocity;
    }

    private Vector3 GetVerticalVelocity()
    {
        if (rb == null)
            return Vector3.zero;

        return new Vector3(0f, rb.velocity.y, 0f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        PlayerCharacters playerCharacters = collision.transform.GetComponent<PlayerCharacters>();
        if (playerCharacters != null)
        {
            for (int i = 0; i < characterManager.GetCharactersOwnedList().Count; i++)
            {
                CharacterData characterData = characterManager.GetCharactersOwnedList()[i];

                if (characterData.GetPlayerCharacterSO().Elemental == elemental)
                    characterData.AddorRemoveCurrentEnergyBurstCost(2.5f);
                else
                    characterData.AddorRemoveCurrentEnergyBurstCost(1f);
            }
            Destroy(gameObject);
        }
    }
}