using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

    public override Vector3 GetPointOfContact()
    {
        return GetPlayerController().GetPlayerOffsetPosition().position;
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
        if (!GetPlayerController().GetPlayerMovementState().CheckIfisAboutToFall())
        {
            GetPlayerController().GetCharacterRB().MovePosition(GetPlayerController().GetCharacterRB().position + deltaPosition);
        }
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

    public override bool IsDead()
    {
        if (GetCharacterData() != null)
            return GetCharacterData().IsDead();

        return base.IsDead();
    }

    private Collider[] GetAllNearestCharacters(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(GetPlayerController().transform.position, range, LayerMask.GetMask("Entity"));
        List<Collider> colliderCopy = new List<Collider>(colliders);
        for (int i = colliderCopy.Count - 1; i >= 0; i--)
        {
            Characters c = colliderCopy[i].GetComponent<Characters>();
            if (c != null)
            {
                Vector3 dir = GetPlayerController().GetPlayerOffsetPosition().position - c.transform.position;
                if (Physics.Raycast(c.transform.position, dir.normalized, out RaycastHit hit, range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.GetComponent<PlayerCharacters>() == null)
                    {
                        colliderCopy.RemoveAt(i);
                    }
                }

                if (c.IsDead() && i < colliderCopy.Count)
                {
                    colliderCopy.RemoveAt(i);
                }

            }
        }
        return colliderCopy.ToArray();
    }

    public Characters GetNearestCharacters(float range)
    {
        Collider[] colliders = GetAllNearestCharacters(range);

        if (colliders.Length == 0)
            return null;

        List<Collider> colliderCopy = new List<Collider>(colliders);

        Collider nearestCollider = colliderCopy[0];

        for (int i = colliderCopy.Count - 1; i >= 0; i--)
        {
            Characters c = colliderCopy[i].GetComponent<Characters>();
            if (c != null)
            {
                float dist1 = Vector3.Distance(colliderCopy[i].transform.position, GetPlayerController().transform.position);
                float dist2 = Vector3.Distance(nearestCollider.transform.position, GetPlayerController().transform.position);

                if (dist1 <= dist2)
                {
                    nearestCollider = colliderCopy[i];
                }
            }
        }

        return nearestCollider.GetComponent<Characters>();
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
        SetBurstActive(false);

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
        base.Update();

        if (GetPlayerController().isDeadState())
            return;

        NearestEnemy = GetNearestCharacters(Range);

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
            Animator.SetBool("isFalling", GetPlayerController().GetPlayerMovementState() is PlayerFallingState);
            Animator.SetFloat("Velocity", GetPlayerController().GetAnimationSpeed(), 0.15f, Time.deltaTime);
            Animator.SetBool("isGrounded", GetPlayerController().CanPerformAction());
            Animator.SetBool("isWalking", GetPlayerController().IsMoving());
        }
    }

    public Vector3 GetRayPosition3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        if (GetPlayerController() == null)
            return Vector3.zero;

        return GetPlayerController().GetRayPosition3D(origin, direction, maxdistance);
    }


    private void SetTargetRotation(Quaternion quaternion)
    {
        if (GetPlayerController() == null)
            return;

        GetPlayerController().GetPlayerState().GetPlayerMovementState().UpdateTargetRotation_Instant(quaternion);
    }

    protected void LookAtDirection(Vector3 dir)
    {
        Quaternion quaternion = Quaternion.LookRotation(dir);
        SetTargetRotation(quaternion);
    }

    protected void UpdateCameraAim()
    {
        if (GetPlayerController() == null)
            return;

        GetPlayerController().UpdateAim();
    }

    public override bool UpdateDie()
    {
        if (characterData.GetHealth() <= 0)
            return true;

        return false;
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
        if (characterData == null || !GetPlayerController().CanAttack())
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

    private void OnEnable()
    {
        playerController = CharacterManager.GetInstance().GetPlayerController();
        GetPlayerController().OnElementalSkillHold += ElementalSkillHold;
        GetPlayerController().OnElementalBurstTrigger += ElementalBurstTrigger;
        GetPlayerController().OnElementalSkillTrigger += ElementalSkillTrigger;
        GetPlayerController().OnE_1Down += EKey_1Down;
        GetPlayerController().OnChargeHold += ChargeHold;
        GetPlayerController().OnChargeTrigger += ChargeTrigger;
        GetPlayerController().GetPlayerState().OnPlayerStateChange += PlayerStateChange;
        GetPlayerController().OnPlungeAttack += PlungeAttackGroundHit;

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
            GetPlayerController().GetPlayerState().OnPlayerStateChange -= PlayerStateChange;
            GetPlayerController().OnPlungeAttack -= PlungeAttackGroundHit;
        }
    }

    private void PlayerStateChange(PlayerState state)
    {
        if (state.GetPlayerMovementState() is not PlayerStoppingState)
            Animator.SetBool("isStopping", false);
        if (state.GetPlayerMovementState() is not PlayerDashState)
            Animator.SetBool("isDashing", false);

        switch (GetPlayerController().GetPlayerMovementState().GetPlayerStateEnum())
        {
            case PlayerMovementState.PlayerStateEnum.JUMP:
                if (Animator != null)
                    Animator.SetTrigger("Jump");
                break;
            case PlayerMovementState.PlayerStateEnum.DASH:
                if (Animator != null)
                    Animator.SetBool("isDashing", true);
                break;
            case PlayerMovementState.PlayerStateEnum.STOPPING:
                if (Animator != null)
                    Animator.SetBool("isStopping", true);
                break;
            case PlayerMovementState.PlayerStateEnum.DEAD:
                if (Animator != null)
                    Animator.SetTrigger("Dead");
                break;
        }
    }
}
