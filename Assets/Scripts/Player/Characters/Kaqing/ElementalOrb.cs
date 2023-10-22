using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementalOrb : MonoBehaviour
{
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

    public IEnumerator MoveToTargetLocation(Vector3 target, float speed)
    {
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        EnergyOrbMoving = false;
    }

    public bool GetEnergyOrbMoving()
    {
        return EnergyOrbMoving;
    }
}
