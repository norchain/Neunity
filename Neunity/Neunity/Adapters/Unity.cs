using System;
using System.Numerics;
using System.Text;
//using UnityEngine;
using HiStudioGames.Utils;
using System.Globalization;
using System.Security.Cryptography;

namespace Neunity.Adapters.Unity
{
    public static class Op
    {

        public static byte[] Void = new byte[0];

        public static BigInteger Bytes2BigInt(byte[] data) => new BigInteger(data);

        public static byte[] BigInt2Bytes(BigInteger bigInteger) => bigInteger.ToByteArray();


        public static byte[] String2Bytes(String str) => (str.Length == 0) ? (new byte[1] { 0 }) : Encoding.UTF8.GetBytes(str);


        public static String Bytes2String(byte[] data) => (data.Length == 0) ? "\0" : Encoding.UTF8.GetString(data);

        public static String BigInt2String(BigInteger bigInteger) => bigInteger.ToString();


        public static bool Bytes2Bool(byte[] data) => (data.Length == 0) ? false : (data[0] != 0);

        public static byte[] Bool2Bytes(bool val) => val ? (new byte[1] { 1 }) : new byte[1] { 0 };

        public static byte Bytes2Byte(byte[] data) => data[0];

        public static byte[] Byte2Bytes(byte b) => new byte[1] { b };

        public static byte Int2Byte(int i) => (byte)i;

        //public static int BigInt2Int(BigInteger i) => (int)i;

        public static byte[] HexToBytes(this string hexString)
        {
            if (hexString == null || hexString.Length == 0)
                return new byte[0];
            if (hexString.Length % 2 == 1)
                throw new FormatException();

            if (hexString.StartsWith("0x"))
            {
                hexString = hexString.Substring(2);
            }

            byte[] result = new byte[hexString.Length / 2];
            for (int i = 0; i < result.Length; i++)
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
            //Debug.Log(str);
        }
    }

    public class SmartContract { 
		public static bool VerifySignature(byte[]signature, byte[] address){
			return true;
		}
        protected static byte[] Hash256(byte[] orig){
            HashAlgorithm sha = new SHA256Managed();
            return sha.ComputeHash(orig); 
        }
	}

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

    public static class Blockchain
    {
        public static uint GetHeight()
        {
            return 10000;
        }
    }
    /*
    public static class Funcs {
        static Random s_random = new Random();

        //public static bool VerifySign(byte[] signature, byte[] address) {
        //    return true;
        //}
        public static byte[] Hash(byte[] origin) {
            HashAlgorithm md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(origin);
        }

        public static byte[] SHA(byte[] orginal){
            HashAlgorithm sha = new SHA256Managed();
            return sha.ComputeHash(orginal);
        }

        public static void SetRandomSeed(byte[] seed) {
            int seedValue = 0;
            for(int i = 0; i < seed.Length; i++) {
                seedValue = seedValue + seed[i];
            }
            s_random = new Random(seedValue);
        }

        //maxValue is the excluded bound top value
        //public static BigInteger Random(BigInteger maxValue) {
        //    if(maxValue <= 0) {
        //        return new BigInteger(s_random.Next());
        //    } else {
        //        return new BigInteger(s_random.Next() % (int)maxValue);
        //    }
        //}

        //public static byte[] Rand(byte[] seed, int bytes)
        //{
        //    byte[] r = Hash(seed);
        //    if(bytes <= r.Length){
        //        return Op.SubBytes(r, 0, bytes);
        //    }
        //    else{
        //        return r;
        //    }
        //}


    }
*/


    public class StorageContext { }

    public class Storage
    {
		//SaveDataFile
		const string RD_FILE_NAME = "smart_data.json";

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
			string strKey = key.ToHexString();
            string strValue = DataStorage.GetString(RD_FILE_NAME, strKey);
            byte[] result = strValue.HexToBytes();
            return result;
        }

        public static void Put(StorageContext context, byte[] key, byte[] value)
        {
			string strKey = key.ByteToHex();
            string strValue = value.ByteToHex();
            DataStorage.SetString(RD_FILE_NAME, strKey, strValue);
        }

        public static void Delete(StorageContext context, byte[] key)
        {
			string strKey = key.ByteToHex();
            DataStorage.DeleteKey(RD_FILE_NAME, strKey);
        }
    }
}
