using System.Collections.Generic;
using UnityEngine;

public abstract class Spawnable : MonoBehaviour, ISpawnable
{
    public List<(int x, int y)> SpawnPoints { private get; set; }
    public bool[,] SpawnMap { get; set; }

    public void RandomizePosition()
    {
        if (SpawnPoints is null) return;
        var newPoint = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
        this.transform.position = new Vector3(newPoint.x, newPoint.y, 0f);
    }
}

public interface ISpawnable
{
    List<(int x, int y)> SpawnPoints { set; }
    bool[,] SpawnMap { set; }
    void RandomizePosition();
}
