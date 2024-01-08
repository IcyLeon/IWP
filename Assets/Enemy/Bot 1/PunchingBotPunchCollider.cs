using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PunchingBotPunchCollider : MonoBehaviour
{
    [SerializeField] PunchingBot PunchingBot;
    private Dictionary<Collider, bool> PunchColliderList = new();

    private void OnTriggerStay(Collider Collider)
    {
        if (!PunchColliderList.TryGetValue(Collider, out bool value))
        {
            PlayerCharacters pc = Collider.GetComponent<PlayerCharacters>();
            if (pc != null)
                PunchColliderList.Add(Collider, true);
        }
    }

    public void WackPlayer()
    {
        if (PunchingBot == null)
        {
            PunchColliderList.Clear();
            return;
        }

        for (int i = 0; i < PunchColliderList.Count; i++)
        {
            KeyValuePair<Collider, bool> PunchColliderkeyValuePair = PunchColliderList.ElementAt(i);
            if (PunchColliderList.TryGetValue(PunchColliderkeyValuePair.Key, out bool value)) {
                PlayerCharacters pc = PunchColliderkeyValuePair.Key.GetComponent<PlayerCharacters>();
                pc.TakeDamage(pc.GetPointOfContact(), new Elements(Elemental.NONE), PunchingBot.GetATK(), PunchingBot);

                ParticleSystem hitEffect = Instantiate(AssetManager.GetInstance().BasicAttackHitEffect, pc.GetPointOfContact(), Quaternion.identity).GetComponent<ParticleSystem>();
                Destroy(hitEffect.gameObject, hitEffect.main.duration);

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
