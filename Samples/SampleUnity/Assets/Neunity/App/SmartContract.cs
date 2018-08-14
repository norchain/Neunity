using System;
using System.Numerics;
using Neunity.Tools;

#if NEOSC
using Neunity.Adapters.NEO;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
#else
using Neunity.Adapters.Unity;
#endif


namespace Neunity.App
{


    // 
    // NuSD Definition of Card: 
    // <Card> = [S<id>,S<name>,S<birthBlock>,S<ownerId>,S<isFighting#1>]
    //
    public class Card
    {
        public byte[] id;
        public string name;
        public BigInteger birthBlock;
        public byte[] ownerId;
        public bool isFighting;
        // --- Non-Storage field ---
        public BigInteger age;    //We don't plan to put it in blockchain storage
    }

    // 
    // NuSD Definition of Card: 
    // <User> = [S<id>,S[S<cardIds>*]]
    //
    public class User{
        public byte[] id;
        public Card[] cards;
    }

    public class Contract:SmartContract
    {
        private const int init_score = 100;

        public static BigInteger UpdateAge(Card card)
        {
            card.age = Blockchain.GetHeight() - card.birthBlock;
            return card.age;
        }

        #region NuSD: Neunity Serialization

        // Customized Serialization for Card.
        // The Class Neunity.Tools.NuSD Manages Serialization/Deserialization.

        public static byte[] Card2Bytes(Card card) => NuSD
            .Seg(card.id)
            .AddSegStr(card.name)
            .AddSegInt(card.birthBlock)
            .AddSegBool(card.isFighting);


        // Customized Deserialization for Card
		// The class Neunity.Adapter.Op manages type conversation for different platforms 
        public static Card Bytes2Card(byte[] data) => new Card
        {
            id = data.SplitTbl(0),
            name = data.SplitTblStr(1),
            birthBlock = data.SplitTblInt(2),
            isFighting = data.SplitTblBool(3)
        };

        public static byte[] User2Bytes(User user){
            byte[] dataCardIds = Op.Void;
            if (user.cards.Length > 0)
            {
                for (int i = 0; i < user.cards.Length; i++)
                {
                    //no cards data
                    dataCardIds = dataCardIds.AddSeg(user.cards[i].id);
                }
            }

            return NuSD.Seg(user.id).AddSeg(dataCardIds);

        }

        public static Bytes2User(byte[] data){
            
        }
              
        #endregion

        #region NuIO: Neunity Storage
        public static byte SaveCard(Card card){
            //KeyPath of card: /card/{card.Id}
            return NuIO.SetStorageWithKeyPath(Card2Bytes(card), "card", Op.Bytes2String(card.id));
        }

        public static Card ReadCard(byte[] id){
            byte[] data = NuIO.GetStorageWithKeyPath("card", Op.Bytes2String(id));
            return Bytes2Card(data);
        }

        public static byte SaveUser(User user){
            //KeyPath of card: /card/{card.Id}
            return NuIO.SetStorageWithKeyPath(Card2Bytes(card), "card", Op.Bytes2String(card.id));
        }
        #endregion

        // The Logic of merging the cards
        public static NuTP.Response CardMerge(byte[] cardData1, byte[] cardData2, string name)
        {
            Card card1 = Bytes2Card(cardData1);
            Card card2 = Bytes2Card(cardData2);

			Card card = new Card
			{
                id = Hash256(Op.JoinTwoByteArray(card1.id,card2.id)),
				name = name,
				birthBlock = Blockchain.GetHeight(),
                isFighting = false
			};

            SaveCard(card);
			


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
