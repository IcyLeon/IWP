using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffFood : Food
{
    private float Timer;
    public override void Update()
    {
        if (Timer > 0)
            Timer -= Time.deltaTime;
    }
    public bool isFoodBuffEnds()
    {
        return Timer <= 0f;
    }

    public void AddDurationTime()
    {
        Timer += ((BuffFoodSO)GetFoodData()).Duration;
    }
    public void ResetTimer()
    {
        Timer = ((BuffFoodSO)GetFoodData()).Duration;
    }

    public BuffFood(bool isNew, ItemTemplate itemSO) : base(isNew, itemSO)
    {
        ResetTimer();
    }
}
