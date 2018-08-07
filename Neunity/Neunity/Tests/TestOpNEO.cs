using System;

using System.Numerics;
using Neunity.Adapters;
using Neunity.Toolchain;

using Neunity.Adapters.Unity;
//using Neo.SmartContract.Framework;
//using Neo.SmartContract.Framework.Services.Neo;

namespace Neunity.Tests
{
	public class TestConvNEO: SmartContract
    {
		public static bool TestOp(){
			bool result = true;
            
			BigInteger integer = 15;
			byte b = Op.BigInt2Bytes(integer)[0];
			result = Op.And(result, b == 15);         

            BigInteger integer1 = 255;
			byte b1 = Op.BigInt2Bytes(integer1)[0];
			result = Op.And(result, b == 255); 
                     
            int i = 2;
            string s = "c";
            string o = s + i.ToString();
			result = Op.And(result, o == "c2");

			return result;
		}

		public static bool TestSub()
        {         
			bool result = true;
         
            byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] out1 = { 1, 2, 3 };

			result = Op.And(result, Op.SubBytes(data, 0, 3) == out1);

            byte[] out2 = { 4, 5, 6, 7, 8, 9 };
			result = Op.And(result, Op.SubBytes(data, 3, 6) == out2);

			return result;

        }



		public static void Main()
        {
			Runtime.Notify("TestOp",TestOp());
			Runtime.Notify("TestSub", TestSub());
        }
    }
}
