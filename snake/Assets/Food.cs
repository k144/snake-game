
using UnityEngine;

public class Food : MonoBehaviour
{
    public BoxCollider2D gridArea;

    private void Start()
    {
        RandomizePosition();
    }


    private void RandomizePosition() //to sprawia, ze jedzonko pojawia sie w losowych miejscach
    {
        Bounds bounds = this.gridArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
    }

    private void OnTriggerEnter2D(Collider2D other) //tutaj sprawia kod, ?e jedzenie reaguje na snake'a...
    {
        if (other.tag == "Player")
        {
            RandomizePosition(); // ... i nastepnie, jak snake zje, potem pojawia sie znowu w random miejscu
        }
        
    }
}
