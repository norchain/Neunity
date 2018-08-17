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
    // <Card> = [S<id>,S<name>,S<birthBlock>,S<level>,S<ownerId>,S<isFighting#1>]
    //
    public class Card
    {
        public byte[] id;
        public string name;
        public BigInteger birthBlock;
        public BigInteger level;
        public byte[] ownerId;
        public bool isLive;
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
        // NuSD: <Card> = [S<id>,S<name>,S<birthBlock>,S<level>,S<ownerId>,S<isFighting#1>]
        public static byte[] Card2Bytes(Card card){
            if (card == null){
                return Op.Void;
            }else{
                return NuSD.Seg(card.id)
                           .AddSegStr(card.name)
                           .AddSegInt(card.birthBlock)
                           .AddSegInt(card.level)
                           .AddSeg(card.ownerId)
                           .AddSegBool(card.isLive);
            }

        } 


        // Customized Deserialization for Card
        // The class Neunity.Adapter.Op manages type conversation for different platforms 
        public static Card Bytes2Card(byte[] data){
            if (data.SizeTable() <5)    //Invalid input data
            {
                return null;
            }
            else{
                return new Card
                {
                    id = data.SplitTbl(0),
                    name = data.SplitTblStr(1),
                    birthBlock = data.SplitTblInt(2),
                    level = data.SplitSegInt(3),
                    ownerId = data.SplitTbl(4),
                    isLive = data.SplitTblBool(5)
                };
            }
        } 

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

        public static User Bytes2User(byte[] data){
            if (data.Length == 0) return null;
            User user = new User
            {
                id = data.SplitTbl(0),
            };
            byte[] cardIds = data.SplitTbl(1);
            int numCards = cardIds.SizeTable();
            user.cards = new Card[numCards];

            for (int i = 0; i < numCards; i++)
            {
                user.cards[i] = ReadCard(cardIds.SplitTbl(i));
            }
            return user;
        }
              
        #endregion

        #region NuIO: Neunity Storage
        public static byte SaveCard(Card card){
            //KeyPath of card: /c/{card.Id}
            return NuIO.SetStorageWithKeyPath(Card2Bytes(card), "c", Op.Bytes2String(card.id));
        }

        public static Card ReadCard(byte[] id){
            byte[] data = NuIO.GetStorageWithKeyPath("c", Op.Bytes2String(id));
            return Bytes2Card(data);
        }


        public static byte SaveUser(User user){
            //KeyPath of card: /u/{user.Id}
            return NuIO.SetStorageWithKeyPath(User2Bytes(user), "u", Op.Bytes2String(user.id));
        }

        public static User ReadUser(byte[] id){
            return Bytes2User(NuIO.GetStorageWithKeyPath("u", Op.Bytes2String(id)));
        }

        #endregion


        #region NuTP: Neunity Transfer Protocol

        /**Generate a random card*/
        public static byte[] RandomCard(byte[] ownerId, byte[] cardId, string name){
            Card cardOrig = ReadCard(cardId);

            if(cardOrig == null){
                byte[] newLvData = Hash256(Op.JoinTwoByteArray(ownerId, Op.BigInt2Bytes(Blockchain.GetHeight())));

                                           
                Card card = new Card
                {
                    id = cardId,
                    name = name,
                    birthBlock = Blockchain.GetHeight(),
                    level = Op.Bytes2BigInt(newLvData) % 3,
                    ownerId = ownerId,
                    isLive =true
                };
                SaveCard(card);
                return NuTP.RespDataSucWithBody(Card2Bytes(card));
            }
            else{
                return NuTP.RespDataWithCode(Error.Dom, Error.AlreadyExist);
            }

        }

        /** The Logic of merging the cards. The rule is: 
         * 0. Two cards has to share the same owner
         * 1. Card1 has higher level than Card2
         * 2. Card1 and Card2 should have the same 
         */
        public static byte[] CardMerge(byte[] cardID1, byte[] cardID2, string name)
        {
            Card card1 = ReadCard(cardID1);
            if(card1 == null){
                return NuTP.RespDataWithDetail(Error.Dom,Error.NoExist,Op.Bytes2String(cardID1),Op.Void);
            }
            Card card2 = ReadCard(cardID2);
            if (card2 == null)
            {
                return NuTP.RespDataWithDetail(Error.Dom, Error.NoExist, Op.Bytes2String(cardID2), Op.Void);
            }

            if ( Op.Bytes2BigInt(card1.ownerId) != Op.Bytes2BigInt(card2.ownerId)){
                return NuTP.RespDataWithCode(Error.Dom, Error.DiffOwner);
            }
            if (card1.level >= card2.level){
                BigInteger newLevel = card1.level + card2.level;
                Card newCard = new Card
                {
                    id = Hash256(Op.JoinTwoByteArray(card1.id, card2.id)),
                    name = name,
                    level = newLevel,
                    birthBlock = Blockchain.GetHeight(),
                    ownerId = card1.ownerId,
                    isLive = true
                };

                SaveCard(newCard);

                card1.isLive = false;
                card2.isLive = false;
                SaveCard(card1);
                SaveCard(card2);

                return NuTP.RespDataSucWithBody(Card2Bytes(newCard));
            }
            else{
                return NuTP.RespDataWithCode(Error.Dom, Error.LvInvalid);
            }

        }
        #endregion
        //Main Entrance
        public static Object Main(string operation, params object[] args)
        {
            
            if (operation == "randomCard")
            {   
                byte[] ownerId = (byte[])args[0];
                byte[] cardId = (byte[])args[1];
                string cardName = (string)args[2];

                return RandomCard(ownerId, cardId, cardName);
            }

            if (operation == "getCard")
            {   //Used Internally Only
                byte[] cardId = (byte[])args[0];
                Card card = ReadCard(cardId);
                byte[] cardData = Card2Bytes(card);
                return NuTP.RespDataSucWithBody(cardData);
            }

            if (operation == "cardMerge")
            {
                byte[] cardID1 = (byte[])args[0];
                byte[] cardID2 = (byte[])args[1];
                string name = (string)args[2];

                return CardMerge(cardID1,cardID2,name);
            }


            //... Other operations

            return NuTP.RespDataWithCode(NuTP.SysDom,NuTP.Code.NotFound);
        }


    }

    public static class Error
    {
        public static readonly BigInteger Dom = 1;

        public static readonly BigInteger LvInvalid = 10 ;
        public static readonly BigInteger DiffOwner = 20;
        public static readonly BigInteger NoExist = 30;
        public static readonly BigInteger AlreadyExist = 40;
    }

}
