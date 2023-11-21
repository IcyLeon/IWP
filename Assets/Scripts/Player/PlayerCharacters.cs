using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerCharacters : Characters
{
    protected float AimSpeed = 20f;
    private CharacterData characterData;
    private PlayerController playerController;
    protected bool isBurstActive;
    private Coroutine CameraZoomAndPosOffsetCoroutine;
    [SerializeField] CinemachineVirtualCamera BurstCamera;
    protected Characters NearestEnemy;
    protected float Range = 1f;

    public void SetBurstActive(bool value)
    {
        isBurstActive = value;
    }
    public bool GetBurstActive()
    {
        return isBurstActive;
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
        GetPlayerController().GetCharacterRB().MovePosition(GetPlayerController().GetCharacterRB().position + deltaPosition);
        GetPlayerController().GetCharacterRB().transform.rotation = rootRotation;
    }

    public void SetCharacterData(CharacterData characterData)
    {
        this.characterData = characterData;
    }

    public override Elements TakeDamage(Vector3 pos, Elements elements, float amt)
    {
        Elements e = base.TakeDamage(pos, elements, amt);
        return e;
    }

    private ElementsIndicator GetElementsIndicator()
    {
        return CharacterManager.GetInstance().GetElementsIndicator();
    }

    private void SetElementsIndicator(ElementsIndicator ElementsIndicator)
    {
        CharacterManager.GetInstance().SetElementsIndicator(ElementsIndicator);
    }

    public Characters GetNearestCharacters(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(GetPlayerController().transform.position, range, LayerMask.GetMask("Entity"));

        if (colliders.Length == 0)
            return null;

        List<Collider> colliderCopy = new List<Collider>(colliders);

        Collider nearestCollider = colliderCopy[0];

        for (int i = colliderCopy.Count - 1; i >= 0; i--)
        {
            Characters c = colliderCopy[i].GetComponent<Characters>();
            if (c != null)
            {
                if (c.IsDead())
                {
                    colliderCopy.RemoveAt(i);
                }
                else
                {
                    float dist1 = Vector3.Distance(colliderCopy[i].transform.position, GetPlayerController().transform.position);
                    float dist2 = Vector3.Distance(nearestCollider.transform.position, GetPlayerController().transform.position);

                    if (dist1 <= dist2)
                    {
                        nearestCollider = colliderCopy[i];
                    }
                }
            }
        }

        if (colliderCopy.Count == 0)
            nearestCollider = null;


        if (nearestCollider != null)
            return nearestCollider.GetComponent<Characters>();
        else
            return null;
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

        return characterData.GetMaxHealth();
    }

    public override float GetDamage()
    {
        if (characterData == null)
            return base.GetDamage();

        return characterData.GetDamage();
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

        return characterData.GetDEF();
    }

    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    protected override void Start()
    {
        isBurstActive = false;

        healthBarScript = MainUI.GetInstance().GetPlayerHealthBar();

        if (GetBurstCamera())
            GetBurstCamera().gameObject.SetActive(false);

        base.Start();
    }

    protected virtual Collider[] PlungeAttackGroundHit(Vector3 HitPos)
    {
        Collider[] colliders = Physics.OverlapSphere(GetPlayerController().transform.position, 5f, LayerMask.GetMask("Entity"));

        AssetManager.GetInstance().SpawnParticlesEffect(HitPos, AssetManager.GetInstance().PlungeParticlesEffect);

        return colliders;
    }

    private IEnumerator UpdateDefaultPosOffsetAndZoomAnim(float delay)
    {
        if (GetPlayerController() == null)
            yield break;

        yield return new WaitForSeconds(delay);
        GetPlayerController().UpdateDefaultPosOffsetAndZoom();
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
        NearestEnemy = GetNearestCharacters(Range);
        base.Update();

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
            Animator.SetBool("isFalling", GetPlayerController().GetPlayerActionStatus() == PlayerActionStatus.FALL);
            Animator.SetFloat("Velocity", GetPlayerController().GetInputDirection().magnitude, 0.15f, Time.deltaTime);
            Animator.SetBool("isGrounded", GetPlayerController().IsInMovingState());
            Animator.SetBool("isWalking", 
                GetPlayerController().GetPlayerActionStatus() == PlayerActionStatus.WALK ||
                GetPlayerController().GetPlayerActionStatus() == PlayerActionStatus.SPRINTING
                );
        }
    }

    protected virtual void FixedUpdate()
    {
        GetPlayerController().UpdatePhysicsMovement();
        GetPlayerController().UpdateTargetRotation();
    }

    public Vector3 GetRayPosition3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        if (GetPlayerController() == null)
            return Vector3.zero;

        return GetPlayerController().GetRayPosition3D(origin, direction, maxdistance);
    }

    protected void UpdateInputTargetQuaternion()
    {
        if (GetPlayerController() == null)
            return;

        GetPlayerController().UpdateInputTargetQuaternion();
    }

    protected void SetTargetRotation(Quaternion quaternion)
    {
        if (GetPlayerController() == null)
            return;

        GetPlayerController().SetTargetRotation(quaternion);
    }

    protected void LookAtDirection(Vector3 dir)
    {
        Quaternion quaternion = Quaternion.LookRotation(dir);
        SetTargetRotation(quaternion);
    }
    protected void ResetVelocity()
    {
        if (GetPlayerController() == null)
            return;
        GetPlayerController().ResetVelocity();
    }

    protected void UpdateCameraAim()
    {
        if (GetPlayerController() == null)
            return;

        GetPlayerController().UpdateAim();
    }

    protected virtual void ElementalSkillTrigger()
    {
    }

    protected virtual void ElementalSkillHold()
    {
    }

    protected virtual void EKey_1Down()
    {

    }
    protected virtual bool ElementalBurstTrigger()
    {
        if (characterData == null || GetPlayerController().GetPlayerGroundStatus() != PlayerGroundStatus.GROUND)
            return false;

        if (GetCharacterData().CanTriggerBurstSKill() && GetCharacterData().CanTriggerBurstSKillCost())
        {
            characterData.ResetElementalBurstCooldown();
            if (GetBurstCamera())
            {
                GetBurstCamera().gameObject.SetActive(true);
                if (ContainsParam(Animator, "IsBurst"))
                {
                    Animator.SetBool("IsBurst", true);
                }
                Animator.SetTrigger("BurstTrigger");
            }
            return true;
        }

        return false;
    }


    protected virtual void ChargeHold()
    {

    }
    protected virtual void ChargeTrigger()
    {

    }

    public virtual void CoordinateAttackTrigger()
    {

    }

    private void OnEnable()
    {
        playerController = CharacterManager.GetInstance().GetPlayerController();
        GetPlayerController().OnElementalSkillHold += ElementalSkillHold;
        GetPlayerController().OnElementalBurstTrigger += ElementalBurstTrigger;
        GetPlayerController().OnElementalSkillTrigger += ElementalSkillTrigger;
        GetPlayerController().OnE_1Down += EKey_1Down;
        GetPlayerController().OnChargeHold += ChargeHold;
        GetPlayerController().OnChargeTrigger += ChargeTrigger;
        GetPlayerController().OnDash += Dash;
        GetPlayerController().onPlayerStateChange += PlayerStateChange;
        GetPlayerController().OnPlungeAttack += PlungeAttackGroundHit;

    }

    protected virtual void Dash()
    {
        if (!GetBurstActive() && !GetPlayerController().IsAiming())
            GetPlayerController().Dash();
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

    private void DisableInputs()
    {
        if (GetPlayerController() != null)
        {
            GetPlayerController().OnElementalSkillHold -= ElementalSkillHold;
            GetPlayerController().OnElementalBurstTrigger -= ElementalBurstTrigger;
            GetPlayerController().OnElementalSkillTrigger -= ElementalSkillTrigger;
            GetPlayerController().OnE_1Down -= EKey_1Down;
            GetPlayerController().OnChargeHold -= ChargeHold;
            GetPlayerController().OnChargeTrigger -= ChargeTrigger;
            GetPlayerController().onPlayerStateChange -= PlayerStateChange;
            GetPlayerController().OnDash -= Dash;
            GetPlayerController().OnPlungeAttack -= PlungeAttackGroundHit;
        }
    }

    private void PlayerStateChange()
    {
        if (GetPlayerController().GetPlayerActionStatus() != PlayerActionStatus.STOPPING)
            Animator.SetBool("isStopping", false);
        if (GetPlayerController().GetPlayerActionStatus() != PlayerActionStatus.DASH)
            Animator.SetBool("isDashing", false);

        switch (GetPlayerController().GetPlayerActionStatus())
        {
            case PlayerActionStatus.JUMP:
                if (Animator != null)
                    Animator.SetTrigger("Jump");
                break;
            case PlayerActionStatus.DASH:
                if (Animator != null)
                    Animator.SetBool("isDashing", true);
                break;
            case PlayerActionStatus.STOPPING:
                if (Animator != null)
                    Animator.SetBool("isStopping", true);
                break;
        }

    }
}
