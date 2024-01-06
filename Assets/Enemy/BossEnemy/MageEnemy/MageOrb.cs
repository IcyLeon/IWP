using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MageOrb : MonoBehaviour, IDamage
{
    private float BaseMaxHealth = 200;
    private float CurrentHealth;
    private HealthBarScript healthBarScript;
    [SerializeField] Collider Collider;
    private bool TimesUp;
    [SerializeField] GameObject DestroyedExplosionPrefab;
    [SerializeField] Transform HealthBarPivotParent;
    private Elemental Elemental;

    public float GetHealth()
    {
        return CurrentHealth;
    }

    public float GetMaxHealth()
    {
        return BaseMaxHealth;
    }

    public void SetTimesUp(bool val)
    {
        TimesUp = val;
    }
    public Vector3 GetPointOfContact()
    {
        return Collider.bounds.center;
    }

    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }

    public void SetHealth(float val)
    {
        CurrentHealth = val;
    }

    public Elements TakeDamage(Vector3 position, Elements elementsREF, float damageAmt, IDamage source, bool callHitInfo = true)
    {
        if (IsDead() || TimesUp)
            return null;

        Elemental elemental;
        if (elementsREF != null)
        {
            elemental = elementsREF.GetElements();
        }
        else
        {
            elemental = Elemental.NONE;
        }
        Elements e = new Elements(elemental);
        int DamageValue = Mathf.RoundToInt(damageAmt);

        SetHealth(GetHealth() - DamageValue);
        return e;

    }

    // Start is called before the first frame update
    void Start()
    {
        TimesUp = false;
        SetHealth(BaseMaxHealth);

        healthBarScript = Instantiate(AssetManager.GetInstance().EnemyHealthUIPrefab).GetComponent<HealthBarScript>();
        healthBarScript.transform.SetParent(HealthBarPivotParent, true);

    }

    void DestroyOrb()
    {
        ParticleSystem PS = Instantiate(DestroyedExplosionPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        Destroy(PS.gameObject, PS.main.duration);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBarScript)
        {
            healthBarScript.UpdateHealth(GetHealth(), 0f, GetMaxHealth());
            healthBarScript.SliderInvsibleOnlyFullHealth();
        }

        if (IsDead())
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider Collider)
    {
        MageOrb mageOrb = Collider.GetComponent<MageOrb>();

        if (mageOrb != null && TimesUp)
        {
            DestroyOrb();
        }
    }

    public ElementalReaction GetElementalReaction()
    {
        return null;
    }

    public float GetATK()
    {
        return 0f;
    }

    public float GetEM()
    {
        return 0f;
    }
}
