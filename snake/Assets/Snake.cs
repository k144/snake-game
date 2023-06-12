using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.up; //wskazuje, w ktora strone idzie snek (domyslnie idzie w prawo na poczatku)
    private List<Transform> _segments;
       
    public Transform segmentPrefab; //baza; prefabrykat segmentu weza
    public Sprite tailSprite; //podpiete zmienne, aby potem zmiala sie grafika ciala weza
    public Sprite bodySprite;
    public Sprite angleSprite;

    private void Start()
    {   
        _segments = new List<Transform>(); //utworzenie nowej listy segmentow, wczesniej tylko deklarowalismy
        _segments.Add(this.transform); //dodanie glowy do listy segmentow
        Grow();
        Grow();
    }


    private void Update()  //TUTAJ SPRAWIAMY, ZE NASZ WAZ SIE PORUSZA ZA POMOCA WASD
    {
        if (Input.GetKeyDown(KeyCode.W) && _direction != Vector2.down) //sprawdz czy waz nie jest skierowany w dol
        {
            _direction = Vector2.up;
            this.transform.eulerAngles = new Vector3(0, 0, 0); //tutaj zmienia sie rotacja obiektu (segmentu) = zmiana rotacji glowy
        }
        else if (Input.GetKeyDown(KeyCode.S) && _direction != Vector2.up)
        {
            _direction = Vector2.down;
            this.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else if (Input.GetKeyDown(KeyCode.A) && _direction != Vector2.right)
        {
            _direction = Vector2.left;
            this.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else if (Input.GetKeyDown(KeyCode.D) && _direction != Vector2.left)
        {
            _direction = Vector2.right;
            this.transform.eulerAngles = new Vector3(0, 0, 270);
        }
    }

    private void FixedUpdate() //   
    {
        for (int i = _segments.Count - 1; i > 0; i--) //iterowanie po segmentach OD KONCA; w tej petli ignorujemy glowe, bo ma indeks 0
        {
            if (i == _segments.Count - 1) //sprawdza czy "i" jest ostatnim elementem weza (ogon)
            {
                _segments[i].GetComponent<SpriteRenderer>().sprite = tailSprite; //jezeli "i" wskauzje na ogon to zamienia jego sprite'a na tailSprite
            }
            else
            {
                if (_segments[i].eulerAngles != _segments[i - 1].eulerAngles) //sprawdzanie czy rotacja dwoch segmentow rozni sie od siebie
                {
                    _segments[i].GetComponent<SpriteRenderer>().sprite = angleSprite; //jesli sie rozni to podmienia sprite obiektu na angleSprite (katowy)
 
                }
                else
                {
                    _segments[i].GetComponent<SpriteRenderer>().sprite = bodySprite; //jezeli sie NIE ROZNI to dodaje tylko bodySprite
                }
            }

            _segments[i].position = _segments[i - 1].position; //odwolaj sie do segmentu i-tego i ustaw pozycje z poprzedniego segmentu (zmiana wartosci segmentow)
            _segments[i].eulerAngles = _segments[i - 1].eulerAngles; //odwolaj sie do segmentu i-tego i ustaw jego rotacje na wartosc poprzedniego segmentu

        }

        this.transform.position = new Vector3( //tu dopiero sie rozpoczyna ruch
            Mathf.Round(this.transform.position.x) + _direction.x, Mathf.Round(this.transform.position.y) + _direction.y, 0.0f); 
    }

    private void Grow() //przepis na rosniecie weza:
    {
        Transform segment = Instantiate(this.segmentPrefab);

        segment.position = _segments[_segments.Count - 1].position;

        _segments.Add(segment);
    }

    private void ResetState()
    {
        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }
        _segments.Clear();
        _segments.Add(this.transform);
        Grow();
        Grow();

        this.transform.position = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            Grow();
        }
        else if (other.tag == "Obstacle")
        {
            ResetState();
        }
    }
}
