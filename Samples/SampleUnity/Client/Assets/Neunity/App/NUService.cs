
#define NuLocal

using Neunity.Tools;

namespace Neunity.App{
    public static class NuContract
    {
        public static object Invoke(string endpoint, string op, params object[] args)
        {
            #if NuLocal
            return Contract.Main(op, args);
            #else
            //[TODO]: use NeoLux
            return new byte[0];
            #endif

        }

        public static NuTP.Response InvokeWithResp(string endpoint,string operation, params object[] args)
        {
            #if NuLocal
            return NuTP.Bytes2Response((byte[])Contract.Main(operation, args));
            #else
                        //[TODO]: use NeoLux
                        return new byte[0];
            #endif

        }
    }
}

