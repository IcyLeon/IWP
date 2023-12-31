using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUIManager : MonoBehaviour
{
    [SerializeField] GameObject UIContent;
    [SerializeField] HealthBarScript HealthBarScript;
    [SerializeField] TextMeshProUGUI EnemyNameTxt;
    private BaseEnemy BaseEnemy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateContent();
    }

    public void SetBaseEnemy(BaseEnemy e)
    {
        BaseEnemy = e;
    }

    public void UpdateContent()
    {
        if (BaseEnemy == null)
            return;

        if (HealthBarScript != null)
        {
            HealthBarScript.SetupMinAndMax(0, BaseEnemy.GetMaxHealth());
            HealthBarScript.UpdateHealth(BaseEnemy.GetHealth());
        }

        UIContent.SetActive(!BaseEnemy.IsDead());
    }
}
