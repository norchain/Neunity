using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Neunity.App;
using Neunity.Tools;
using Neunity.Adapters.Unity;
using System.Text;

public class ManagerCards : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MergeCards();
        TestLocalStorage();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void MergeCards(){
        Debug.Log("Merge Begin");
        Card card1 = new Card
        {
            type = TypeArmy.Archer,
            lvls = new byte[4] { 1, 2, 4, 5 },
			birthBlock = 10002,
            name = "Nice Archer"
        };

        Card card2 = new Card
        {
            type = TypeArmy.Cavalry,
            lvls = new byte[4] { 12, 0, 3, 5 },
			birthBlock = 10055,
            name = "The last Knight"
        };

        byte[] b1 = Contract.Card2Bytes(card1);
        byte[] b2 = Contract.Card2Bytes(card2);

        byte[] bn = (byte[])Contract.Main("cardMerge", b1, b2, "NovaCard");

        Card cardNew = Contract.Bytes2Card(bn);

        Debug.Log("New card's name:\t" + cardNew.name);

        string lvlString = "";
        for (int i = 0; i < cardNew.lvls.Length; i++){
            lvlString += cardNew.lvls[i].ToString() + ",";
        }
        Debug.Log("New card's lvls:\t" + lvlString);
		Debug.Log("New card's birthBlock:\t" + cardNew.birthBlock);
        Debug.Log("New card's type:\t" + cardNew.type);
    }

    void TestLocalStorage() {
        byte[] key = Encoding.UTF8.GetBytes("name");
        byte[] name = Encoding.UTF8.GetBytes("terrence");
        NuIO.SetStorageWithKey(key, name);

        byte[] nameResult  = NuIO.GetStorageWithKey(key);
        string strName = Encoding.UTF8.GetString(nameResult);
        Debug.Log(strName);

    }
}
