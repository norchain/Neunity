using System;
using NUnit.Framework;

using Neunity.Adapters.Unity;
using System.Numerics;

namespace Neunity.Test
{

    [TestFixture()]
    public class TestOpersion
    {
        [Test()]
        public void TestOp()
        {
            BigInteger integer = 15;
            byte b = integer.ToByteArray()[0];
            Assert.AreEqual(b, 15);

            BigInteger integer1 = 255;
            byte b1 = integer1.ToByteArray()[0];
            Assert.AreEqual(b1, 255);

            int i = 2;
            string s = "c";
            string o = s + i.ToString();
            Assert.AreEqual(o, "c2");
        }

        [Test()]
        public void TestDataSub()
        {
            byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] out1 = { 1, 2, 3 };

            Assert.AreEqual(Op.SubBytes(data, 0, 3), out1);

            byte[] out2 = { 4, 5, 6, 7, 8, 9 };
			Assert.AreEqual(Op.SubBytes(data, 3, 6), out2);


        }

        [Test()]
        public void TestSJoin()
        {
            byte[] data1 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            byte[] data2 = { 1, 2, 3, 4, 5, 6, 7, 8 };
            byte[] data3 = { 1, 2, 3, 4 };
            byte[] data4 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

            byte[] dataout = {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                1, 2, 3, 4, 5, 6, 7, 8,
                1, 2, 3, 4,
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10,11
            };

            byte[] datajoin = Op.JoinByteArray(data1, data2, data3, data4);

            Assert.AreEqual(dataout, datajoin);
        }

        [Test()]
        public void TestOpNull()
        {
            /**
                Default Values: 
                For the type BigInteger and String, if the value is empty (zero byte), the logic should convert them to the default value.
                Specifically, 
                -   the default value of bool is false
                -   the default value of BigInteger is 0
                -   the default value of String is "\0"

                To convert the default value back to byte[], the result would be "0x00" rather than null. 
            */
            byte[] nullBA = new byte[0];
            byte[] zBA = new byte[1] { 0x00 };
            //----- Bool -------
            bool b1 = false;
            Assert.AreEqual(b1, Op.Bytes2Bool(nullBA));
            Assert.AreEqual(Op.Bool2Bytes(b1), zBA);

            //----- BigInt -------
            BigInteger big = Op.Bytes2BigInt(nullBA);
            BigInteger bp1 = big + 1;

            Assert.AreEqual(big, new BigInteger(0));
            Assert.AreEqual(bp1, new BigInteger(1));
            Assert.AreEqual(Op.BigInt2Bytes(big), zBA);

            //----- String -------
            String str = Op.Bytes2String(nullBA);
            Assert.AreEqual(str, "\0");
            Assert.AreEqual(Op.String2Bytes(str), zBA);



        }

        [Test()]
        public void TestOp1()
        {
            BigInteger i1 = 1;
            BigInteger i2 = 12102159;
            bool b1 = true;
            bool b2 = false;
            string s1 = "";
            string s2 = "adfeav";

            byte[] bi1 = Op.BigInt2Bytes(i1);
			byte[] bi2 = Op.BigInt2Bytes(i2);
            byte[] bb1 = Op.Bool2Bytes(b1);
			byte[] bb2 = Op.Bool2Bytes(b2);
            byte[] bs1 = Op.String2Bytes(s1);
			byte[] bs2 = Op.String2Bytes(s2);

            Assert.AreNotEqual(bi1, bi2);
        }
        
    }
}
