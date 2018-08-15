
#if NEOSC
using Neunity.Adapters.NEO;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
#else
using Neunity.Adapters.Unity;
#endif


namespace Neunity.Tools
{
    /** The Utilities */
    public static class NuIO{
        /** The result state of Storage.Put Operation */
        public static class State
        {
            public const byte Create = 0;
            public const byte Update = 1;
            public const byte Delete = 2;
            public const byte Unchanged = 3;
            public const byte Abort = 4;
            public const byte Invalid = 99;
        }

        /* ===========================================================
        * Storage functions are designed to support multi-segment keys
        * Eg. {Key = "seg1.seg2.seg3", Value = "someValue"}
        ==============================================================*/

        public static byte[] KeyPath(params string[] elements)
        {
            if (elements.Length == 0)
            {
                return new byte[0];
            }
            else
            {
				string r = "";
                for (int i = 0; i < elements.Length; i++)
                {
					r = r + "/" + elements[i];
                }
				return Op.String2Bytes(r);
            }
        }

        public static byte[] KeyPath(byte[] splitter, params string[] elements)
        {
            if (elements.Length == 0)
            {
                return new byte[0];
            }
            else
            {
                byte[] r = Op.String2Bytes(elements[0]);
                for (int i = 1; i < elements.Length; i++)
                {
                    r = Op.JoinByteArray(r, splitter, Op.String2Bytes(elements[i]));
                }
                return r;
            }
        }
        public static byte[] GetStorageWithKeyPath(params string[] elements) => GetStorageWithKey(KeyPath(elements));

        public static byte[] GetStorageWithKey(byte[] key) => Storage.Get(Storage.CurrentContext, key);

        public static byte[] GetStorageWithKey(string key) => Storage.Get(Storage.CurrentContext, key);


        public static byte SetStorageWithKeyPath(byte[] value, params string[] segments)
        {
            return SetStorageWithKey(KeyPath(segments), value);
        }

        public static byte SetStorageWithKey(string key, byte[] value)
        {
            // To avoid repeat spend of GAS caused by unchanged storage
            byte[] orig = GetStorageWithKey(key);
            if (orig == value) { return State.Unchanged; }

            if (value.Length == 0)
            {
                Storage.Delete(Storage.CurrentContext, key);
                return State.Delete;

            }
            else
            {
                Storage.Put(Storage.CurrentContext, key, value);
                return (orig.Length == 0) ? State.Create : State.Update;
            }
        }


        public static byte SetStorageWithKey(byte[] key, byte[] value)
        {
            if (value.Length == 0)
            {
                Storage.Delete(Storage.CurrentContext, key);
                return State.Delete;
            }
            else
            {
                byte[] orig = GetStorageWithKey(key);
                if (orig == value) { return State.Unchanged; }
                else
                {
                    Storage.Put(Storage.CurrentContext, key, value);
                    return (orig.Length == 0) ? State.Create : State.Update;
                }

            }
        }

    }
}
