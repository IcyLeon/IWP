using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberESkillArrows : MonoBehaviour
{
    private CharacterData Amber;
    private IDamage target;
    [SerializeField] Rigidbody rb;
    private Vector3 FocalPointPos;

    public Rigidbody GetRB()
    {
        return rb;
    }
    public void SetFocalPointContact(Vector3 pos, IDamage target = null)
    {
        FocalPointPos = pos;
        this.target = target;
        if (this.target != null)
        {
            FocalPointPos = default(Vector3);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Moving());
        Destroy(gameObject, 5f);
    }

    public void SetCharacterData(CharacterData characterData)
    {
        Amber = characterData;
    }

    private IEnumerator Moving()
    {
        float maxTurnRate = 5f;
        float maxVelocity = 12f;
        Vector3 forceDirection = rb.velocity.normalized;

        while (target != null || FocalPointPos != default(Vector3))
        {
            if (((FocalPointPos - rb.position).magnitude <= 1f))
                yield break;

            rb.useGravity = target != null;

            Vector3 targetDirection;

            if (target != null)
            {
                targetDirection = (target.GetPointOfContact() - rb.position).normalized;
            }
            else
            {
                targetDirection = (FocalPointPos - rb.position).normalized;
            }

            forceDirection = Vector3.RotateTowards(forceDirection, targetDirection, maxTurnRate * Time.deltaTime, 0.0f);

            rb.velocity = forceDirection.normalized * maxVelocity;

            maxTurnRate += Time.deltaTime;
            yield return null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (target != null)
            FocalPointPos = target.GetPointOfContact();

        if (rb.velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(rb.position, 3.5f, LayerMask.GetMask("Entity", "BossEntity"));
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamage damageObject = colliders[i].gameObject.GetComponent<IDamage>();
            if (damageObject != null)
            {
                if (!damageObject.IsDead())
                    damageObject.TakeDamage(damageObject.GetPointOfContact(), new Elements(Amber.GetPlayerCharacterSO().Elemental), 10f);
            }
        }

        ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().HitExplosion, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(hitEffect.gameObject, hitEffect.main.duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCharacters player = other.transform.GetComponent<PlayerCharacters>();

        if (player == null)
        {
            if (other.isTrigger)
                return;

            Explode();
            Destroy(gameObject);
        }
    }
}
