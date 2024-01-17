using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MageEnemyPunch : MonoBehaviour
{
    [SerializeField] GameObject MagePunchPSPrefab;
    [SerializeField] Transform HeadPivot;
    [SerializeField] MageEnemy MageEnemy;
    private Dictionary<Collider, bool> PunchColliderList = new();

    private void OnTriggerStay(Collider Collider)
    {
        if (!PunchColliderList.TryGetValue(Collider, out bool value))
        {
            IDamage IDmg = Collider.GetComponent<IDamage>();
            if (IDmg != null)
                if (IDmg is PlayerCharacters || IDmg is FriendlyKillers)
                    PunchColliderList.Add(Collider, true);
        }
    }

    public void WackPlayer()
    {
        ParticleSystem MagePunchPS = Instantiate(MagePunchPSPrefab, HeadPivot.position, HeadPivot.transform.rotation).GetComponent<ParticleSystem>();
        Destroy(MagePunchPS.gameObject, MagePunchPS.main.duration);

        if (MageEnemy == null)
        {
            PunchColliderList.Clear();
            return;
        }

        for (int i = 0; i < PunchColliderList.Count; i++)
        {
            KeyValuePair<Collider, bool> PunchColliderkeyValuePair = PunchColliderList.ElementAt(i);
            if (PunchColliderList.TryGetValue(PunchColliderkeyValuePair.Key, out bool value))
            {
                IDamage IDmg = PunchColliderkeyValuePair.Key.GetComponent<IDamage>();
                if (IDmg != null)
                {
                    Elements hitElement = IDmg.TakeDamage(IDmg.GetPointOfContact(), new Elements(Elemental.NONE), MageEnemy.GetATK(), MageEnemy);
                    if (hitElement != null)
                    {
                        if (IDmg is PlayerCharacters player)
                        {
                            Vector3 ForceDir = HeadPivot.transform.forward * 15f;
                            player.GetPlayerManager().GetCharacterRB().AddForce(ForceDir, ForceMode.Impulse);
                        }

                        ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().BasicAttackHitEffect, IDmg.GetPointOfContact(), Quaternion.identity).GetComponent<ParticleSystem>();
                        Destroy(hitEffect.gameObject, hitEffect.main.duration);
                    }
                }

            }
        }
        PunchColliderList.Clear();
    }

    private void OnTriggerExit(Collider Collider)
    {
        if (PunchColliderList.TryGetValue(Collider, out bool value))
        {
            PunchColliderList.Remove(Collider);
        }
    }

}
