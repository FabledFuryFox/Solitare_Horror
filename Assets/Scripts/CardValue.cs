using UnityEngine;

public enum Suit
{
    Heart,
    Spade,
    Club,
    Diamond
}
;

public class CardValue : MonoBehaviour
{
    [Range(2, 11)]
    public int cardValue = 2;
    public Vector3 cardPosition;
    public Suit suit;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
