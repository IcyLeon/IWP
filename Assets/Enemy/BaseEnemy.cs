using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : Characters
{
    private ElementsIndicator elementsIndicator;
    protected float Ratio;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        CurrentHealth = GetMaxHealth();
        Level = 1;
        healthBarScript = Instantiate(AssetManager.GetInstance().EnemyHealthUIPrefab).GetComponent<HealthBarScript>();
        elementalReaction = new ElementalReaction();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateHealthBar();

        if (elementsIndicator)
            elementsIndicator.transform.position = transform.position + Vector3.up * 2.1f;

        if (GetElementalReaction() != null)
            GetElementalReaction().UpdateElementsList();
    }

    private void UpdateHealthBar()
    {
        healthBarScript.transform.position = transform.position + Vector3.up * 1.5f;
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


    public override float GetMaxHealth()
    {
        return BaseMaxHealth * (1 + Ratio);
    }
}
