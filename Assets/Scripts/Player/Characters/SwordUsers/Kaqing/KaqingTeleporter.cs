using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingTeleporter : MonoBehaviour
{
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

    void Explode()
    {

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
        TravelDamage();
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject, Kaqing.GetCharacterData().GetPlayerCharacterSO().ElementalSkillsTimer);
        EnergyOrbMoving = false;
    }

    private void TravelDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.localScale.magnitude * 2f, LayerMask.GetMask("Entity", "BossEntity"));
        for(int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            IDamage damage = collider.GetComponent<IDamage>();
            if (damage != null)
            {
                if (!damage.IsDead())
                    damage.TakeDamage(damage.GetPointOfContact(), elements, Kaqing.GetATK(), Kaqing);
            }
        }
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
