using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossUIManager : MonoBehaviour
{
    private EnemyManager EM;
    private Dictionary<BossHealthBar, BaseEnemy> BossEnemyUI_Dictionary;
    [SerializeField] GameObject UIContent;
    [SerializeField] GameObject BossHealthPrefab;

    private void Start()
    {
        BossEnemyUI_Dictionary = new();
        EM = EnemyManager.GetInstance();
        EM.OnBossEnemyAdd += OnBossEnemyAdd;
        EM.OnBossEnemyRemove += OnBossEnemyRemove;
    }

    private void OnDestroy()
    {
        EM.OnBossEnemyAdd -= OnBossEnemyAdd;
        EM.OnBossEnemyRemove -= OnBossEnemyRemove;
    }

    private BossHealthBar GetKeyFromValue(BaseEnemy value)
    {
        foreach (var pair in BossEnemyUI_Dictionary)
        {
            if (pair.Value == value)
            {
                return pair.Key;
            }
        }
        return null;
    }

    void OnBossEnemyRemove(BaseEnemy baseEnemy)
    {
        BossHealthBar BHB = GetKeyFromValue(baseEnemy);
        if (BHB == null)
            return;

        if (BossEnemyUI_Dictionary.TryGetValue(BHB, out BaseEnemy BE))
        {
            BossEnemyUI_Dictionary.Remove(BHB);
            Destroy(BHB.gameObject);
        }
    }

    void OnBossEnemyAdd(BaseEnemy baseEnemy)
    {
        if (baseEnemy == null)
            return;

        BossHealthBar BossHealthBar = Instantiate(BossHealthPrefab, UIContent.transform).GetComponent<BossHealthBar>();
        BossHealthBar.SetBaseEnemy(baseEnemy);
        BossEnemyUI_Dictionary.Add(BossHealthBar, baseEnemy);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateContent();
    }


    public void UpdateContent()
    {
        for (int i = BossEnemyUI_Dictionary.Count - 1; i > 0; i--)
        {
            KeyValuePair<BossHealthBar, BaseEnemy> keyValuePair = BossEnemyUI_Dictionary.ElementAt(i);
            if (BossEnemyUI_Dictionary.TryGetValue(keyValuePair.Key, out BaseEnemy BE))
            {
                if (BE == null)
                {
                    BossEnemyUI_Dictionary.Remove(keyValuePair.Key);
                    Destroy(keyValuePair.Key.gameObject);
                }
            }
        }

        UIContent.SetActive(EM.GetBossEnemyList().Count != 0);
    }
}
