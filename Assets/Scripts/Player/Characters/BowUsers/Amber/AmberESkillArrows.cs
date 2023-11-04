using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmberESkillArrows : MonoBehaviour
{
    private float speed = 50f;
    private CharacterData Amber;
    [SerializeField] Rigidbody rb;
    private Vector3 FocalPointPos;

    public Rigidbody GetRB()
    {
        return rb;
    }
    public void SetFocalPointContact(Vector3 pos)
    {
        FocalPointPos = pos;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Moving());
    }

    public void SetCharacterData(CharacterData characterData)
    {
        Amber = characterData;
    }

    private IEnumerator Moving()
    {
        float maxTurnRate = 360f;
        float maxVelocity = 10f; 

        while (true)
        {
            Vector3 targetDirection = (FocalPointPos - rb.position).normalized;

            float angle = Vector3.Angle(rb.velocity.normalized, targetDirection);

            Vector3 forceDirection = targetDirection;

            if (angle > 0.0f)
            {
                float turnRate = Mathf.Min((angle / 180.0f) * maxTurnRate, maxTurnRate);
                forceDirection = Vector3.RotateTowards(rb.velocity.normalized, targetDirection, turnRate * Time.deltaTime, 0.0f);
            }

            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
            rb.AddForce(forceDirection.normalized * speed);

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(rb.position, 4f, LayerMask.GetMask("Entity"));
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamage damageObject = colliders[i].gameObject.GetComponent<IDamage>();
            if (damageObject != null)
            {
                damageObject.TakeDamage(transform.position, new Elements(Amber.GetPlayerCharacterSO().Elemental), 10f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCharacters player = other.transform.GetComponent<PlayerCharacters>();

        if (player == null && other.GetComponent<AmberESkillArrows>() == null)
        {
            if (other.tag != "Player")
            {
                Explode();
                Destroy(gameObject);
            }
        }
    }
}
