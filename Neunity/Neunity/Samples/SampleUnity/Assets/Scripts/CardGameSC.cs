using System;
using System.Numerics;

using Neunity.Adapters.Unity;
using Neunity.Tools;

namespace Neunity.SomeCardGame
{
    public class CardGame : SmartContract
    {
        public static class TypeArmy
        {
            public const byte Infantry = 0;
            public const byte Archer = 1;
            public const byte Cavalry = 2;
            public const byte Invalid = 4;
        }

        public class Card
        {
            public BigInteger type;   //TypeArmy
            public byte[] lvls;    // Range: 0 - 255
            public BigInteger score;
            public string name;
        }

        //Customized Serialization for Card
        public static byte[] Card2Bytes(Card card) => SD.JoinSegs2Seg(
            SD.SegInt(card.type),
            SD.Seg(card.lvls),
            SD.SegInt(card.score),
            SD.SegString(card.name)
        );

        //... Customized Deserialization for Card
        public static Card Bytes2Card(byte[] data) => new Card
        {
            type = Op.Bytes2BigInt(SD.DesegWithIdFromSeg(data,0) ),
            lvls = SD.DesegWithIdFromSeg(data,1),
            score = Op.Bytes2BigInt(SD.DesegWithIdFromSeg(data,2) ),
            name = Op.Bytes2String(SD.DesegWithIdFromSeg(data,3) )
        };

        // ... The Logic of the result of merging the cards
        public static Byte[] CardMerge(byte[] cardData1, byte[] cardData2, string name)
        {
            Card card1 = Bytes2Card(cardData1);
            Card card2 = Bytes2Card(cardData2);

            Card card = new Card();
            card.type = card1.type;
            card.lvls = card2.lvls;
            card.score = 0;
            card.name = name;

            return Card2Bytes(card);

        }

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
