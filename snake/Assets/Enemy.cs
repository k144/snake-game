using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Spawnable
{
    private int Mobility { get; set; }
    private int updateCount = 0;
    private static readonly Vector3[] Directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
    
    private const int _mapWidth = 35;
    private const int _mapWidthMid = _mapWidth / 2;
    private const int _mapHeight = 20;
    private const int _mapHeightMid = _mapHeight / 2;

    private readonly Vector3 _grave = new Vector3(1000, 1000, 0);

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (updateCount++ % 2 == 0) return;
        
        var moveWill = Random.Range(0, 100);
        if (moveWill < Mobility) return;
        
        Move();
    }

    private void Move()
    {
        if (SpawnMap is null || this.transform.position == _grave)
            return;

        for (var tryCount = 0; tryCount < 4; tryCount++)
        {
            var direction = Directions[Random.Range(0, Directions.Length)];
            
            var nextPosition = new Vector3(
               Mathf.Round(this.transform.position.x) + direction.x,
               Mathf.Round(this.transform.position.y) + direction.y,
               0.0f);


            var isNextPointWithinSpawnArea = SpawnMap[
                Convert.ToInt32(nextPosition.x) + _mapWidthMid,
                Convert.ToInt32(nextPosition.y) + _mapHeightMid];
            
            if (!isNextPointWithinSpawnArea)
                continue;
            
            this.transform.position = nextPosition;
            break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "SnakeBody":
                Die();
                break;
        }
    }

    private void Spawn()
    {
        RandomizePosition();
        Mobility = Random.Range(10, 90);
    }

    private void Die()
    {
        this.transform.position = _grave;
        Invoke(nameof(Spawn), 2);
    }
}
