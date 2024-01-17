using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static DragnDropButton;

public class BossHealthBar : MonoBehaviour
{
    private BaseEnemy BaseEnemy;
    [SerializeField] HealthBarScript healthBarScript;
    [SerializeField] TextMeshProUGUI EnemyNameTxt;
    public void SetBaseEnemy(BaseEnemy enemy)
    {
        BaseEnemy = enemy;
        if (BaseEnemy != null)
        {
            BaseEnemy.OnHit += HitEvent;
            BaseEnemy.OnDeadEvent += OnDeadEvent;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (BaseEnemy == null) {
            return;
        }

        if (healthBarScript)
        {
            healthBarScript.UpdateHealth(BaseEnemy.GetHealth(), 0, BaseEnemy.GetMaxHealth());
            healthBarScript.UpdateShield(BaseEnemy.GetCurrentElementalShield(), 0, BaseEnemy.GetElementalShield());
            healthBarScript.UpdateLevel(BaseEnemy.GetLevel());
        }


        EnemyNameTxt.text = BaseEnemy.GetCharactersSO().ItemName;
    }

    void HitEvent(ElementalReactionsTrigger ER, Elements e, Characters c)
    {
        if (healthBarScript)
        {
            if (healthBarScript.GetElementsIndicator() == null)
                healthBarScript.SetElementsIndicator(BaseEnemy);
        }
    }

    private void OnDestroy()
    {
        if (BaseEnemy)
        {
            BaseEnemy.OnHit -= HitEvent;
            BaseEnemy.OnDeadEvent -= OnDeadEvent;
        }
    }

    void OnDeadEvent(BaseEnemy e)
    {
        e.OnHit -= HitEvent;
        e.OnDeadEvent -= OnDeadEvent;
    }
}
