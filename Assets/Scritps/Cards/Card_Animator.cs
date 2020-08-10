using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

public class CardAnimation
{
    Card card;
    Vector2 destination;
    Quaternion rotation;

    public CardAnimation(Card c, Vector2 pos)
    {
        card = c;
        destination = pos;
        rotation = Quaternion.identity;
    }

    public CardAnimation(Card c, Vector2 pos, Quaternion rot)
    {
        card = c;
        destination = pos;
        rotation = rot;
    }

    public bool Play()
    {
        bool finished = false;

        if (Vector2.Distance(card.transform.position, destination) < Constants.CARD_SNAP_DISTANCE)
        {
            card.transform.position = destination;
            finished = true;
        }
        else
        {
            card.transform.position = Vector2.MoveTowards(card.transform.position, destination, Constants.CARD_MOVEMENT_SPEED * Time.deltaTime);
            card.transform.rotation = Quaternion.Lerp(card.transform.rotation, rotation, Constants.CARD_ROTATION_SPEED * Time.deltaTime);
        }

        return finished;
    }
}

public class Card_Animator : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;

    public List<Card> DisplayingCards = new List<Card>();

    Queue<CardAnimation> cardAnimations;

    CardAnimation currentCardAnimation;

    public UnityEvent OnAllAnimationsFinished = new UnityEvent();

    bool working = false;

    private void Awake()
    {
        cardAnimations = new Queue<CardAnimation>();
        InitializeDeck();
    }

    private void InitializeDeck()
    {

        for (int i = 0; i < 52; i++)
        {
            GameObject newGameObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);
            newGameObject.transform.parent = transform;
            Card card = newGameObject.GetComponent<Card>();
            card.SetDisplayingOrder(-1);
            DisplayingCards.Add(card);
        }
    }

    public void DealDisplayingCards(List<TVD.Player> players, int numberOfCard)
    {
        int top = DisplayingCards.Count - 1;

        List<Card> cardsToRemoveFromDeck = new List<Card>();

        for (int i = 0; i < numberOfCard; i++)
        {
            for(int j = 0; j < players.Count; j++)
            {
                Card card = DisplayingCards[top];
                players[j].ReceiveDisplayingCard(card);
                cardsToRemoveFromDeck.Add(card);

                AddCardAnimation(card, players[j].NextCardPosition(), Quaternion.identity);

                top--;
            }
        }

        foreach(Card card in cardsToRemoveFromDeck)
        {
            DisplayingCards.Remove(card);
        }
    }

    public void AddCardAnimation(Card card, Vector2 position, Quaternion rotation)
    {
        CardAnimation ca = new CardAnimation(card, position, rotation);
        cardAnimations.Enqueue(ca);
        working = true;
    }

    private void Update()
    {
        if (currentCardAnimation == null)
        {
            NextAnimation();
        }
        else
        {
            if (currentCardAnimation.Play())
            {
                NextAnimation();
            }
        }
    }

    void NextAnimation()
    {
        currentCardAnimation = null;

        if (cardAnimations.Count > 0)
        {
            CardAnimation ca = cardAnimations.Dequeue();
            currentCardAnimation = ca;
        }
        else
        {
            if (working)
            {
                working = false;
                OnAllAnimationsFinished.Invoke();
            }
        }
    }

    

}
