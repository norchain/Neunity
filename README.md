# Neunity

Neunity is a framework aims to 



simplifies the process of developing C# based NEO Dapp. Via Neunity, you can **share the NEO smart contract logic with corresponding Unity (or other .Net) project**. You can also **build testcases or put breakpoints to debug your smart contract locally without testnet interaction**. 

Neunity also provides a series of tools to help C# developers to improve the experience of developing NEO smart contract, and avoid some common mistakes.



## The Purpose

One of the most outstanding challenge of developing NEO Dapps/Smart Contract(SC) is the blockchain interaction. To test/debug the SC we have to face the following challenges, comparing with the conventional app development:

* **Chain Setup**: We need to setup a private chain or apply for some GAS from testnet, which sometimes kills hours. [neocompiler.io](neocompiler.io) is the best acceleration we found so far, with execellent development experience and the whole gamut of tools. [privatenet docker image](https://github.com/CityOfZion/neo-privatenet-docker) is another smart choice if you have to work offline (in subway or on airplane, a café with phishing WIFI). Another advantage of local privatenet is that you would never worry about your hands frozen during winter - Well, I'm in Canada and I guarantee it.
* **Deployment**: To test even a tiny value change, you have to re-deploy the smart contract. If the wallet is lack of GAS, we need to wait for a few blocks for GAS claiming even we are the richest in the world. Then we wait another couple of blocks for SC deployment. 
* **Logging/Debug**: No breakpoint, no step in, The only thing we lean on is `Runtime.Notify` supporting only byte array (Not sure `Runtime.Log`, I didn't try it). Then we do the type conversion to understand the symptom, feeling like some sort of encrypted telegram from a spy.
* **Logic Feature Restrictions**: NEO smart contract does not support all .Net framework syntax, frameworks, or language features (type conversion, byte munipulation, class method declaration, etc.). It causes runtime confusion since the compiler doesn't complain all of them. eg. a [Ternary issue](https://github.com/NeoResearch/learning-examples/blob/master/BadExamples.md) which blocked my friend's project a few days.



Through the team [Norchain](http://norchain.io/home/)'s experience during the [NEO Game Competition](http://neo.game/), we are trying to alleviate the above pains for C# Dapps. The essential ideas are:

1. During Development/Debug period, embed the SC code (by C#) into client, and invoke them via a mock RPC call. So we can direct put breakpoints into SC and debug them chainlessly. 
2. During Test/Deployment period, change a few references in the SC and client code (without logic change), then both can work in a chain powered envrionment.
3. Avoid the SC logic from using any restricted logic features as much as possible.

We'll talk about how to realise them in the following section.



## The Structure

C# client applications may have various compatability restrictions of .Net framework adaption. In this section, we'll take the world class game engine Unity for example, since we already implemented this part and tested working well with Norchain's game [CarryBattle](http://norchain.io/home/carrybattle.html). 

In order to realize the targets mentioned at the end of the previous section, let's consider a 3-layer structure demostrated in the following figure. 

![NeunityStructure](pics/Neunity.jpg)



### 1. Application Layer

Application layer is the top layer of in Neunity's structure, whereas the only layer requiring the developer's implementation. Application layer logic is categorized into two parts:

**Offchain Logic**: The logic that developers would't put into SC. eg. The UI, ViewController, etc.

**Contract Logic**: The core SC logic, including the asset managment, public algorithms, etc. 

Let's take a look at the following *contract logic* `CardGameSC.cs`. 

```csharp
/** ----------------------------
CardGameSC.cs
Check ./Neunity/Samples/SampleUnity/Assets/Scripts/CardGameSC.cs for the complete implementation
--------------------------------*/
using System;
using System.Numerics;
using Neunity.Adapters.Unity;	//Adapter Layer. Uncomment this line to use this class in Client
//using Neunity.Adapters.NEO;	//Adapter Layer. Uncomment this line to use this class in SC
using Neunity.Tools;		//Tool Layer. Storage, Serialization, etc.

namespace Neunity.CardGame
{
    public class CardGame : SmartContract
    {
        //... Some Other logic ...
        
        // The definition of class Card
        public class Card
        {
            public BigInteger type;   
            public byte[] lvls;
            public BigInteger score;
            public string name;
        }
        
        // Customized Serialization for Card.
        public static byte[] Card2Bytes(Card card) => SD.JoinSegs2Seg(
            //... Neunity.Tools.SD manages Serialization/Deserialization.
        );

        // Customized Deserialization for Card
        
        public static Card Bytes2Card(byte[] data) => new Card
        {
        	// ... Neunity.Adapter.Op manages type conversation for different platforms 
        };

        // The Logic of merging two cards
        public static Byte[] CardMerge(byte[] cardData1, byte[] cardData2, string name)
        {
            Card card1 = Bytes2Card(cardData1);
            Card card2 = Bytes2Card(cardData2);
            Card card = new Card{
            	type = card1.type,
            	lvls = card2.lvls,
            	name = name,
            	birthBlock = Blockchain.GetHeight()
            };
            byte[] newCardData = Card2Bytes(card);
            
            //Neunity.Tools.IO manages the interaction of storage for different platforms.
            IO.SetStorageWithKeyPath(newCardData, "card", name);
            return newCardData;
        }

        //Main Entrance
        public static Object Main(string operation, params object[] args)
        {
            if (operation == "cardMerge")
            {
                byte[] cardData1 = (byte[])args[0];
                byte[] cardData2 = (byte[])args[1];
                string name = (string)args[2];

                return CardMerge(cardData1,cardData2,name);
            }
            if (operation == "getCard")
            {   //Used Internally Only
                byte[] cardData = (byte[])args[0];
                return Bytes2Card(cardData);
            }
            //... Other operations
            return false;
        }
    }
}
```

Then we directly embed `CardGameSC.cs` into Unity project, as shown in following screenshot.   

![UnitySample](pics/UnitySample.jpg)

We also have the *offchain logic* `ManagerCards.cs` which is responsible for managing the card related operations. It calls `CardGameSC.cs` functions in a few places:

```cs
/** ------------------------------------
ManagerCards.cs
Check ./Neunity/Samples/SampleUnity/Assets/Scripts/ManagerCards.cs for the complete implementation
----------------------------------------*/
using UnityEngine;
using Neunity.SomeCardGame;

public class ManagerCards : MonoBehaviour {
	//... Other logic above
    //
    void MergeCards(){
        Debug.Log("Merge Begin");
        CardGame.Card card1 = new CardGame.Card()	//1. Using the class Card from CardGameSC.cs
        {
            //... card1's properties
        };

        CardGame.Card card2 = new CardGame.Card()
        {
            //... card1's properties
        };

        byte[] b1 = CardGame.Card2Bytes(card1);	//2. Calling CardGameSC.cs method
        byte[] b2 = CardGame.Card2Bytes(card2);	//2. Calling CardGameSC.cs method

        //
        byte[] bn = (byte[])CardGame.Main("cardMerge", b1, b2, "NovaCard");	//3. Calling CardGameSC.cs Entrance Function "Main"

        CardGame.Card cardNew = CardGame.Bytes2Card(bn);	//2. Calling CardGameSC.cs method

        Debug.Log("New card's name:\t" + cardNew.name);

        string lvlString = "";
        for (int i = 0; i < cardNew.lvls.Length; i++){
            lvlString += cardNew.lvls[i].ToString() + ",";
        }
        Debug.Log("New card's lvls:\t" + lvlString);
        Debug.Log("New card's birthBlock:\t" + cardNew.birthBlock);
        Debug.Log("New card's type:\t" + cardNew.type);
    }
}
```



We've accomplished two important things !! First, via embedding `CardGameSC.cs` into client project, we shared the class definitions and the majority logic between SC and client.  Secondly, we can now direct debug the SC logic with testcases and breakpoints now.

In the Test/Deployment period, we need to:

1. In  `CardGameSC.cs` , change `using Neunity.Adapters.Unity;` to `using Neunity.Adapters.NEO;`
2. Combine `CardGameSC.cs`  with the file `./Neunity/Adapters/NEO.cs` and all classes in  `./Neunity/Tools` , then compile to .avm.

No logic chance or adaption required.



### 2. Tool Layer

Neunity also provides a series of tools to help C# developers to improve the experience of developing NEO smart contract, and avoid some common mistakes. The tool logic is based on Adapter Layer and do not require any change between smart contract and C# client. 

We are continuouly adding models into tool layer. For now, we have two.

#### 2.1 Storage (Neunity.Tools.IO)

There are two limitations in NEO's official Storage tools:

1. Essentially, NEO storage supports one layer key-value. The class `StorageMap` supports two layers, but still not sufficient for complex tree-like hierachy.

2. The method of `Storage.Put` does not distinguish unchanged value or empty values. The following code would still cost 1GAS which is actually unnecessary.

   ```cs
   //---- Unchanged Put ----- 
   byte[] origValue = Storage.Get(Storage.CurrentContext, "key1");	//Cost 0.1GAS
   Storage.Put(Storage.CurrentContext, "key1",origValue);	//Cost 1GAS, which can be avoid
   //---- Empty Put ----- 
   byte[] emptyBytes = new byte[0];
   Storage.Put(Storage.CurrentContext, "key2",emptyBytes);	//Cost 1GAS, which is equivalent with Delete (0.1GAS)
   ```

To resolve the above mentioned issues, `Neunity.Tools.IO` supports URI-like key structure:

```cs
/** --------------------------------
SampleCode: URI-like key structure
------------------------------------*/
using Neunity.Tools;  

/**
warID = 1000232
warParty = 2;
position = 3;
*/
public static byte WriteCardToWar(byte[] cardInfo, BigInteger warID, int warParty, int position){
    //The Key = "war/1000232/2/3"
    IO.SetStorageWithKeyPath(cardInfo, "war",warID,warParty,position);
}


```

`Neunity.Tools.IO`  also avoids the unchanged and empty value problems:

```cs
/** ------------------------
SampleCode: Avoid unchanged and empty value problems
------------------------*/
using Neunity.Tools;  

public static byte WriteCardToWar(byte[] cardInfo, BigInteger warID, int warParty, int position){
    //---- Unchanged Put ----- 
    byte[] val = IO.GetStorageWithKey("key");	//0.1GAS
    IO.SetStorageWithKey("key",val);	//0.1GAS
    //---- Empty Put ----- 
    byte[] emptyBytes = new byte[0];
    IO.SetStorageWithKey("key",emptyBytes);	//0.1GAS
}


```



#### 2.2 Serialization (Neunity.Tools.SD)

NEO officially supports a convenient serialization method in Opcode, which also consumes less GAS. But not sufficient in complex cases. The following table compares the advantages/disadvantages of official and Neunity.Tools.SD

|               | Neunity.Tools.SD                                        | [Serializable] (Official)                       |
| ------------- | ------------------------------------------------------- | ----------------------------------------------- |
| Flexibility   | Can choose only the necessary fields for S/D            | Once stuct defined, all fields are put into S/D |
| Storage Size  | Dynamic Prefix. Saves about 20% (In CarryBattle's case) | Fixed Prefix.                                   |
| Compatibility | Compatable with Unity                                   | Not compatable with Unity                       |
| Array Support | Yes                                                     | No                                              |
| GAS           | High                                                    | Low with OpCode support                         |

We would suggest to use `Neunity.Tools.SD` for big and complex structs, while use official implementation for simple ones. The following example shows how it works:

```cs
/** ------------------------
SampleCode: Serialization

------------------------*/
using Neunity.Tools; 
using Neunity.Adapters.Unity; 

namespace Neunity.CardGame
{
    public class CardGame : SmartContract
    {
    	//.... Other logic ...
    	
    	// The definition of class Card
        public class Card
        {
            public BigInteger type;   
            public byte[] lvls;
            public BigInteger birthBlock;
            public string name;
            // --- Non-Storage field ---
            public BigInteger age;    //We don't plan to put this field in blockchain storage
        }
        
        //Manage non-Storage fields
        public static BigInteger UpdateAge(Card card){
            card.age = Blockchain.GetHeight() - card.birthBlock;
            return card.age;
        }

        // Customized Serialization for Card.
        public static byte[] Card2Bytes(Card card) => SD.JoinSegs2Seg(
            SD.SegInt(card.type),
            SD.Seg(card.lvls),
            SD.SegInt(card.birthBlock),
            SD.SegString(card.name)
        );

        // Customized Deserialization for Card
        public static Card Bytes2Card(byte[] data) => new Card{
            type = Op.Bytes2BigInt(SD.DesegWithIdFromSeg(data,0) ),
            lvls = SD.DesegWithIdFromSeg(data,1),
            birthBlock = Op.Bytes2BigInt(SD.DesegWithIdFromSeg(data,2) ),
            name = Op.Bytes2String(SD.DesegWithIdFromSeg(data,3) ),
        };
    }
```

We'll put more material regarding to this feature, but now you can check  `Neunity/Tools/Serialization.cs` for more details.

### 3. Adapter Layer

Unlike the Application Layer and Tool Layer, Adapter Layer code shares only the method signatures rather than implementations between platforms.  This is due to different language features on different platforms. eg. conversions, concatenation and operators.

```cs
using System;
using System.Numerics;
using System.Text;

namespace Neunity.Adapters.Unity{
    public static class Op{
        public static byte[] BigInt2Bytes(BigInteger bigInteger) => bigInteger.ToByteArray();
        public static byte[] JoinTwoByteArray(byte[] ba1, byte[] ba2)
        {
            byte[] ret = new byte[ba1.Length + ba2.Length];
            Buffer.BlockCopy(ba1, 0, ret, 0, ba1.Length);
            Buffer.BlockCopy(ba2, 0, ret, ba1.Length, ba2.Length);
            return ret;
        }
        public static bool And(bool left, bool right) => left && right;
        
        public static byte[] SubBytes(byte[] data, int start, int length)
        {
            if (data.Length < start + length)
            {
                return new byte[0];
            }
            else
            {
                byte[] ret = new byte[length];
                Array.Copy(data, start, ret, 0, length);
                return ret;
            }

        }
    }
}
```

The implementation of same functions for NEO are:

```cs
using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using Helper = Neo.SmartContract.Framework.Helper;
using System.Numerics;

namespace Neunity.Adapters.NEO{
    public static byte[] BigInt2Bytes(BigInteger bigInteger)
    {
        if (bigInteger == 0) return new byte[1] { 0 };
        return bigInteger.AsByteArray();
    }
    
    public static byte[] JoinTwoByteArray(byte[] ba1, byte[] ba2) => ba1.Concat(ba2);
    
    public static bool And(bool left, bool right){
        if (left) return right;
        return false;
    }
   
    public static byte[] SubBytes(byte[] data, int start, int length) => Helper.Range(data, start, length);
}
```

We need to . Hopefully with the community's help, we can keep adding more functions there.

The future plan of `Neunity.Adapters` includes:

1. More test cases for every function from different platforms to ensure their return values are equal. That's the meaning of "Adapters".
2. Add more functions to mock blockchain in `Neunity.Adapters.Unity`

**Please provide more failure testcases and suggest more functions and tactics. With the community's help, we can make it more powerful.** 



Contact: @dprat0821 on Github and Discord. Thanks