using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using Helper = Neo.SmartContract.Framework.Helper;
using System.Numerics;



namespace Neunity.Adapters.NEO
{
	public static class Conv
    {
        public static byte[] JoinTwoByteArray(byte[] ba1, byte[] ba2) => ba1.Concat(ba2);
        public static BigInteger ByteArray2BigInteger(byte[] data)
        {
            if (data.Length == 0) return 0;
            return data.AsBigInteger();
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

        public static byte[] BigInteger2ByteArray(BigInteger bigInteger)
        {
            if (bigInteger == 0) return new byte[1] { 0 };
            return bigInteger.AsByteArray();
        }
        public static byte[] String2ByteArray(String str)
        {
            if (str.Length == 0) return "\0".AsByteArray();

            return str.AsByteArray();
        }
        public static String ByteArray2String(byte[] data) => data.AsString();
        public static byte[] SubByteArray(byte[] data, int start, int length) => Helper.Range(data, start, length);


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
    }
}
