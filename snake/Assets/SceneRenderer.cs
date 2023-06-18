using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class SceneRenderer : MonoBehaviour
{
    public int LevelNumber = 3;
    public GameObject Wall;
    public GameObject Food;
    public GameObject Enemy;
    public GameObject Effect;
    public GameObject Snake;

    public TMP_Text FoodScore;
    public TMP_Text EffectScore;

    private int _foodMaxScore { get; set; }
    private int _effectMaxScore { get; set; }
    
    private (int x, int y) SnakeSpawnPoint;
    
    private readonly List<(int x, int y)> FoodSpawnPoints = new();
    private readonly List<(int x, int y)> EffectSpawnPoints = new();
    private readonly List<(int x, int y)> EnemySpawnPoints = new();

    private readonly List<GameObject> _children = new();
    private Food _foodScript;
    private Effect _effectScript;
    private Enemy _enemyScript;
    

    private const byte _spawnColor = 0xAA;
    private const string WallColor = "FFFFFF";
    private const string SnakeSpawnColor = "0000FF";
    private const int _mapWidth = 35;
    private const int _mapWidthMid = _mapWidth / 2;
    private const int _mapHeight = 20;
    private const int _mapHeightMid = _mapHeight / 2;

    // Start is called before the first frame update
    private void Start()
    {
        LoadLevel(LevelNumber);
    }

    private void FixedUpdate()
    {
        var foodCount = (
            collected: _foodScript?.GetConsumptionCount(),
            required: _foodMaxScore);
        
        var effectCount = (
            collected: _effectScript?.GetConsumptionCount(),
            required: _effectMaxScore);

        FoodScore.SetText(GenerateScoreText(foodCount));
        EffectScore.SetText(GenerateScoreText(effectCount));

        if (!IsEnough(foodCount) || !IsEnough(effectCount)) return;
        
        DestroyAllChildren();
        ResetCounts();
        FoodSpawnPoints.Clear();
        EffectSpawnPoints.Clear();
        EnemySpawnPoints.Clear();

        try
        {
            LoadLevel(++LevelNumber);
        }
        catch
        {
            SceneManager.LoadScene("Scenes/End");
        }
    }

    private void ResetCounts()
    {
        _foodScript?.ResetCount();
        _effectScript?.ResetCount();
    }

    private string GenerateScoreText((int? collected, int required) count)
    {
        count.collected ??= 0;

        var sb = new StringBuilder();
        sb.Append(count.collected.ToString());
        sb.Append('/');
        sb.Append(count.required.ToString());
        return sb.ToString();
    }

    private bool IsEnough((int? collected, int required) count)
        => count.collected is null || count.required == 0 || count.collected >= count.required;

    private void LoadLevel(int levelNumber)
    {
        var bytes = LoadLevelBytes(levelNumber);
        var texture = new Texture2D(_mapWidth, _mapHeight);
        texture.LoadImage(bytes);

        for (var x = 0; x < texture.width; x++)
        {
            for (var y = 0; y < texture.height; y++)
            {
                var pixel = texture.GetPixel(x, y);
                var hex = pixel.ToHexString();

                var point = (x: x - _mapWidthMid, y: y - _mapHeightMid);

                if (hex.StartsWith(WallColor))
                {
                    Spawn(Wall, point);
                    continue;
                }

                if (hex.StartsWith(SnakeSpawnColor))
                {
                    var snake = Spawn(Snake, point);
                    var script = snake.GetComponent<Snake>();
                    script.SceneRenderer = this;
                    script.SpawnPosition = point;
                    continue;
                }
                
                if (Convert.ToInt16(pixel.r * 255) == _spawnColor)
                    EnemySpawnPoints.Add(point);
                if (Convert.ToInt16(pixel.g * 255) == _spawnColor)
                    FoodSpawnPoints.Add(point);
                if (Convert.ToInt16(pixel.b * 255) == _spawnColor)
                    EffectSpawnPoints.Add(point);
            }
        }
        
        
        _foodScript = Populate<Food>(FoodSpawnPoints, 1, Food);
        _enemyScript = Populate<Enemy>(EnemySpawnPoints, 4, Enemy);
        _effectScript = Populate<Effect>(EffectSpawnPoints, 1, Effect);

        _foodMaxScore = 20;

        _effectMaxScore = EffectSpawnPoints.Count switch
        {
            0 => 0,
            _ => levelNumber * 2
        };
    }

    private static byte[] LoadLevelBytes(int levelNumber)
        => System.IO.File.ReadAllBytes(Path.Combine("Assets", "Levels", levelNumber.ToString() + ".png"));

    private GameObject Spawn(GameObject prefab, (float x, float y) point)
    {
        var obj = Instantiate(prefab, new Vector3(point.x, point.y, 0), Quaternion.identity);
        _children.Add(obj);
        return obj;
    }

    private TScript Populate<TScript>(List<(int x, int y)> points, int maxInstances, GameObject prefab)
        where TScript : class, ISpawnable
    {
        if (!points.Any())
            return null;
        

        var count = points.Count();
        Debug.Log("Populuje: " + prefab.name + count);

        var spawnMap = new bool[_mapWidth, _mapHeight];


        foreach (var point in points)
        {
            Debug.Log(point.x.ToString() + " " + point.y.ToString());
            spawnMap[point.x + _mapWidthMid, point.y + _mapHeightMid] = true;
        }

        TScript firstScript = null;
        for (var i = 0; i < maxInstances && i < count; i++)
        {
            var instance = Spawn(prefab, (1000, 1000));
            var script = instance.GetComponent<TScript>();
            firstScript ??= script;
            script.SpawnPoints = points;
            script.SpawnMap = spawnMap;
        }

        return firstScript;
    }

    public void ResetMap()
    {
        ResetCounts();
        _effectScript?.ResetBoosts();
    }

    private void DestroyAllChildren()
    {
        if (!_children.Any())
            return;
        
        foreach (var child in _children)
            GameObject.Destroy(child);

        _children.Clear();
    }
    
}
