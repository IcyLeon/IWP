using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    private Elements elements;
    private CharacterData BowCharacters;
    private float FlightTime = 10f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TurnOnGravity());
        Destroy(gameObject, FlightTime);
    }

    IEnumerator TurnOnGravity()
    {
        rb.useGravity = false;
        yield return new WaitForSeconds(0.5f);
        rb.useGravity = true;
    }

    void FixedUpdate()
    {
        LimitFallVelocity();
    }

    private void LimitFallVelocity()
    {
        float FallSpeedLimit = 15f;
        Vector3 velocity = new Vector3(0f, rb.velocity.y, 0f);
        if (velocity.y >= -FallSpeedLimit)
        {
            return;
        }

        Vector3 limitVel = new Vector3(0f, -FallSpeedLimit - velocity.y, 0f);
        rb.AddForce(limitVel, ForceMode.VelocityChange);

    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    public void SetElements(Elements elements)
    {
        this.elements = elements;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCharacters player = other.GetComponent<PlayerCharacters>();
    
        if (player == null && !other.isTrigger)
        {
            IDamage damageObject = other.gameObject.GetComponent<IDamage>();
            if (damageObject != null)
            {
                if (!damageObject.IsDead())
                    damageObject.TakeDamage(damageObject.GetPointOfContact(), elements, BowCharacters.GetDamage());
            }

            ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().HitEffect, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            Destroy(hitEffect.gameObject, hitEffect.main.duration);
            Destroy(gameObject);
        }
    }

    public void SetCharacterData(CharacterData characterData)
    {
        BowCharacters = characterData;
    }
}
