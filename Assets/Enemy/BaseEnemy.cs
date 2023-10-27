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
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        UpdateHealthBar();

        if (elementsIndicator)
            elementsIndicator.transform.position = transform.position + Vector3.up * 2f;
    }

    private void UpdateHealthBar()
    {
        healthBarScript.transform.position = transform.position + Vector3.up * 1.5f;
        healthBarScript.SliderInvsibleOnlyFullHealth();
    }

    public override void TakeDamage(Vector3 pos, Elements elements, float amt)
    {
        base.TakeDamage(pos, elements, amt);

        if (elementsIndicator == null)
        {
            elementsIndicator = Instantiate(AssetManager.GetInstance().ElementalContainerPrefab).GetComponent<ElementsIndicator>();
            elementsIndicator.SetCharacters(this);
        }
    }


    public override float GetMaxHealth()
    {
        return BaseMaxHealth * (1 + Ratio);
    }
}
