
using UnityEngine;

public class Food : Spawnable, IConsumable
{
    public BoxCollider2D gridArea;
    private static int ConsumptionCount { get; set; } = 0;

    public int GetConsumptionCount() => ConsumptionCount;
    public void ResetCount() => ConsumptionCount = 0;
    public void Consume()
    {
        ConsumptionCount++;
        Debug.Log(ConsumptionCount.ToString());
    }

    private void Start()
    {
        RandomizePosition();
    }

    private void OnTriggerEnter2D(Collider2D other) //tutaj sprawia kod, ?e jedzenie reaguje na snake'a...
    {
        if (other.tag != "Player") return;
        
        RandomizePosition(); // ... i nastepnie, jak snake zje, potem pojawia sie znowu w random miejscu
        Consume();
    }
}
