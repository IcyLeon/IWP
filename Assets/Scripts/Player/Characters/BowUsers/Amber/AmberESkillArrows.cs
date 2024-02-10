using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberESkillArrows : MonoBehaviour
{
    private PlayerCharacters Amber;
    private CharacterData AmberCharacterData;
    private IDamage target;
    [SerializeField] Rigidbody rb;
    [SerializeField] AudioClip explosionClipSound;
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
        StartCoroutine(Timeout());
    }

    private IEnumerator Timeout()
    {
        yield return new WaitForSeconds(5f);
        Explode();
        Destroy(gameObject);
    }

    public void SetCharacterData(PlayerCharacters PlayerCharacters)
    {
        Amber = PlayerCharacters;
        AmberCharacterData = PlayerCharacters.GetCharacterData();
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
        Explosion explosion = Instantiate(AssetManager.GetInstance().HitExplosion, transform.position, Quaternion.identity).GetComponent<Explosion>();
        explosion.SetExplosionSound(explosionClipSound);
        explosion.Init(AmberCharacterData.GetPlayerCharacterSO().Elemental, LayerMask.GetMask("Entity", "BossEntity"), 3.5f, AmberCharacterData.GetActualATK(AmberCharacterData.GetLevel()), Amber);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        PlayerCharacters player = other.GetComponent<PlayerCharacters>();
        IDamage damageObject = other.GetComponent<IDamage>();

        if (player == null && (!other.isTrigger || damageObject != null))
        {
            if (damageObject != null)
            {
                if (damageObject.IsDead())
                {
                    Physics.IgnoreCollision(GetComponent<Collider>(), other);
                    return;
                }
            }

            Explode();
            Destroy(gameObject);
        }
    }
}
