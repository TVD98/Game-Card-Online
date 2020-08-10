using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TVD
{
    [Serializable]
    public class Player
    {
        public string PlayerId;
        public string PlayerName;
        public Vector2 Position;

        int numberOfDisplayingCards;

        public List<Card> DisplayingCards = new List<Card>();

        public void ReceiveDisplayingCard(Card card)
        {
            DisplayingCards.Add(card);
            card.OwnerId = PlayerId;
            numberOfDisplayingCards++;
        }

        public Vector2 NextCardPosition()
        {
            Vector2 nextPos = Position + Vector2.right * Constants.PLAYER_CARD_POSITION_OFFSET * numberOfDisplayingCards;
            return nextPos;
        }

        public void HideCardValues()
        {
            foreach (Card card in DisplayingCards)
            {
                card.SetFaceUp(false);
            }
        }

        public void ShowCardValues()
        {
            foreach (Card card in DisplayingCards)
            {
                card.SetFaceUp(true);
            }
        }

        public void SetCardValues(List<byte> cardValues)
        {
            for (int i = 0; i < DisplayingCards.Count; i++)
            {
                Card card = DisplayingCards[i];
                card.SetCardValue(cardValues[i]);
                card.SetDisplayingOrder(i);
            }
        }
    }
}
