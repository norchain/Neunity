using System;
using System.Numerics;
using System.Text;


namespace Neunity.Adapters.Unity
{
	public static class Conv
    {
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

        public static BigInteger ByteArray2BigInteger(byte[] data) => new BigInteger(data);

        public static byte[] BigInteger2ByteArray(BigInteger bigInteger) => bigInteger.ToByteArray();


        public static byte[] String2ByteArray(String str) => (str.Length == 0) ? (new byte[1] { 0 }) : Encoding.UTF8.GetBytes(str);


        public static String ByteArray2String(byte[] data) => (data.Length == 0) ? "\0" : Encoding.UTF8.GetString(data);


        public static byte[] SubByteArray(byte[] data, int start, int length)
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

        public static bool ByteArray2Bool(byte[] data)
        {
            if (data.Length == 0) return false;
            return data[0] != 0;
        }

        public static byte[] Bool2ByteArray(bool val)
        {
            if (val) return new byte[1] { 1 };
            return new byte[1] { 0 };
        }

        //public static byte[] Byte2ByteArray(byte b) => new byte[1] { b };


    }

    public class SmartContract { }

    public static class Extensions
    {
		public static byte[] ToScriptHash(this string address) => Conv.String2ByteArray(address);
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

    public class StorageContext { }

    public class Storage
    {
        public static StorageContext CurrentContext = new StorageContext();
		public static byte[] Get(StorageContext context, string key) => Get(context, Conv.String2ByteArray(key));


        public static void Put(StorageContext context, byte[] key, BigInteger value)
        {
			Put(context, key, Conv.BigInteger2ByteArray(value));
        }
        public static void Put(StorageContext context, byte[] key, string value)
        {
			Put(context, key, Conv.String2ByteArray(value));
        }

        public static void Put(StorageContext context, string key, byte[] value)
        {
			Put(context, Conv.String2ByteArray(key), value);
        }
        public static void Put(StorageContext context, string key, BigInteger value)
        {
			Put(context, Conv.String2ByteArray(key), value);
        }
        public static void Put(StorageContext context, string key, string value)
        {
			Put(context, Conv.String2ByteArray(key), value);
        }

        public static void Delete(StorageContext context, string key)
        {
			Delete(context, Conv.String2ByteArray(key));
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
