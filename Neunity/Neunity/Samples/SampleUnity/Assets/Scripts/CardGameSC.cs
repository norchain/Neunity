using System;
using System.Numerics;

using Neunity.Adapters.Unity;
using Neunity.Tools;

namespace Neunity.SomeCardGame
{
    public class CardGame : SmartContract
    {

		private const int init_score = 100;

        public static class TypeArmy
        {
            public const byte Infantry = 0;
            public const byte Archer = 1;
            public const byte Cavalry = 2;
            public const byte Invalid = 4;
        }

		// The definition of class Card
        public class Card
        {
            public BigInteger type;   
            public byte[] lvls;
            public BigInteger birthBlock;
            public string name;
			// --- Non-Storage field ---
			public BigInteger age;    //We don't plan to put it in blockchain storage
        }

		public static BigInteger UpdateAge(Card card){
			card.age = Blockchain.GetHeight() - card.birthBlock;
			return card.age;
		}

		// Customized Serialization for Card.
		// The Class Neunity.Tools.SD Manages Serialization/Deserialization.
        public static byte[] Card2Bytes(Card card) => SD.JoinSegs2Seg(
            SD.SegInt(card.type),
            SD.Seg(card.lvls),
			SD.SegInt(card.birthBlock),
            SD.SegString(card.name)
        );

        // Customized Deserialization for Card
		// The class Neunity.Adapter.Op manages type conversation for different platforms 
        public static Card Bytes2Card(byte[] data) => new Card
        {
            type = Op.Bytes2BigInt(SD.DesegWithIdFromSeg(data,0) ),
            lvls = SD.DesegWithIdFromSeg(data,1),
			birthBlock = Op.Bytes2BigInt(SD.DesegWithIdFromSeg(data,2) ),
            name = Op.Bytes2String(SD.DesegWithIdFromSeg(data,3) ),
        };
        
        // The Logic of merging the cards
        public static Byte[] CardMerge(byte[] cardData1, byte[] cardData2, string name)
        {
            Card card1 = Bytes2Card(cardData1);
            Card card2 = Bytes2Card(cardData2);

			Card card = new Card
			{
				type = card1.type,
				lvls = card2.lvls,
				name = name,
				birthBlock = Blockchain.GetHeight()
			};


			byte[] newCardData = Card2Bytes(card);

			//Neunity.Tools.IO manages the interaction of storage for different platforms.
			IO.SetStorageWithKeyPath(newCardData, "card/", name);
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
