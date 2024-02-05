using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    private Elements elements;
    private PlayerCharacters BowCharacters;
    private CharacterData BowCharactersData;
    private float FlightTime = 15f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TurnOnGravity());
        Destroy(gameObject, FlightTime);
    }

    IEnumerator TurnOnGravity()
    {
        rb.useGravity = false;
        yield return new WaitForSeconds(0.2f);
        rb.useGravity = true;
    }

    void FixedUpdate()
    {
        LimitFallVelocity();
    }

    private void LimitFallVelocity()
    {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 45f);
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
        FriendlyKillers friendlyKillers = other.GetComponent<FriendlyKillers>();
        IDamage damageObject = other.gameObject.GetComponent<IDamage>();

        if (player == null && (!other.isTrigger || damageObject != null) && friendlyKillers == null)
        {
            if (damageObject != null)
            {
                if (!damageObject.IsDead())
                {
                    float dmg = BowCharactersData.GetActualATK(BowCharacters.GetLevel());
                    if (elements.GetElements() != Elemental.NONE)
                    {
                        dmg *= 2.5f;
                    }
                    damageObject.TakeDamage(damageObject.GetPointOfContact(), elements, dmg, BowCharacters);
                }
                else
                {
                    Physics.IgnoreCollision(GetComponent<Collider>(), other);
                    return;
                }

            }

            ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().HitEffect, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            Destroy(hitEffect.gameObject, hitEffect.main.duration);
            Destroy(gameObject);
        }
    }

    public void SetCharacterData(PlayerCharacters PlayerCharacters)
    {
        BowCharacters = PlayerCharacters;
        BowCharactersData = PlayerCharacters.GetCharacterData();
    }
}
