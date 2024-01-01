using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
    private BaseEnemy BaseEnemy;
    [SerializeField] HealthBarScript healthBarScript;
    [SerializeField] TextMeshProUGUI EnemyNameTxt;
    public void SetBaseEnemy(BaseEnemy enemy)
    {
        BaseEnemy = enemy;
    }

    // Update is called once per frame
    void Update()
    {
        if (BaseEnemy == null)
            return;

        if (healthBarScript)
        {
            healthBarScript.UpdateHealth(BaseEnemy.GetHealth(), 0, BaseEnemy.GetMaxHealth());
            healthBarScript.UpdateShield(BaseEnemy.GetCurrentElementalShield(), 0, BaseEnemy.GetElementalShield());
            healthBarScript.UpdateLevel(BaseEnemy.GetLevel());
        }


        EnemyNameTxt.text = BaseEnemy.GetCharactersSO().ItemName;
    }
}
