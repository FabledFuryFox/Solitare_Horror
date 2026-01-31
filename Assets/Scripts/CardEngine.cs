using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class CardEngine : MonoBehaviour
{

    public List<GameObject> cardList = new List<GameObject>(); //list of all of the childobjects(cards) in the deck
    public Material cardBackMaterial; //material for the back of the cards
    public bool gameInSession = false;
    public List<GameObject> PlayerHand = new List<GameObject>();
    public List<GameObject> ComputerHand = new List<GameObject>();
    public GameObject DeckStartingPosition;

    private int PCardsOut = 0;
    private int CCardsOut = 0;

    
    
    
   

    void Start()
    {
        CardPosition();
        foreach (GameObject card in cardList) //Sets the art on the back of the cards
        {
           GameObject cardBack = card.transform.GetChild(1).gameObject;
           cardBack.GetComponent<Renderer>().material = cardBackMaterial;
        }
        Delay(); // Start async method
    }


    async void Delay()
    {
        await Task.Delay(1000); //wait 3 seconds
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
        card.transform.rotation = Quaternion.Euler(180, 0, 0);
        card.GetComponent<CardValue>().cardPosition = new Vector3(0, j, 0);

        j += 0.002f;
    }
    transform.position = DeckStartingPosition.transform.position; //Takes the whole deck and moves it to the starting position
   }


   void StartNewGame() //Call this function whenever a new game starts
   {
        PCardsOut = 0; //Resets the counting of cards in play for each round (affects positioning)
        CCardsOut = 0;
        GivePlayerCard();
        
        GiveCompCard(false);
        
        GivePlayerCard();
        
        GiveCompCard(true);
    

   }


   void GivePlayerCard() //This Function takes from the deck, adds it to the player's hand and positions the card to the front of the player
   {
        int RandomCardNum = Random.Range(0, cardList.Count);
        GameObject SelectedCard = cardList[RandomCardNum];
        PlayerHand.Add(SelectedCard);

        Vector3 targetPlayerPosition = new Vector3(-0.56f - (PCardsOut * 0.25f), -0.18f, -0.34f);
        Quaternion targetPlayerRotation = Quaternion.Euler(0, 180, 0);

        PCardsOut += 1;
        cardList.Remove(cardList[RandomCardNum]);
        StartCoroutine(MoveCard(SelectedCard, targetPlayerPosition, targetPlayerRotation));
   }


   void GiveCompCard(bool HiddenCard) //This Function takes from the deck, adds it to the Computer's hand and positions the card to the front of the Computer
   {
        int RandomCardNum = Random.Range(0, cardList.Count);
        GameObject SelectedCard = cardList[RandomCardNum];
        ComputerHand.Add(SelectedCard);
        
        if (!HiddenCard) //Checks to see if the card is supposed to be hidden or not (Important at the begining)
        {
           Quaternion targetComputerRotation = Quaternion.Euler(0, 0, 0); 
           Vector3 targetComputerPosition = new Vector3(-0.56f - (CCardsOut * 0.25f), -0.18f, 0.34f);
           CCardsOut += 1;
           cardList.Remove(cardList[RandomCardNum]);
           StartCoroutine(MoveCard(SelectedCard, targetComputerPosition, targetComputerRotation));

        }
        else
        {
           Quaternion targetComputerRotation = Quaternion.Euler(180, 0, 0);
           Vector3 targetComputerPosition = new Vector3(-0.56f - (CCardsOut * 0.25f), 0, 0.34f);  
           CCardsOut += 1;
           cardList.Remove(cardList[RandomCardNum]);
           StartCoroutine(MoveCard(SelectedCard, targetComputerPosition, targetComputerRotation));
        }
        
        

   }


   IEnumerator MoveCard(GameObject card, Vector3 targetPosition, Quaternion targetRotation) //Allows for the smooth animation of cards going from the deck to play
   {
    float t = 0;
    while (t < 1)
    {
        t += 0.075f * Time.deltaTime;
        card.transform.localPosition = Vector3.Lerp(card.transform.localPosition, targetPosition, t);
        card.transform.localRotation = Quaternion.Lerp(card.transform.localRotation, targetRotation, t);
        yield return null;
    }
   }
   
   
    void Update()
    {
        
    }
}
