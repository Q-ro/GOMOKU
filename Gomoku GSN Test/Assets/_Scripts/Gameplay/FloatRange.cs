/*
Author: Andres Mrad (Q-ro)
Creation Date : [ 2020/05 (May)/15 (Fri) ] @[ 20:59 ]
Description : Helper class to define numeric float ranges
*/


using System;
using UnityEngine;

[Serializable]
public class FloatRange
{
    [SerializeField] float minRange;
    [SerializeField] float maxRange;

    public FloatRange(float minRange, float maxRange)
    {
        this.minRange = minRange;
        this.maxRange = maxRange;
    }

    public float MinRange { get => minRange; }
    public float MaxRange { get => maxRange; }
}
