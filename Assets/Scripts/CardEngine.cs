using UnityEngine;
using System.Collections.Generic;

public class CardEngine : MonoBehaviour
{

    public List<GameObject> cardList = new List<GameObject>(); //list of all of the childobjects(cards) in the deck
    public Material cardBackMaterial; //material for the back of the cards
    public bool gameInSession = false;
    public List<GameObject> PlayerHand = new List<GameObject>();
    public List<GameObject> ComputerHand = new List<GameObject>();

    void Start()
    {
        CardPosition();
        foreach (GameObject card in cardList) //Sets the art on the back of the cards
        {
           GameObject cardBack = card.transform.GetChild(1).gameObject;
           cardBack.GetComponent<Renderer>().material = cardBackMaterial;
        }
        
        StartNewGame();
    }

   void CardPosition() //set the position of the cards in the deck
   {
    cardList.Clear();
    for (int i = 0; i < transform.childCount; i++) //add all of the childobjects(cards) to the list
        {
            cardList.Add(transform.GetChild(i).gameObject);
        }
    
    float j = 0;    
    foreach (GameObject card in cardList) //Takes all of the cards and posistions them like a deck
    {
        card.transform.position = new Vector3(0, j, 0);
        card.GetComponent<CardValue>().cardPosition = new Vector3(0, j, 0);
        j += 0.002f;
    }
    transform.position = new Vector3(4, 0.976f, 0); //Takes the whole deck and moves it to the starting position
   }

   void StartNewGame()
   {
    
   }
   
   
   
    void Update()
    {
        
    }
}
