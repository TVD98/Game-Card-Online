using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProtectedData
{
    [SerializeField]
    List<byte> poolOfCards = new List<byte>();
    [SerializeField]
    List<byte> player1Cards = new List<byte>();
    [SerializeField]
    List<byte> player2Cards = new List<byte>();

    [SerializeField]
    string player1Id;
    [SerializeField]
    string playerI2d;


    public ProtectedData(string p1Id, string p2Id)
    {
        player1Id = p1Id;
        playerI2d = p2Id;
    }

    public void SetPoolOfCards(byte[] cardValues)
    {
        poolOfCards.Clear();

        for (byte i = 0; i < cardValues.Length; i++)
        {
            poolOfCards.Add(cardValues[i]);
        }

        if(cardValues.Length != Constants.INITIAL_CARDS) { return; }

        player1Cards.Clear();
        player2Cards.Clear();

        for (int i = 0; i < Constants.PLAYER_INITIAL_CARDS; i++)
        {
            int index = i * 2;
            player1Cards.Add(cardValues[index]);
            player2Cards.Add(cardValues[index + 1]);
        }

        player1Cards.Sort();
        player2Cards.Sort();
    }

    public List<byte> GetPoolOfCards()
    {
        return poolOfCards;
    }

    public void AddCardValuesToPlayer(TVD.Player player)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if(player.PlayerId == player1Id)
                player.SetCardValues(player1Cards);
            else player.SetCardValues(player2Cards);
        }
        else
        {
            if (player.PlayerId == player1Id)
                player.SetCardValues(player2Cards);
            else player.SetCardValues(player1Cards);
        }
    }

    


}
