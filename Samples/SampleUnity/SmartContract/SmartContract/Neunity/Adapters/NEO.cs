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
        

		public static BigInteger Bytes2BigInt(byte[] data)
        {
            if (data.Length == 0) return 0;
            return data.AsBigInteger();
        }

		public static byte[] BigInt2Bytes(BigInteger bigInteger)
        {
            if (bigInteger == 0) return new byte[1] { 0 };
            return bigInteger.AsByteArray();
        }



		public static byte[] String2Bytes(String str)
        {
            if (str.Length == 0) return "\0".AsByteArray();

            return str.AsByteArray();
        }

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

        public static byte[] Byte2Bytes(byte b){
            return new byte[1] { b };
        }

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


		public static void Log(string str){
			Runtime.Notify(str);
		}

		//private static string keyRandom = "kr";

		//public static byte[] Random()
   //     {
			//byte[] ranSeed = Storage.Get(Storage.CurrentContext, keyRandom);
			//byte[] hash = Blockchain.GetHeader(Blockchain.GetHeight()).Hash;
			//byte[] ba = Sha256(ranSeed.Concat(hash));
			//Storage.Put(Storage.CurrentContext, keyRandom, (ranSeed.AsBigInteger() + 1).AsByteArray());
			//return ba;

        //}

    }

	public static class Funcs
    {

        public static byte[] Hash(byte[] origin)
        {
            return origin;
        }
    }
}
