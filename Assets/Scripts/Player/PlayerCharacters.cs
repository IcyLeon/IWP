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

    public override object GetSource()
    {
        return PlayerManager;
    }

    public virtual void OnEntityHitSendInfo(ElementalReactionsTrigger ER, Elements e, IDamage d)
    {

    }

    public override void FootstepSound()
    {
        if (GetPlayerManager().GetPlayerController().GetPlayerState().GetPlayerMovementState() is not PlayerMovingState)
            return;

        base.FootstepSound();
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

    public void PlayRandomSkillsRecastVoice()
    {
        if (GetPlayersSO() == null)
            return;

        if (AssetManager.isInProbabilityRange(0.75f))
            GetSoundManager().PlayVOSound(GetPlayersSO().GetRandomSkillRecastVoice());
    }

    public void PlayRandomSkillsVoice()
    {
        if (GetPlayersSO() == null)
            return;

        if (AssetManager.isInProbabilityRange(0.8f))
            GetSoundManager().PlayVOSound(GetPlayersSO().GetRandomSkillVoice());
    }
    public void PlayRandomSkillsBurstVoice()
    {
        if (GetPlayersSO() == null)
            return;

        GetSoundManager().PlayVOSound(GetPlayersSO().GetRandomBurstVoice());
    }

    public void PlayRandomFallenVoice()
    {
        if (GetPlayersSO() == null)
            return;

        GetSoundManager().PlayVOSound(GetPlayersSO().GetRandomFallenVoice());
    }

    public void PlayRandomBasicAttackVoice()
    {
        if (GetPlayersSO() == null)
            return;

        if (AssetManager.isInProbabilityRange(0.75f))
            GetSoundManager().PlayVOSound(GetPlayersSO().GetRandomBasicAttackVoice());
    }

    protected override void UpdateOutofBound()
    {
        bool OutOfBound = isOutofBound(GetPlayerManager().GetCharacterRB().position);
        if (OutOfBound)
        {
            GetPlayerManager().ResetAllEnergy();
            GetPlayerManager().AddHealthAllCharactersPercentage(-0.15f);
            GetPlayerManager().GetCharacterRB().position = GetPlayerManager().GetPlayerController().GetPlayerState().PlayerData.PreviousPosition;
        }
    }
    public override Elements TakeDamage(Vector3 pos, Elements elementsREF, float amt, IDamage source, bool callHitInfo = true)
    {
        if (GetPlayerManager().isBurstState())
            return null;

        if (characterData != null)
            characterData.SetPreviousHealthRatio(characterData.GetHealth() / characterData.GetActualMaxHealth(characterData.GetLevel()));

        return base.TakeDamage(pos, elementsREF, amt, source, callHitInfo);
    }
    public CinemachineVirtualCamera GetBurstCamera()
    {
        return BurstCamera;
    }
    public CharacterData GetCharacterData()
    {
        return characterData;
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

    public override float GetEM()
    {
        if (characterData == null)
            return base.GetEM();

        return characterData.GetActualEM(characterData.GetLevel());
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

        healthBarScript = GetPlayerManager().GetPlayerCanvasUI().GetPlayerHealthBar();
        PlayerManager.onEntityHitSendInfo += OnEntityHitSendInfo;
        if (GetBurstCamera())
            GetBurstCamera().gameObject.SetActive(false);

        base.Start();
    }

    protected virtual Collider[] PlungeAttackGroundHit(Vector3 HitPos)
    {
        Collider[] colliders = Physics.OverlapSphere(GetPointOfContact(), 5f, LayerMask.GetMask("Entity", "BossEntity"));

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

    public void ResetElementsIndicator()
    {
        if (healthBarScript)
        {
            healthBarScript.SetElementsIndicator(null);
        }

    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Time.timeScale == 0)
            return;

        if (GetPlayerManager() == null)
            return;

        //Debug.Log(PlayerCharacterState.GetCurrentState());
        //Debug.Log(GetPlayerManager().GetPlayerController().GetPlayerState().GetPlayerMovementState());

        GetPlayerCharacterState().Update();

        if (GetPlayerManager().isDeadState())
            return;

        NearestTarget = GetNearestCharacters(GetPlayerManager().GetPlayerOffsetPosition().position, Range, LayerMask.GetMask("Entity", "BossEntity"));

        if (healthBarScript)
        {
            if (GetElementalReaction().GetElementList().Count != 0)
            {
                if (healthBarScript.GetElementsIndicator() == null)
                    healthBarScript.SetElementsIndicator(this);
            }
            else
            {
                ResetElementsIndicator();
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
        if (characterData == null)
            return false;

        return characterData.GetHealth() <= 0;
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
        PlayerController.OnElementalSkillHold += ElementalSkillHold;
        PlayerController.OnElementalBurstTrigger += ElementalBurstTrigger;
        PlayerController.OnElementalSkillTrigger += ElementalSkillTrigger;
        PlayerController.OnChargeHold += ChargeHold;
        PlayerController.OnChargeTrigger += ChargeTrigger;
        PlayerController.OnPlungeAttack += PlungeAttackGroundHit;
        PlayerManager.onCharacterChange += OnCharacterChanged;
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

    protected virtual void OnCharacterChanged(CharacterData c, PlayerCharacters playerCharacters)
    {
        UpdateDefaultPosOffsetAndZoom(0f);
    }

    private void DisableInputs()
    {
        PlayerController.OnElementalSkillHold -= ElementalSkillHold;
        PlayerController.OnElementalBurstTrigger -= ElementalBurstTrigger;
        PlayerController.OnElementalSkillTrigger -= ElementalSkillTrigger;
        PlayerController.OnChargeHold -= ChargeHold;
        PlayerController.OnChargeTrigger -= ChargeTrigger;
        PlayerController.OnPlungeAttack -= PlungeAttackGroundHit;
        PlayerManager.onCharacterChange -= OnCharacterChanged;
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
