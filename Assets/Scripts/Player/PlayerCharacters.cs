using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PlayerCharacters : Characters, ISkillsBurstManager
{
    protected float AimSpeed = 20f;
    private CharacterData characterData;
    private PlayerManager PlayerManager;
    protected bool isBurstActive;
    private Coroutine CameraZoomAndPosOffsetCoroutine;
    [SerializeField] CinemachineVirtualCamera BurstCamera;
    protected IDamage NearestTarget;
    protected float Range = 1f, UltiRange = 1f;
    protected PlayerCharacterState PlayerCharacterState;

    public virtual void OnEntityHitSendInfo(Elements e, IDamage d)
    {

    }
    public void SetBurstActive(bool value)
    {
        isBurstActive = value;
    }
    public bool GetBurstActive()
    {
        return isBurstActive;
    }

    public float GetUltiRange()
    {
        return UltiRange;
    }
    public PlayerCharacterState GetPlayerCharacterState()
    {
        return PlayerCharacterState;
    }

    public override Elements TakeDamage(Vector3 pos, Elements elementsREF, float amt, bool callHitInfo = true)
    {
        if (characterData != null)
            characterData.SetPreviousHealthRatio(characterData.GetHealth() / characterData.GetActualMaxHealth(characterData.GetLevel()));

        return base.TakeDamage(pos, elementsREF, amt, callHitInfo);
    }
    public CinemachineVirtualCamera GetBurstCamera()
    {
        return BurstCamera;
    }
    public CharacterData GetCharacterData()
    {
        return characterData;
    }

    public override Vector3 GetPointOfContact()
    {
        return GetPlayerManager().GetPlayerOffsetPosition().position;
    }

    public override ElementalReaction GetElementalReaction()
    {
        if (characterData == null)
            return new ElementalReaction();

        return characterData.GetElementalReaction();
    }

    public PlayerCharacterSO GetPlayersSO()
    {
        return CharactersSO as PlayerCharacterSO;
    }

    public override void AnimatorMove(Vector3 deltaPosition, Quaternion rootRotation)
    {
        if (GetPlayerManager() == null)
            return;

        if (!GetPlayerManager().GetPlayerMovementState().CheckIfisAboutToFall())
        {
            GetPlayerManager().GetCharacterRB().MovePosition(GetPlayerManager().GetCharacterRB().position + deltaPosition);
        }
        GetPlayerManager().GetCharacterRB().transform.rotation = rootRotation;
    }

    public void SetCharacterData(CharacterData characterData)
    {
        this.characterData = characterData;
    }

    private ElementsIndicator GetElementsIndicator()
    {
        return PlayerManager.GetElementsIndicator();
    }

    private void SetElementsIndicator(ElementsIndicator ElementsIndicator)
    {
        PlayerManager.SetElementsIndicator(ElementsIndicator);
    }

    public override bool IsDead()
    {
        if (GetCharacterData() != null)
            return GetCharacterData().IsDead();

        return base.IsDead();
    }
    public override int GetLevel()
    {
        if (characterData == null)
            return -1;

        return characterData.GetLevel();
    }

    public override float GetHealth()
    {
        if (characterData == null)
            return 1;

        return characterData.GetHealth();
    }

    public override float GetMaxHealth()
    {
        if (characterData == null)
            return base.GetMaxHealth();

        return characterData.GetActualMaxHealth(characterData.GetLevel());
    }

    public override float GetATK()
    {
        if (characterData == null)
            return base.GetATK();

        return characterData.GetActualATK(characterData.GetLevel());
    }

    public override void SetHealth(float val)
    {
        if (characterData == null)
            return;

        characterData.SetHealth(val);
    }

    public override float GetDEF()
    {
        if (characterData == null)
            return base.GetDEF();

        return characterData.GetActualDEF(characterData.GetLevel());
    }

    public PlayerManager GetPlayerManager()
    {
        return PlayerManager;
    }
    protected override void Start()
    {
        SetBurstActive(false);

        healthBarScript = MainUI.GetInstance().GetPlayerHealthBar();
        PlayerManager.onEntityHitSendInfo += OnEntityHitSendInfo;
        if (GetBurstCamera())
            GetBurstCamera().gameObject.SetActive(false);

        base.Start();
    }

    protected virtual Collider[] PlungeAttackGroundHit(Vector3 HitPos)
    {
        Collider[] colliders = Physics.OverlapSphere(GetPlayerManager().transform.position, 5f, LayerMask.GetMask("Entity"));

        AssetManager.GetInstance().SpawnParticlesEffect(HitPos, AssetManager.GetInstance().PlungeParticlesEffect);

        return colliders;
    }

    private IEnumerator UpdateDefaultPosOffsetAndZoomAnim(float delay)
    {
        if (GetPlayerManager() == null)
            yield break;

        yield return new WaitForSeconds(delay);
        GetPlayerManager().GetPlayerController().UpdateDefaultPosOffsetAndZoom();
        yield return null;
    }

    public void UpdateDefaultPosOffsetAndZoom(float delay)
    {
        if (CameraZoomAndPosOffsetCoroutine != null)
        {
            StopCoroutine(CameraZoomAndPosOffsetCoroutine);
        }

        if (gameObject.activeSelf)
            CameraZoomAndPosOffsetCoroutine = StartCoroutine(UpdateDefaultPosOffsetAndZoomAnim(delay));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (GetPlayerManager() == null)
            return;


        GetPlayerCharacterState().Update();

        if (GetPlayerManager().isDeadState())
            return;

        NearestTarget = GetNearestCharacters(GetPlayerManager().GetPlayerOffsetPosition().position, Range, LayerMask.GetMask("Entity", "BossEntity"));

        if (GetElementalReaction().GetElementList().Count != 0)
        {
            if (GetElementsIndicator() == null)
            {
                SetElementsIndicator(Instantiate(AssetManager.GetInstance().ElementalContainerUIPrefab, MainUI.GetInstance().GetElementalDisplayUITransform()).GetComponent<ElementsIndicator>());
                GetElementsIndicator().SetCharacters(this);
            }
        }
        else
        {
            if (GetElementsIndicator() != null)
            {
                Destroy(GetElementsIndicator().gameObject);
            }
        }

        if (Animator)
        {
            Animator.SetBool("isFalling", GetPlayerManager().GetPlayerMovementState() is PlayerFallingState && !GetPlayerManager().IsGrounded());
            Animator.SetFloat("Velocity", GetPlayerManager().GetPlayerController().GetAnimationSpeed(), 0.15f, Time.deltaTime);
            Animator.SetBool("isGrounded", GetPlayerManager().IsGrounded());
            if (ContainsParam(Animator, "isWalking"))
                Animator.SetBool("isWalking", GetPlayerManager().IsMoving());
        }
    }

    protected Vector3 GetRayPosition3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        if (GetPlayerManager() == null)
            return Vector3.zero;

        return GetPlayerManager().GetPlayerController().GetRayPosition3D(origin, direction, maxdistance);
    }

    protected RaycastHit[] GetRayPositionAll3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        if (GetPlayerManager() == null)
            return default(RaycastHit[]);

        return GetPlayerManager().GetPlayerController().GetRayPositionAll3D(origin, direction, maxdistance);
    }

    private void SetTargetRotation(Quaternion quaternion)
    {
        if (GetPlayerManager() == null)
            return;

        GetPlayerManager().GetPlayerController().GetPlayerState().GetPlayerMovementState().UpdateTargetRotation_Instant(quaternion);
    }

    protected void LookAtDirection(Vector3 dir)
    {
        Quaternion quaternion = Quaternion.LookRotation(dir);
        SetTargetRotation(quaternion);
    }

    public void UpdateCameraAim()
    {
        if (GetPlayerManager() == null)
            return;

        GetPlayerManager().GetPlayerController().UpdateAim();
    }

    public override bool UpdateDie()
    {
        if (characterData.GetHealth() <= 0)
            return true;

        return false;
    }

    protected virtual bool ElementalSkillTrigger()
    {
        if (!GetCharacterData().CanTriggerESKill() || !GetPlayerManager().CanPerformAction())
            return false;

        GetPlayerCharacterState().ElementalSkillTrigger();
        GetCharacterData().ResetElementalSkillCooldown();

        return true;
    }

    protected virtual bool ElementalSkillHold()
    {
        if (!GetCharacterData().CanTriggerESKill() || !GetPlayerManager().CanPerformAction())
            return false;

        GetPlayerCharacterState().ElementalSkillHold();
        return true;
    }

    protected virtual bool ElementalBurstTrigger()
    {
        if (characterData == null || !GetPlayerManager().CanPerformAction())
            return false;

        if (GetCharacterData().CanTriggerBurstSKill() && GetCharacterData().CanTriggerBurstSKillCost())
        {
            characterData.ResetElementalBurstCooldown();
            GetPlayerCharacterState().ElementalBurstTrigger();
            return true;
        }

        return false;
    }

    public virtual void OnBurstAnimationDone()
    {
        SetBurstActive(false);
    }

    public virtual void SetLookAtTarget()
    {
        if (NearestTarget != null)
        {
            Vector3 forward = NearestTarget.GetPointOfContact() - GetPlayerManager().GetPlayerOffsetPosition().position;
            forward.y = 0;
            forward.Normalize();
            LookAtDirection(forward);
        }
    }

    protected virtual void ChargeHold()
    {
        if (!GetPlayerManager().CanPerformAction())
            return;

        GetPlayerCharacterState().ChargeHold();
    }
    protected virtual void ChargeTrigger()
    {
        if (!GetPlayerManager().CanPerformAction())
            return;

        GetPlayerCharacterState().ChargeTrigger();
    }

    public virtual void LaunchBasicAttack()
    {

    }

    private void OnEnable()
    {
        PlayerManager = transform.root.GetComponent<PlayerManager>();
        GetPlayerManager().GetPlayerController().OnElementalSkillHold += ElementalSkillHold;
        GetPlayerManager().GetPlayerController().OnElementalBurstTrigger += ElementalBurstTrigger;
        GetPlayerManager().GetPlayerController().OnElementalSkillTrigger += ElementalSkillTrigger;
        GetPlayerManager().GetPlayerController().OnChargeHold += ChargeHold;
        GetPlayerManager().GetPlayerController().OnChargeTrigger += ChargeTrigger;
        GetPlayerManager().GetPlayerController().OnPlungeAttack += PlungeAttackGroundHit;
        GetPlayerManager().onCharacterChange += OnCharacterChanged;
    }

    protected void BasicAttackTrigger()
    {
        PlayerCoordinateAttackManager.CallCoordinateAttack();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DisableInputs();
    }

    protected virtual void OnDisable()
    {
        DisableInputs();
    }

    protected virtual void OnCharacterChanged(CharacterData c)
    {
        UpdateDefaultPosOffsetAndZoom(0f);
    }

    private void DisableInputs()
    {
        if (GetPlayerManager() == null)
            return;

        GetPlayerManager().GetPlayerController().OnElementalSkillHold -= ElementalSkillHold;
        GetPlayerManager().GetPlayerController().OnElementalBurstTrigger -= ElementalBurstTrigger;
        GetPlayerManager().GetPlayerController().OnElementalSkillTrigger -= ElementalSkillTrigger;
        GetPlayerManager().GetPlayerController().OnChargeHold -= ChargeHold;
        GetPlayerManager().GetPlayerController().OnChargeTrigger -= ChargeTrigger;
        GetPlayerManager().GetPlayerController().OnPlungeAttack -= PlungeAttackGroundHit;
        GetPlayerManager().onCharacterChange -= OnCharacterChanged;
    }

    public void TriggerOnAnimationTransition()
    {
        GetPlayerManager().GetPlayerController().OnAnimationTransition();
    }

    public void TriggerOnCharacterStateAnimationTransition()
    {
        GetPlayerCharacterState().OnAnimationTransition();
    }

    public void TriggerNextAtkTransition()
    {
        ResetAttack();
        if (ContainsParam(Animator, "NextAtk"))
            Animator.SetBool("NextAtk", true);
    }

    public virtual void UpdateISkills()
    {
        GetPlayerCharacterState().UpdateElementalSkill();
    }
    public virtual void UpdateEveryTime()
    {

    }

    public void StopBurstAnimation()
    {
        if (GetBurstCamera())
        {
            GetBurstCamera().gameObject.SetActive(false);
            GetPlayerManager().GetPlayerController().GetCameraManager().Recentering();
        }
    }

    public virtual bool ISkillsEnded()
    {
        return true;
    }

    public virtual void UpdateIBursts()
    {
        GetPlayerCharacterState().UpdateBurst();
    }

    public virtual bool IBurstEnded()
    {
        return true;
    }
}
