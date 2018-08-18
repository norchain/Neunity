using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using Helper = Neo.SmartContract.Framework.Helper;
using System.Numerics;



namespace Neunity.Adapters.NEO
{
	public static class Op
    {
        public static byte[] Void() => new byte[0];

        public static BigInteger Bytes2BigInt(byte[] data) => data.AsBigInteger();

        public static byte[] BigInt2Bytes(BigInteger bigInteger) => bigInteger.AsByteArray();



        public static byte[] String2Bytes(String str) => str.AsByteArray();

		public static String Bytes2String(byte[] data) => data.AsString();

        public static String BigInt2String(BigInteger bigInteger) => bigInteger.AsByteArray().AsString();

		public static bool Bytes2Bool(byte[] data)
        {
            if (data.Length == 0) return false;
            return data[0] != 0;
        }

		public static byte[] Bool2Bytes(bool val)
        {
            if (val) return new byte[1] { 1 };
            return new byte[1] { 0 };
        }

        public static byte Bytes2Byte(byte[] data) => data[0];

        public static byte[] Byte2Bytes(byte b) => new byte[1] { b };

        public static byte Int2Byte(int i) => (byte)i;

        //public static int BigInt2Int(BigInteger i) => (int)i;

        public static byte[] SubBytes(byte[] data, int start, int length) => Helper.Range(data, start, length);


        public static byte[] JoinTwoByteArray(byte[] ba1, byte[] ba2) => ba1.Concat(ba2);

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

		public static bool And(bool left, bool right){
			if (left) return right;
			return false;
		}

		public static bool Or(bool left, bool right)
        {
			if (left) return true;
			return right;
        }


        public static void Log(params object[] ba){
			Runtime.Notify(ba);
		}


        public static void SetStoragePath(string path)
        {
            
        }

        //public static byte[] RandomSeed(){
        //    Blockchain.GetTransaction()
        //}
    }

}
