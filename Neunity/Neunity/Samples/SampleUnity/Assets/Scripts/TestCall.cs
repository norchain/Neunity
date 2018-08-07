using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Neunity.SomeCardGame;


public class TestCall : MonoBehaviour {

	// Use this for initialization
	void Start () {
        TestMergeCards();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void TestMergeCards(){
        Debug.Log("Test Begin");
        CardGame.Card card1 = new CardGame.Card()
        {
            type = CardGame.TypeArmy.Archer,
            lvls = new byte[4] { 1, 2, 4, 5 },
            score = 12,
            name = "Nice Archer"
        };

        CardGame.Card card2 = new CardGame.Card()
        {
            type = CardGame.TypeArmy.Cavalry,
            lvls = new byte[4] { 12, 0, 3, 5 },
            score = 55,
            name = "The last Knight"
        };

        byte[] b1 = CardGame.Card2Bytes(card1);
        byte[] b2 = CardGame.Card2Bytes(card2);

        byte[] bn = (byte[])CardGame.Main("cardMerge", b1, b2, "NovaCard");

        CardGame.Card cardNew = CardGame.Bytes2Card(bn);

        Debug.Log("New card's name:\t" + cardNew.name);

        string lvlString = "";
        for (int i = 0; i < cardNew.lvls.Length; i++){
            lvlString += cardNew.lvls[i].ToString() + ",";
        }
        Debug.Log("New card's lvls:\t" + lvlString);
        Debug.Log("New card's score:\t" + cardNew.score);
        Debug.Log("New card's type:\t" + cardNew.type);
    }
}
