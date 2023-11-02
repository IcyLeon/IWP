using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private SwordCharacters SwordCharacters;

    public SwordCharacters GetSwordCharacters()
    {
        return SwordCharacters;
    }

    public void SetSwordCharacterWield(SwordCharacters swordCharacters)
    {
        SwordCharacters = swordCharacters;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (GetSwordCharacters() == null)
            return;

        BaseEnemy enemy = other.GetComponent<BaseEnemy>();

        if (enemy != null) {
            Vector3 hitPosition = enemy.transform.position;

            if (other is MeshCollider meshCollider)
            {
                hitPosition = meshCollider.ClosestPointOnBounds(enemy.transform.position);
            }
            enemy.TakeDamage(hitPosition, new Elements (GetSwordCharacters().GetCurrentSwordElemental()), 1000f);
        }
    }
}
