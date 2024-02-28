using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingDrone : FriendlyKillers
{
    [SerializeField] Collider col;
    [SerializeField] GameObject HealPrefab;
    private GameObject HealPS;
    private float FireInBetweenShots;
    private float StartFireRate;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        FireInBetweenShots = 1f / GetFireRate();
        StartFireRate = Time.time;
    }

    private float GetFireRate()
    {
        return GetTurretSO().FireRate;
    }

    public TurretSO GetTurretSO()
    {
        TurretSO turretSO = GetFriendlyKillerSO() as TurretSO;
        return turretSO;
    }

    public override Vector3 GetPointOfContact()
    {
        return col.bounds.center;
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateHealingDrone();
    }

    private void UpdateHealingDrone()
    {
        if (CanInteract() || IsDead())
            return;

        if (Time.time - StartFireRate > FireInBetweenShots)
        {
            Heal();
            StartFireRate = Time.time;
        }
    }

    private void Heal()
    {
        Collider[] colliders = GetAllNearestIDamage(transform.position, GetDetectionRange(), LayerMask.GetMask("Player"), ~LayerMask.GetMask("Ignore Raycast, Ignore Collision"));
        for (int i = 0; i < colliders.Length; i++)
        {
            PlayerCharacters pc = colliders[i].GetComponent<PlayerCharacters>();
            if (pc != null)
            {
                pc.GetPlayerManager().HealCharacter(pc.GetCharacterData(), GetTurretSO().BaseDamage);
            }
        }
    }

    protected override void UpdateDead()
    {
        if (IsDead())
        {
            if (HealPS != null)
                Destroy(HealPS.gameObject);
        }
        base.UpdateDead();
    }
    protected override void PurchaseAction(PlayerManager PM)
    {
        if (CanInteract())
        {
            HealPS = Instantiate(HealPrefab, transform);
        }
        base.PurchaseAction(PM);
    }
}
