using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : Characters
{
    protected float DetectionRange;
    private ElementsIndicator elementsIndicator;
    protected float Ratio;
    private PlayerController Player;
    [SerializeField] protected NavMeshAgent NavMeshAgent;
    protected Vector3 RamdomDestination;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        CurrentHealth = GetMaxHealth();
        DetectionRange = 1f;
        Level = 1;
        healthBarScript = Instantiate(AssetManager.GetInstance().EnemyHealthUIPrefab).GetComponent<HealthBarScript>();
        elementalReaction = new ElementalReaction();
        OnElementReactionHit += ElementReactionHit;

        Player = CharacterManager.GetInstance().GetPlayerController();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateHealthBar();

        if (elementsIndicator)
            elementsIndicator.transform.position = GetModel().transform.position + Vector3.up * 2.1f;

        if (GetElementalReaction() != null)
            GetElementalReaction().UpdateElementsList();
    }

    private void UpdateHealthBar()
    {
        healthBarScript.transform.position = GetModel().transform.position + Vector3.up * 1.5f;
        healthBarScript.SliderInvsibleOnlyFullHealth();
    }

    public override Elements TakeDamage(Vector3 pos, Elements elements, float amt)
    {
        Elements e = base.TakeDamage(pos, elements, amt);

        if (elementsIndicator == null)
        {
            elementsIndicator = Instantiate(AssetManager.GetInstance().ElementalContainerPrefab).GetComponent<ElementsIndicator>();
            elementsIndicator.SetCharacters(this);
        }

        if (e.GetElements() != Elemental.NONE)
        {
            GameObject go = Instantiate(AssetManager.GetInstance().ElementalOrbPrefab, transform.position, Quaternion.identity);
        }

        return e;
    }

    protected virtual void ElementReactionHit(ElementalReactionsTrigger e)
    {

    }

    protected bool HasReachedTargetLocation(Vector3 target)
    {
        if (NavMeshAgent == null)
            return false;

        return (transform.position - target).magnitude <= NavMeshAgent.stoppingDistance + 0.5f;
    }

    protected void SetRandomDestination()
    {
        if (NavMeshAgent == null)
            return;

        Vector3 randomPoint = transform.position + new Vector3(Random.Range(-DetectionRange, DetectionRange), 0, Random.Range(-DetectionRange, DetectionRange));

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, DetectionRange, NavMesh.AllAreas))
        {
            RamdomDestination = hit.position;
        }
        else
        {
            SetRandomDestination();
        }
    }

    protected Vector3 GetPlayerLocation()
    {
        return Player.GetCharacterRB().position;
    }

    protected bool isInDetectionRange(float range)
    {
        return (transform.position - GetPlayerLocation()).magnitude <= range;
    }
    public override float GetMaxHealth()
    {
        return BaseMaxHealth * (1 + Ratio);
    }

    protected virtual void OnDestroy()
    {
        OnElementReactionHit -= ElementReactionHit;
    }


    protected void DisableAgent()
    {
        NavMeshAgent.updatePosition = false;
        NavMeshAgent.updateRotation = false;
        NavMeshAgent.velocity = Vector3.zero;
        NavMeshAgent.isStopped = true;
        NavMeshAgent.enabled = false;
    }

    protected void EnableAgent()
    {
        NavMeshAgent.enabled = true;
        NavMeshAgent.updatePosition = true;
        NavMeshAgent.updateRotation = true;
        NavMeshAgent.isStopped = false;
    }
}
