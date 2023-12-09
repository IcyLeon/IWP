using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Turret : FriendlyKillers
{
    private float Damage;
    private Collider Target;
    private float FireInBetweenShots;
    private float StartFireRate;
    private const float SmoothTime = 20f;
    [SerializeField] Transform TurretRotationPivot;
    [SerializeField] GameObject TurretMuzzle;

    private Quaternion CurrentTargetRotation, Target_Rotation;

    public float GetDamage()
    {
        return GetTurretSO().BaseDamage;
    }
    public TurretSO GetTurretSO()
    {
        TurretSO turretSO = GetFriendlyKillerSO() as TurretSO;
        return turretSO;
    }

    public override bool IsDead()
    {
        return base.IsDead();
    }

    private void Fire()
    {
        IDamage targetIDamage = Target.GetComponent<IDamage>();
        targetIDamage.TakeDamage(targetIDamage.GetPointOfContact(), new Elements(Elemental.NONE), GetDamage());
    }

    public override Vector3 GetPointOfContact()
    {
        return TurretRotationPivot.position;
    }

    private Collider GetNearestIDamage(float range)
    {
        Collider[] colliders = GetAllNearestIDamage(range);

        if (colliders.Length == 0)
            return null;

        List<Collider> colliderCopy = new List<Collider>(colliders);

        Collider nearestCollider = colliderCopy[0];

        for (int i = colliderCopy.Count - 1; i >= 0; i--)
        {
            IDamage c = colliderCopy[i].GetComponent<IDamage>();
            if (c != null)
            {
                float dist1 = Vector3.Distance(colliderCopy[i].transform.position, transform.position);
                float dist2 = Vector3.Distance(nearestCollider.transform.position, transform.position);

                if (dist1 <= dist2)
                {
                    nearestCollider = colliderCopy[i];
                }
            }
        }

        return nearestCollider;
    }

    private bool TargetStillInRange(Collider target)
    {
        if (target == null)
            return false;

        Collider[] AllTargetsAvailable = GetAllNearestIDamage(GetDetectionRange());

        if (AllTargetsAvailable == null)
            return false;

        return AllTargetsAvailable.Contains(target);
    }

    private Collider[] GetAllNearestIDamage(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(TurretRotationPivot.position, range, LayerMask.GetMask("Entity"));
        List<Collider> colliderCopy = new List<Collider>(colliders);
        for (int i = colliderCopy.Count - 1; i >= 0; i--)
        {
            IDamage c = colliderCopy[i].GetComponent<IDamage>();
            if (c != null)
            {
                Vector3 dir = GetPointOfContact() - c.GetPointOfContact();
                if (Physics.Raycast(c.GetPointOfContact(), dir.normalized, out RaycastHit hit, range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.GetComponent<IDamage>() == null)
                    {
                        colliderCopy.RemoveAt(i);
                    }
                }

                if (c.IsDead() && i < colliderCopy.Count)
                {
                    colliderCopy.RemoveAt(i);
                }

            }
            else
            {
                colliderCopy.RemoveAt(i);
            }
        }

        return colliderCopy.ToArray();
    }


    public override Elements TakeDamage(Vector3 position, Elements elements, float damageAmt)
    {
        Elements e = base.TakeDamage(position, elements, damageAmt);
        return e;
    }

    private float GetFireRate()
    {
        return GetTurretSO().FireRate;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        FireInBetweenShots = 1f / GetFireRate();
        StartFireRate = Time.time;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateTurret();
    }

    private bool ActivatedCompleted()
    {
        if (animator == null)
            return true;

        return animator.GetCurrentAnimatorStateInfo(0).IsName("Activated") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
    }
    private void UpdateTurret()
    {
        if (CanInteract() || !ActivatedCompleted() || IsDead())
            return;

        if (!TargetStillInRange(Target))
        {
            Target = GetNearestIDamage(GetDetectionRange());
        }

        if (Target == null)
            return;

        UpdateRotateToTarget(Quaternion.LookRotation(Target.transform.position - TurretRotationPivot.position));

        if (Time.time - StartFireRate > FireInBetweenShots)
        {
            Fire();
            StartFireRate = Time.time;
        }
    }

    private void UpdateRotateToTarget(Quaternion quaternion)
    {
        SetTargetRotation(quaternion);
        UpdateTargetRotation();
    }
    private void SetTargetRotation(Quaternion quaternion)
    {
        Target_Rotation = quaternion;
    }

    private void RotateTowardsTargetRotation()
    {
        CurrentTargetRotation = Quaternion.Lerp(CurrentTargetRotation, Target_Rotation, Time.deltaTime * SmoothTime);
        TurretRotationPivot.rotation = CurrentTargetRotation;
    }

    private void UpdateTargetRotation()
    {
        if (CurrentTargetRotation != Target_Rotation)
        {
            CurrentTargetRotation = Target_Rotation;
        }
        RotateTowardsTargetRotation();
    }
}
