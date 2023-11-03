using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaqingTeleporter : MonoBehaviour
{
    private Elements elements;
    private CharacterData Kaqing;
    private bool EnergyOrbMoving;
    // Start is called before the first frame update
    void Start()
    {
        EnergyOrbMoving = true;
        Destroy(gameObject, 5f);
    }

    void Explode()
    {

    }

    public void SetElements(Elements elements)
    {
        this.elements = elements;
    }

    public IEnumerator MoveToTargetLocation(Vector3 target, float speed)
    {
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        TravelDamage();
        yield return new WaitForSeconds(0.1f);
        EnergyOrbMoving = false;
    }

    private void TravelDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.localScale.magnitude * 2f, LayerMask.GetMask("Entity"));
        for(int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            IDamage damage = collider.GetComponent<IDamage>();
            if (damage != null)
            {
                damage.TakeDamage(collider.transform.position, elements, Kaqing.GetDamage());
            }
        }
    }

    public void SetCharacterData(CharacterData characterData)
    {
        Kaqing = characterData;
    }
    public bool GetEnergyOrbMoving()
    {
        return EnergyOrbMoving;
    }
}
