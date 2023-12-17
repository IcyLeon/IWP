using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberESkillArrows : MonoBehaviour
{
    private float speed = 50f;
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
        float maxTurnRate = 360f;
        float maxVelocity = 10f;
        Vector3 forceDirection = rb.velocity.normalized;

        while (((FocalPointPos - rb.position).magnitude > 1.5f) || target != null)
        {
            rb.useGravity = target != null;

            Vector3 targetDirection = (FocalPointPos - rb.position).normalized;

            float angle = Vector3.Angle(rb.velocity.normalized, targetDirection);

            if (angle > 0.0f)
            {
                float turnRate = Mathf.Min((angle / 180.0f) * maxTurnRate, maxTurnRate);
                forceDirection = Vector3.RotateTowards(rb.velocity.normalized, targetDirection, maxTurnRate * Time.deltaTime, 0.0f);
            }

            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
            rb.AddForce(forceDirection.normalized * speed);

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
        Collider[] colliders = Physics.OverlapSphere(rb.position, 3.5f, LayerMask.GetMask("Entity"));
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
