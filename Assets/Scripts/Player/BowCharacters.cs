using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BowCharacters : PlayerCharacters
{
    [SerializeField] GameObject ArrowPrefab;
    [SerializeField] Transform EmitterPivot;
    [SerializeField] ParticleSystem ChargeUpFinishPrefab;
    private ParticleSystem ChargeUpEmitter;
    private GameObject CrossHair;
    private float OriginalFireSpeed = 800f, BaseFireSpeed;
    private float LastClickedTime, AttackRate = 0.05f, ChargedAttackRate = 0.5f;
    private Vector3 Direction, ShootDirection;
    private Elemental ShootElemental;

    public Transform GetEmitterPivot()
    {
        return EmitterPivot;
    }

    private void Awake()
    {
        Range = 8f;
    }
    protected override void Update()
    {
        base.Update();

        if (Time.timeScale == 0)
            return;

        if (Animator)
        {
            Animator.SetFloat("AimVelocityX", GetPlayerManager().GetPlayerController().GetInputDirection().x, 0.1f, Time.deltaTime);
            Animator.SetFloat("AimVelocityZ", GetPlayerManager().GetPlayerController().GetInputDirection().z, 0.1f, Time.deltaTime);
        }
    }
    public BowCharactersState GetBowCharactersState()
    {
        return (BowCharactersState)PlayerCharacterState;
    }

    private void FireArrows()
    {
        Arrow ArrowFire = Instantiate(ArrowPrefab, GetEmitterPivot().transform.position, Quaternion.identity).GetComponent<Arrow>();
        Rigidbody ArrowRB = ArrowFire.GetComponent<Rigidbody>();
        ArrowFire.SetElements(new Elements(ShootElemental));
        ArrowFire.SetCharacterData(this);
        ArrowFire.transform.rotation = Quaternion.LookRotation(ShootDirection);

        if (!GetPlayerManager().IsAiming())
            BaseFireSpeed = OriginalFireSpeed * 0.8f;
        else
            BaseFireSpeed = OriginalFireSpeed;

        ArrowRB.AddForce(ShootDirection.normalized * BaseFireSpeed * (1 + GetBowCharactersState().BowData.ChargeElapsed));

        DestroyChargeUpEmitter();
        ShootElemental = Elemental.NONE;
        BasicAttackTrigger();
    }

    protected override Collider[] PlungeAttackGroundHit(Vector3 HitPos)
    {
        Collider[] colliders = base.PlungeAttackGroundHit(HitPos);
        foreach (Collider collider in colliders)
        {
            IDamage damageObject = collider.gameObject.GetComponent<IDamage>();
            if (damageObject != null)
            {
                if (!damageObject.IsDead())
                    damageObject.TakeDamage(damageObject.GetPointOfContact(), new Elements(Elemental.NONE), GetATK() * 3.5f, this);
            }
        }
        return colliders;
    }

    public override void LaunchBasicAttack()
    {
        if (Time.timeScale == 0)
            return;

        if (Time.time - LastClickedTime > AttackRate)
        {
            SetLookAtTarget();
            ShootDirection = Direction;
            Animator.SetTrigger("Attack1");
            LastClickedTime = Time.time;
        }
    }

    public void LaunchChargedAttack()
    {
        if (Time.time - LastClickedTime > ChargedAttackRate)
        {
            ShootElemental = GetBowCharactersState().BowData.CurrentElemental;
            Animator.SetTrigger("Attack1");
            LastClickedTime = Time.time;
        }
    }

    protected override void ChargeTrigger()
    {
        if (!GetPlayerManager().CanPerformAction() && GetPlayerManager().GetPlayerMovementState() is not PlayerAimState)
            return;

        GetPlayerCharacterState().ChargeTrigger();
    }


    public void SpawnChargeUpFinish()
    {
        ParticleSystem ps = Instantiate(ChargeUpFinishPrefab, EmitterPivot).GetComponent<ParticleSystem>();
        Destroy(ps.gameObject, ps.main.duration);
    }

    public void SpawnChargeEmitter()
    {
        if (ChargeUpEmitter != null)
            return;

        ChargeUpEmitter = Instantiate(AssetManager.GetInstance().ChargeUpEmitterPrefab, EmitterPivot).GetComponent<ParticleSystem>();
        foreach(ParticleSystem ps in ChargeUpEmitter.GetComponentsInChildren<ParticleSystem>())
        {
            var mainModule = ps.main;
            mainModule.duration = GetBowCharactersState().BowData.ChargedMaxElapsed;
        }
        ChargeUpEmitter.Play();
    }

    private Vector3 GetFirstTargetHits(float length)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit[] raycastsHitAll = GetRayPositionAll3D(ray.origin, ray.direction, length);
        List<RaycastHit> hits = new List<RaycastHit>(raycastsHitAll);

        for(int i = hits.Count - 1; i >= 0; i--)
        {
            RaycastHit hit = hits[i];
            if (Vector3.Distance(hit.point, ray.origin) < Vector3.Distance(ray.origin, EmitterPivot.position))
            {
                hits.Remove(hit);
            }
        }

        hits.Sort((hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

        if (hits.Count > 0)
            return hits[0].point;

        return ray.origin + ray.direction * length;
    }

    public void InitHitPos_Aim()
    {
        if (Time.timeScale == 0)
            return;

        if (CrossHair == null)
            CrossHair = AssetManager.GetInstance().SpawnCrossHair();

        Direction = GetFirstTargetHits(20f) - EmitterPivot.position;
        ShootDirection = Direction;
        LookAtDirection(Camera.main.transform.forward);
    }

    public override void SetLookAtTarget()
    {
        Direction = GetPlayerManager().transform.forward;
        if (NearestTarget != null)
        {
            Vector3 lookdir = NearestTarget.GetPointOfContact() - GetPlayerManager().GetPlayerOffsetPosition().position;
            Vector3 forward = lookdir;
            forward.y = 0;
            forward.Normalize();
            LookAtDirection(forward);
            Direction = lookdir;
        }
    }

    public void DestroyChargeUpEmitter()
    {
        if (ChargeUpEmitter)
            Destroy(ChargeUpEmitter.gameObject);

        if (GetBowCharactersState() != null)
        {
            GetBowCharactersState().BowData.isChargedFinish = false;
            GetBowCharactersState().BowData.ChargeElapsed = 0;
        }
    }

    public void DestroyCrossHair()
    {
        if (CrossHair)
            Destroy(CrossHair);
    }

    protected override void OnCharacterChanged(CharacterData c)
    {
        base.OnCharacterChanged(c);
        DestroyCrossHair();
    }
    protected override void OnDisable()
    {
        DestroyChargeUpEmitter();

        base.OnDisable();
    }
}