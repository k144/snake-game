using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Consumable: Spawnable, IConsumable
{
    private static int ConsumptionCount { get; set; } = 0;

    public int GetConsumptionCount() => ConsumptionCount;
    public void ResetCount() => ConsumptionCount = 0;
    public void Consume()
    {
        ConsumptionCount++;
        Debug.Log(ConsumptionCount.ToString());
    }
}

public interface IConsumable
{
    public int GetConsumptionCount();
    public void Consume();
    public void ResetCount();
}
