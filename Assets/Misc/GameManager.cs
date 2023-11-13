using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int CurrentWave;
    private static GameManager instance;
    public event Action OnWaveChange;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CurrentWave = 0;
    }

    public void NextWave()
    {
        CurrentWave++;
        OnWaveChange?.Invoke();
    }

    public int GetCurrentWave()
    {
        return CurrentWave;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static GameManager GetInstance()
    {
        return instance;
    }
}
