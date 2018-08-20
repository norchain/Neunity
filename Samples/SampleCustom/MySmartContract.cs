using System;
//using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Helper = Neo.SmartContract.Framework.Helper;
using Neunity.Tools;
using Neunity.Adapters.Unity;
using System.Numerics;
using System.ComponentModel;

namespace CustomSample.SC
{
    public class MySmartContract : SmartContract
    {
        public static string ContractName() => "Student contract";

        public class Student
        {
            public BigInteger id;
            public string name;
            public BigInteger balance;
        }

        public static byte[] Student2Bytes(Student stu) => SD.JoinSegs2Seg(
            SD.SegInt(stu.id),
            SD.SegString(stu.name),
            SD.SegInt(stu.balance)
        );

        public static Student Bytes2Student(byte[] data) => new Student
        {
            id = Op.Bytes2BigInt(SD.DesegWithIdFromSeg(data, 0)),
            name = Op.Bytes2String(SD.DesegWithIdFromSeg(data, 1)),
            balance = Op.Bytes2BigInt(SD.DesegWithIdFromSeg(data, 2))
        };

        //Main Entrance
        public static Object Main(string operation, params object[] args)
        {
            if (operation == "getName")
            {
                return ContractName();
            }
            if (args.Length > 0)
            {
                if (operation == "create")
                {
                    BigInteger id = (BigInteger)args[0];
                    string name = (string)args[1];
                    BigInteger balance = (BigInteger)args[2];
                    Student student = new Student
                    {
                        id = id,
                        name = name,
                        balance = balance
                    };
                    byte[] data = Student2Bytes(student);

                    IO.SetStorageWithKeyPath(data, "student", name);
                    return student;
                }

                if (operation == "balance")
                {
                    string name = (string)args[0];

                    return BalanceOf(name);
                }

                if (operation == "delete")
                {
                    string name = (string)args[0];

                    return Delete(name);
                }
            }
            //... Other operations
            return false;
        }
        private static BigInteger BalanceOf(string name)
        {
            byte[] stuBytes = IO.GetStorageWithKeyPath("student", name);

            if (stuBytes == null || stuBytes.Length == 0)
                return 0;

            Student stu = Bytes2Student(stuBytes);

            return stu.balance;
        }

        private static bool Delete(string name) {
            IO.SetStorageWithKeyPath(new byte[0], "student", name);
            return true;
        }
    }


}
