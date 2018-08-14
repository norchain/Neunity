
#define NuLocal

namespace Neunity.App{
    public static class NuSV
    {
        public static object Invoke(string endpoint, string operation, params object[] args)
        {
            #if NuLocal
            return Contract.Main(operation, args);
            #else
            //[TODO]: use NeoLux
            return new byte[0];
            #endif

        }
    }
}

