using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Numerics;
using CustomSample.SC;

namespace CustomSample.Test
{
    public class TestHelper {
        public static MySmartContract.Student create(Int32 id, string name, Int32 balance) {
            BigInteger sid = (BigInteger)id;
            BigInteger sbalance = (BigInteger)balance;
            MySmartContract.Student stu = (MySmartContract.Student)MySmartContract.Main("create", sid, "liu", sbalance);

            return stu;
        }
    }
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCreate()
        {
            MySmartContract.Student stu = TestHelper.create(1, "liu", 100);

            Assert.AreEqual(1, stu.id);
            Assert.AreEqual("liu", stu.name);
            Assert.AreEqual(100, stu.balance);
        }

        [TestMethod]
        public void TestBalance() {
            // remove all records
            bool delete = (bool)MySmartContract.Main("delete", "liu");
            Assert.AreEqual(true, delete);

            // 0 balance
            BigInteger balance = (BigInteger)MySmartContract.Main("balance", "liu");
            Assert.AreEqual(0, balance);

            // create account
            MySmartContract.Student stu = TestHelper.create(1, "liu", 100);
            Assert.AreEqual(100, stu.balance);

            BigInteger balance2 = (BigInteger)MySmartContract.Main("balance", "liu");
            Assert.AreEqual(100, balance2);
        }

        [TestMethod]
        public void TestDelete() {
            MySmartContract.Student stu = TestHelper.create(1, "liu", 100);

            bool delete = (bool)MySmartContract.Main("delete", "liu");
            Assert.AreEqual(true, delete);

            // 0 balance
            BigInteger balance = (BigInteger)MySmartContract.Main("balance", "liu");
            Assert.AreEqual(0, balance);
        }
    }
}
