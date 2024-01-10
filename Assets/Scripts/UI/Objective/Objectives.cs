using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Objectives : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WavesTxt;
    [SerializeField] TextMeshProUGUI DefeatedTxt;
    private EnemyManager EM;

    private void Awake()
    {
        EnemyManager.OnEnemyWaveChange += OnEnemyWaveChange;
    }
    // Start is called before the first frame update
    void Start()
    {
        EM = EnemyManager.GetInstance();
    }

    void OnEnemyWaveChange()
    {
        if (EM == null)
            return;

        WavesTxt.text = "Wave: " + EM.GetCurrentWave();
    }
    void OnEnemyDefeatedChange()
    {
        if (EM == null)
            return;
        DefeatedTxt.text = "Enemy Defeated: " + EM.GetCurrentEnemyDefeated() + "/" + EM.GetEnemiesCount();
    }

    private void Update()
    {
        OnEnemyDefeatedChange();
    }

    private void OnDestroy()
    {
        EnemyManager.OnEnemyWaveChange -= OnEnemyWaveChange;
    }
}
