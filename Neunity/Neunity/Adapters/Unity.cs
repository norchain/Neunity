using System;
using System.Numerics;
using System.Text;
//using UnityEngine;
using System.Globalization;

namespace Neunity.Adapters.Unity
{
	public static class Op
    {
        

        public static BigInteger Bytes2BigInt(byte[] data) => new BigInteger(data);

		public static byte[] BigInt2Bytes(BigInteger bigInteger) => bigInteger.ToByteArray();


        public static byte[] String2Bytes(String str) => (str.Length == 0) ? (new byte[1] { 0 }) : Encoding.UTF8.GetBytes(str);


        public static String Bytes2String(byte[] data) => (data.Length == 0) ? "\0" : Encoding.UTF8.GetString(data);




		public static bool Bytes2Bool(byte[] data) => (data.Length == 0)? false: (data[0]!=0);

		public static byte[] Bool2Bytes(bool val) => val ? (new byte[1] { 1 }) : new byte[1] { 0 };

        public static byte[] HexToBytes(this string hexString) {
            if(hexString == null || hexString.Length == 0)
                return new byte[0];
            if(hexString.Length % 2 == 1)
                throw new FormatException();

            if(hexString.StartsWith("0x")) {
                hexString = hexString.Substring(2);
            }

            byte[] result = new byte[hexString.Length / 2];
            for(int i = 0; i < result.Length; i++)
                result[i] = byte.Parse(hexString.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);
            return result;
        }

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

        public static byte[] JoinTwoByteArray(byte[] ba1, byte[] ba2)
        {
            byte[] ret = new byte[ba1.Length + ba2.Length];
            Buffer.BlockCopy(ba1, 0, ret, 0, ba1.Length);
            Buffer.BlockCopy(ba2, 0, ret, ba1.Length, ba2.Length);
            return ret;
        }

        public static byte[] JoinByteArray(params byte[][] args)
        {
            if (args.Length == 0) { return new byte[0]; }
            else
            {
                byte[] r = args[0];
                for (int i = 1; i < args.Length; i++)
                {
                    r = JoinTwoByteArray(r, args[i]);
                }

                return r;
            }
        }

		public static bool And(bool left, bool right) => left && right;


		public static bool Or(bool left, bool right) => left || right;

        //public static byte[] Byte2ByteArray(byte b) => new byte[1] { b };
        
		public static void Log(string str)
        {
			//TBD Debug.Log(str);
        }


    }
    
    

    public class SmartContract { }

    public static class Extensions
    {
		public static byte[] ToScriptHash(this string address) => Op.String2Bytes(address);
    }

    public enum TriggerType : byte
    {
        Verification,
        VerificationR,
        Application = 16,
        ApplicationR
    }

    public static class Runtime
    {
        public static uint Time = 0;
        public static TriggerType Trigger = TriggerType.Application;
        public static bool CheckWitness(byte[] hashOrPubkey) => true;

        public static void Log(string message) { }
        public static void Notify(params object[] state) { }
    }

	public static class Blockchain{
		public static uint GetHeight(){
			return 10000;
		}
	}

    public class StorageContext { }

    public class Storage
    {
        public static StorageContext CurrentContext = new StorageContext();
		public static byte[] Get(StorageContext context, string key) => Get(context, Op.String2Bytes(key));


        public static void Put(StorageContext context, byte[] key, BigInteger value)
        {
			Put(context, key, Op.BigInt2Bytes(value));
        }
        public static void Put(StorageContext context, byte[] key, string value)
        {
			Put(context, key, Op.String2Bytes(value));
        }

        public static void Put(StorageContext context, string key, byte[] value)
        {
			Put(context, Op.String2Bytes(key), value);
        }
        public static void Put(StorageContext context, string key, BigInteger value)
        {
			Put(context, Op.String2Bytes(key), value);
        }
        public static void Put(StorageContext context, string key, string value)
        {
			Put(context, Op.String2Bytes(key), value);
        }

        public static void Delete(StorageContext context, string key)
        {
			Delete(context, Op.String2Bytes(key));
        }


        public static byte[] Get(StorageContext context, byte[] key)
        {
            //TBD
            return new byte[0];
        }

        public static void Put(StorageContext context, byte[] key, byte[] value)
        {
            //TBD
        }

        public static void Delete(StorageContext context, byte[] key)
        {
            //TBD
        }
    }
}
