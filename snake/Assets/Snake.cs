using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

public class Snake : MonoBehaviour
{
    public Transform segmentPrefab; //baza; prefabrykat segmentu weza
    public Sprite tailSprite; //podpiete zmienne, aby potem zmiala sie grafika ciala weza
    public Sprite bodySprite;
    public Sprite angleSprite;
    public (int x, int y) SpawnPosition { get; set; }
    public SceneRenderer SceneRenderer { get; set; }

    private Vector2 _direction = Vector2.up; //wskazuje, w ktora strone idzie snek (domyslnie idzie w prawo na poczatku)
    private List<Transform> _segments;
    
    
    private Vector3 _upAngle => new Vector3(0, 0, 0);
    private Vector3 _leftAngle => new Vector3(0, 0, 90);
    private Vector3 _downAngle => new Vector3(0, 0, 180);
    private Vector3 _rightAngle => new Vector3(0, 0, 270);
       

    private void Start()
    {   
        foreach (var segment in GameObject.FindGameObjectsWithTag("SnakeBody"))
        {
            GameObject.Destroy(segment);
        }
        
        _segments = new List<Transform> { this.transform }; //utworzenie nowej listy segmentow z głową
        Grow();
        Grow();
    }


    private void Update()  //TUTAJ SPRAWIAMY, ZE NASZ WAZ SIE PORUSZA ZA POMOCA WASD
    {
        if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down) //sprawdz czy waz nie jest skierowany w dol
        {
            _direction = Vector2.up;
            this.transform.eulerAngles = _upAngle; //tutaj zmienia sie rotacja obiektu (segmentu) = zmiana rotacji glowy
        }
        else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up)
        {
            _direction = Vector2.down;
            this.transform.eulerAngles = _downAngle;
        }
        else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right)
        {
            _direction = Vector2.left;
            this.transform.eulerAngles = _leftAngle;
        }
        else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left)
        {
            _direction = Vector2.right;
            this.transform.eulerAngles = _rightAngle;
        }
    }

    private void FixedUpdate() //   
    {
        for (var i = _segments.Count - 1; i > 0; i--) //iterowanie po segmentach OD KONCA; w tej petli ignorujemy glowe, bo ma indeks 0
        {
            var sprite = GetSprite(i);

            var rotationDelta = _segments[i].eulerAngles.z - _segments[i - 1].eulerAngles.z;
            
            var isLeftTurn = 
                sprite == angleSprite 
                && rotationDelta is -90 or 270;
            
            var renderer = _segments[i].GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            _segments[i].position = _segments[i - 1].position; //odwolaj sie do segmentu i-tego i ustaw pozycje z poprzedniego segmentu (zmiana wartosci segmentow)
            _segments[i].eulerAngles = _segments[i - 1].eulerAngles; //odwolaj sie do segmentu i-tego i ustaw jego rotacje na wartosc poprzedniego segmentu

            renderer.flipX = isLeftTurn;
        }

        this.transform.position = new Vector3( //tu dopiero sie rozpoczyna ruch
            Mathf.Round(this.transform.position.x) + _direction.x,
            Mathf.Round(this.transform.position.y) + _direction.y,
            0.0f); 
    }

    private Sprite GetSprite(int index)
    {
        if (index == _segments.Count - 1) //sprawdza czy "i" jest ostatnim elementem weza (ogon)
        {
             return tailSprite; //jezeli "i" wskauzje na ogon to zamienia jego sprite'a na tailSprite
        }

        var isCorner = _segments[index].eulerAngles != _segments[index - 1].eulerAngles; //sprawdzanie czy rotacja dwoch segmentow rozni sie od siebie
        return isCorner ? angleSprite : bodySprite;
    }
    
    private void Grow() //przepis na rosniecie weza:
    {
        var segment = Instantiate(this.segmentPrefab);
        if (_segments.Any())
            segment.position = _segments[^1].position;
        
        _segments.Add(segment);
    }

    private void ResetState()
    {
        if (_segments is null)
            return;
        
        for (var i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }
        _segments.Clear();
        
        _segments.Add(this.transform);
        Grow();
        Grow();

        this.transform.position = new Vector3((float)SpawnPosition.x, (float)SpawnPosition.y, 0f);
        this.transform.eulerAngles = _upAngle;
        _direction = Vector2.up;

        this.SceneRenderer.ResetMap();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Food":
                Grow();
                break;
            case "Obstacle":
            case "SnakeBody":
            case "Enemy":
                ResetState();
                break;
        }
    }

    private void OnDestroy()
    {
        foreach (var t in _segments)
        {
            Destroy(t.gameObject);
        }
        _segments.Clear();
    }
}
