using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AprilAnimationEvents : SwordUsersAnimationEvents
{
    [SerializeField] GameObject EnemyMarkerPrefab;
    [SerializeField] GameObject HealPrefab;
    private void SpawnHeal()
    {
        ParticleSystem heal = Instantiate(HealPrefab, GetSwordCharacters().GetPlayerManager().transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(heal.gameObject, heal.main.duration);
    }

    private MarkerOnTargets GetEnemyMarker()
    {
        MarkerOnTargets go = Instantiate(EnemyMarkerPrefab).GetComponent<MarkerOnTargets>();
        return go;
    }


    private void SetMarkersOnEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(GetSwordCharacters().GetPointOfContact(), GetSwordCharacters().GetPlayerCharacterState().GetPlayerCharacters().GetUltiRange(), LayerMask.GetMask("Entity", "BossEntity"));
        for (int i = 0; i < colliders.Length; i++)
        {
            IDamage dmg = colliders[i].GetComponent<IDamage>();
            if (dmg != null)
            {
                MarkerEnemyData NewMarkerAdded = GetApril().GetAprilState().aprilData.m_MarkerData.AddMarkerToEnemy(dmg, GetApril().GetPlayersSO().ElementalBurstTimer);
                if (NewMarkerAdded != null)
                {
                    NewMarkerAdded.InitTargets(GetEnemyMarker());
                }
            }
        }
    }

    private April GetApril()
    {
        return (April)GetSwordCharacters();
    }
}
