using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KaqingTeleporter : MonoBehaviour
{
    [SerializeField] AudioSource ExplosionAudioSource;
    [SerializeField] GameObject ExplosionEffects;
    private Elements elements;
    private PlayerCharacters PlayerCharacters;
    private bool EnergyOrbMoving;
    private Vector3 TargetLoc;

    public event Action<PlayerCharacters> OnDestroyOrb;
    public void SetTargetLoc(Vector3 pos)
    {
        TargetLoc = pos;
    }
    // Start is called before the first frame update
    void Start()
    {
        EnergyOrbMoving = true;
        StartCoroutine(MoveToTargetLocation(50f));
    }

    private void OnDestroy()
    {
        OnDestroyOrb?.Invoke(PlayerCharacters);
    }

    public void SetElements(Elements elements)
    {
        this.elements = elements;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCharacters pc = other.GetComponent<PlayerCharacters>();
        if (pc != null)
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast") || other.gameObject.layer == LayerMask.NameToLayer("Ignore Collision"))
            return;

        EnergyOrbMoving = false;
    }

    private IEnumerator MoveToTargetLocation(float speed)
    {
        while (transform.position != TargetLoc && EnergyOrbMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetLoc, speed * Time.deltaTime);
            yield return null;
        }
        Explode();
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject, PlayerCharacters.GetCharacterData().GetPlayerCharacterSO().ElementalSkillsTimer);
        EnergyOrbMoving = false;
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.localScale.magnitude * 2f, LayerMask.GetMask("Entity", "BossEntity"));
        for(int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            IDamage damage = collider.GetComponent<IDamage>();
            if (damage != null)
            {
                if (!damage.IsDead())
                {
                    Vector3 hitPosition = collider.ClosestPointOnBounds(transform.position);
                    damage.TakeDamage(hitPosition, elements, PlayerCharacters.GetATK() * 1.25f, PlayerCharacters);
                }
            }
        }

        ParticleSystem ps = Instantiate(ExplosionEffects, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(ps.gameObject, ps.main.duration);
        ExplosionAudioSource.Play();
    }

    public void SetCharacterData(PlayerCharacters characterData)
    {
        PlayerCharacters = characterData;
    }
    public bool GetEnergyOrbMoving()
    {
        return EnergyOrbMoving;
    }
}
