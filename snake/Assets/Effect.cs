using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : Spawnable, IConsumable
{
    private static int ConsumptionCount { get; set; } = 0;
    private static int NumberOfBoosts { get; set; } = 0;
    private static float OriginalFixedDeltaTime { get; set; }

    private const float SpeedFactor = 0.75f;
    // Start is called before the first frame update


    public int GetConsumptionCount() => ConsumptionCount;
    public void ResetCount() => ConsumptionCount = 0;
    public void Consume()
    {
        ConsumptionCount++;
        Debug.Log(ConsumptionCount.ToString());
    }
    public void ResetBoosts()
    {
        if (NumberOfBoosts == 0) return;

        Time.fixedDeltaTime = OriginalFixedDeltaTime;
        NumberOfBoosts = 0;
    }
    private void Start()
    {
        RandomizePosition();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player") return;
        
        RandomizePosition();
        Consume();
        
        if (NumberOfBoosts == 0)
            OriginalFixedDeltaTime = Time.fixedDeltaTime;
            
        Time.fixedDeltaTime *= SpeedFactor;
        NumberOfBoosts++;
        Invoke(nameof(RemoveBoost), 5);
    }

    private void RemoveBoost()
    {
        switch (NumberOfBoosts)
        {
            case 0:
                return;
            case 1:
                Time.fixedDeltaTime = OriginalFixedDeltaTime;
                NumberOfBoosts--;
                return;
            default:
                Time.fixedDeltaTime /= SpeedFactor;
                NumberOfBoosts--;
                break;
        }
    }
}
