# Neunity Serialization for NEO Smart Contract Development

NEO officially supports a convenient serialization method in Opcode, which also consumes less GAS. But not sufficient in complex cases. The following table compares the advantages/disadvantages of official and `Neunity.Tools.SD`

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