using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Objectives : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WavesTxt;
    [SerializeField] TextMeshProUGUI DefeatedTxt;
    private EnemyManager EM;

    // Start is called before the first frame update
    void Start()
    {
        EM = EnemyManager.GetInstance();
        EM.OnEnemyDefeatedChange += OnEnemyDefeatedChange;
        EM.OnEnemyWaveChange += OnEnemyWaveChange;
        OnEnemyDefeatedChange();
        OnEnemyWaveChange();
    }

    void OnEnemyWaveChange()
    {
        WavesTxt.text = "Wave: " + EM.GetCurrentWave();
        StartCoroutine(UpdateDefeated());
    }
    void OnEnemyDefeatedChange()
    {
        StartCoroutine(UpdateDefeated());
    }

    IEnumerator UpdateDefeated()
    {
        yield return null;
        DefeatedTxt.text = "Enemy Defeated: " + EM.GetCurrentEnemyDefeated() + "/" + EM.GetEnemiesCount();
    }

    private void OnDestroy()
    {
        EM.OnEnemyDefeatedChange -= OnEnemyDefeatedChange;
        EM.OnEnemyWaveChange -= OnEnemyWaveChange;
    }
}
