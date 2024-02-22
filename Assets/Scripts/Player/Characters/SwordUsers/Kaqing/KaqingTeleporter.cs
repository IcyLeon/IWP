using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class KaqingTeleporter : MonoBehaviour
{
    [SerializeField] GameObject ExplosionEffects;
    private Elements elements;
    private PlayerCharacters Kaqing;
    private bool EnergyOrbMoving;
    private Vector3 TargetLoc;

    public event Action OnDestroyOrb;
    public void SetTargetLoc(Vector3 pos)
    {
        TargetLoc = pos;
    }
    // Start is called before the first frame update
    void Start()
    {
        EnergyOrbMoving = true;
        StartCoroutine(MoveToTargetLocation(50f));
    }

    private void OnDestroy()
    {
        OnDestroyOrb?.Invoke();
    }

    public void SetElements(Elements elements)
    {
        this.elements = elements;
    }

    private IEnumerator MoveToTargetLocation(float speed)
    {
        while (transform.position != TargetLoc)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetLoc, speed * Time.deltaTime);
            yield return null;
        }
        Explode();
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject, Kaqing.GetCharacterData().GetPlayerCharacterSO().ElementalSkillsTimer);
        EnergyOrbMoving = false;
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.localScale.magnitude * 2f, LayerMask.GetMask("Entity", "BossEntity"));
        for(int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            IDamage damage = collider.GetComponent<IDamage>();
            if (damage != null)
            {
                if (!damage.IsDead())
                {
                    Vector3 hitPosition = collider.ClosestPointOnBounds(transform.position);
                    damage.TakeDamage(hitPosition, elements, Kaqing.GetATK(), Kaqing);
                }
            }
        }

        ParticleSystem ps = Instantiate(ExplosionEffects, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(ps.gameObject, ps.main.duration);
    }

    public void SetCharacterData(PlayerCharacters characterData)
    {
        Kaqing = characterData;
    }
    public bool GetEnergyOrbMoving()
    {
        return EnergyOrbMoving;
    }
}
