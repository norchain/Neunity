using System;
using System.Numerics;
using Neunity.Tools;

#if NEOSC
using Neunity.Adapters.NEO;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
#else
using Neunity.Adapters.Unity;
#endif


namespace Neunity.App
{
    public class Contract:SmartContract
    {
        public static object Main(string operation, params object[] args) {
            
            return NuTP.RespDataSuccess();
        }
    }
}
