using System;
using System.Numerics;
using System.Text;
using System.IO;
using UnityEngine;
using System.Globalization;
using System.Collections.Generic;

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


    public static class NeUtil {
        public static string ByteToHex(this byte[] data) {
            string hex = BitConverter.ToString(data).Replace("-", "").ToLower();
            return hex;
        }

        public static string ToHexString(this IEnumerable<byte> value) {
            StringBuilder sb = new StringBuilder();
            foreach(byte b in value)
                sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }

        public static byte[] HexToBytes(this string value) {
            if(value == null || value.Length == 0)
                return new byte[0];
            if(value.Length % 2 == 1)
                throw new FormatException();

            if(value.StartsWith("0x")) {
                value = value.Substring(2);
            }

            byte[] result = new byte[value.Length / 2];
            for(int i = 0; i < result.Length; i++)
                result[i] = byte.Parse(value.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);
            return result;
        }
    }

    public class Storage
    {
        const string RECORD_DATA_FILE = "smartcontract_data.jsn";
        static JSONNode s_jsonNode = null;


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

            string strKey = key.ByteToHex();
            if(s_jsonNode == null) {
                string strContent = LoadData();
                if(strContent.Length > 0) {
                    s_jsonNode = JSONNode.Parse(strContent);
                } else { 
                    s_jsonNode = JSONNode.Parse("{}");
                }
            }
            if(s_jsonNode[strKey] == null) {
                return new byte[0];
            } else {
                string strHexValue = s_jsonNode[strKey];
                return strHexValue.HexToBytes();
            }
        }

        public static void Put(StorageContext context, byte[] key, byte[] value)
        {
            string strKey = key.ByteToHex();
            string strValue = value.ByteToHex();

            if(s_jsonNode == null){
                s_jsonNode = JSONNode.Parse("{}");
            }
            s_jsonNode[strKey] = strValue;
            string strContent = s_jsonNode.ToPrettyString();
            SaveData(strContent);
        }

        public static void Delete(StorageContext context, byte[] key)
        {
            string strKey = key.ByteToHex();

            if(s_jsonNode == null) {
                s_jsonNode = JSONNode.Parse("{}");
                return;
            }
            s_jsonNode.Remove(strKey);
            string strContent = s_jsonNode.ToPrettyString();
            SaveData(strContent);
        }

        public static void SaveData(string strContent) {
            string filePath = Application.persistentDataPath + "/" + RECORD_DATA_FILE;
            StreamWriter streamWriter = File.CreateText(filePath);
            streamWriter.Write(strContent);
            streamWriter.Close();
        }

        public static string LoadData() {
            string filePath = Application.persistentDataPath + "/" + RECORD_DATA_FILE;
            if(File.Exists(filePath)) {
                StreamReader streamReader = File.OpenText(filePath);
                string strContent = streamReader.ReadToEnd();
                streamReader.Close();
                return strContent;
            } else {
                return "{}";
            }

        }
    }
}
