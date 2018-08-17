using System;
using System.Numerics;
using System.Text;

using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Neunity.Adapters.Unity
{
    public class SmartContract
    {
        public static bool VerifySignature(byte[] signature, byte[] address)
        {
            return true;
        }
        protected static byte[] Hash256(byte[] orig)
        {
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




    public class StorageContext { }


    public static class NeUtil
    {
        public static string ByteToHex(this byte[] data)
        {
            string hex = BitConverter.ToString(data).Replace("-", "").ToLower();
            return hex;
        }

        public static string ToHexString(this IEnumerable<byte> value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in value)
                sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }

        public static byte[] HexToBytes(this string value)
        {
            if (value == null || value.Length == 0)
                return new byte[0];
            if (value.Length % 2 == 1)
                throw new FormatException();

            if (value.StartsWith("0x"))
            {
                value = value.Substring(2);
            }

            byte[] result = new byte[value.Length / 2];
            for (int i = 0; i < result.Length; i++)
                result[i] = byte.Parse(value.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);
            return result;
        }
    }
}
