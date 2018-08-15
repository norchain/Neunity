using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Neunity.App;
using Neunity.Tools;
using Neunity.Adapters.Unity;
using System.Text;
using System.Linq;

public class ManagerCards : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Op.SetStoragePath(Application.persistentDataPath + "/smartcontract_data.jsn");
        //MergeCards();
        //TestLocalStorage();
        string a = "";
        string b = "1";
        string c = "advda";

        byte[] ba = Op.String2Bytes(a);
        byte[] bb = Op.String2Bytes(b);
        byte[] bc = Op.String2Bytes(c);

        string sba = Op.Bytes2String(ba);
        string sbb = Op.Bytes2String(bb);
        string sbc = Op.Bytes2String(bc);

        Debug.Log("");
	}
	
	
    private byte[] myID = { 123, 45, 67, 8, 9, 101, 112, 13 };

    private Card CreateCard(string id, string cardName){

        object[] paras = new object[3];
        paras[0] = myID;
        paras[1] = Op.String2Bytes(id);
        paras[2] = cardName;

        NuTP.Response response = NuContract.InvokeWithResp("", "randomCard", paras);
        if(response.header.code == NuTP.Code.Success){
            return Contract.Bytes2Card(response.body);
        }
        else{
            Debug.LogError(response.header.code + ":" + response.header.description);
            paras = new object[1];
            paras[0] = Op.String2Bytes(id);
            NuTP.Response respGetCard = NuContract.InvokeWithResp("", "getCard", paras);
            if(respGetCard.header.code == NuTP.Code.Success){
                return Contract.Bytes2Card(respGetCard.body);
            }
            else{
                return null;
            }
        }
    }

    public void MergeCards(){
        Debug.Log("Merge Begin");

        // Create Card1
        Card card1 = CreateCard("card1", "CrazyKnight");
        // Create Card2
        Card card2 = CreateCard("card2", "FloppyInfantry");

        object[] paras = new object[3];
        paras[0] = card1.id;
        paras[1] = card2.id;
        paras[2] = "SharpArchery";

        NuTP.Response response = NuContract.InvokeWithResp("", "cardMerge", paras);
        
        if (response.header.code == NuTP.Code.Success){
            Card cardNew = Contract.Bytes2Card(response.body);
            Debug.Log("New card's ID:\t" + cardNew.id);
            Debug.Log("New card's name:\t" + cardNew.name);
            Debug.Log("New card's birthBlock:\t" + cardNew.birthBlock);
        }
        else{
            Debug.Log("failed to merge card");
        }
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
