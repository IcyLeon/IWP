using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalOrb : MonoBehaviour
{
    private Elements elements;
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
        yield return new WaitForSeconds(1f);
        EnergyOrbMoving = false;
    }

    private void TravelDamage()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.localScale.magnitude);
        for(int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            IDamage damage = collider.GetComponent<IDamage>();
            if (damage != null)
            {
                damage.TakeDamage(collider.transform.position, elements, 1);
            }
        }
    }
    public bool GetEnergyOrbMoving()
    {
        return EnergyOrbMoving;
    }
}
