# Neunity: Storage Management for NEO SC

Neunity also provides a series of tools to help C# developers to improve the experience of developing NEO smart contract, and avoid some common mistakes. The tool logic is based on Adapter Layer and do not require any change between smart contract and C# client. 

We are continuouly adding models into tool layer. In this article, we are introducing the Storage Management framework: NuIO.

There are two limitations in NEO's official Storage tools:

1. Essentially, NEO storage supports one layer key-value. The class `StorageMap` supports two layers, but still not sufficient for complex tree-like hierachy.

2. The method of `Storage.Put` does not distinguish unchanged value or empty values. The following code would still cost 1GAS which is actually unnecessary.

   ```cs
   //---- Unchanged Put ----- 
   byte[] origValue = Storage.Get(Storage.CurrentContext, "key1");	//Cost 0.1GAS
   Storage.Put(Storage.CurrentContext, "key1",origValue);	//Cost 1GAS, which can be avoid
   //---- Empty Put ----- 
   byte[] emptyBytes = new byte[0];
   Storage.Put(Storage.CurrentContext, "key2",emptyBytes);	//Cost 1GAS, which is equivalent with Delete (0.1GAS)
   ```

To resolve the above mentioned issues, `Neunity.Tools.IO` supports URI-like key structure:

```cs
/** --------------------------------
SampleCode: URI-like key structure
------------------------------------*/
using Neunity.Tools;  

/**
warID = 1000232
warParty = 2;
position = 3;
*/
public static byte WriteCardToWar(byte[] cardInfo, BigInteger warID, int warParty, int position){
    //The Key = "war/1000232/2/3"
    IO.SetStorageWithKeyPath(cardInfo, "war",warID,warParty,position);
}


```

`Neunity.Tools.IO`  also avoids the unchanged and empty value problems:

```cs
/** ------------------------
SampleCode: Avoid unchanged and empty value problems
------------------------*/
using Neunity.Tools;  

public static byte WriteCardToWar(byte[] cardInfo, BigInteger warID, int warParty, int position){
    //---- Unchanged Put ----- 
    byte[] val = IO.GetStorageWithKey("key");	//0.1GAS
    IO.SetStorageWithKey("key",val);	//0.1GAS
    //---- Empty Put ----- 
    byte[] emptyBytes = new byte[0];
    IO.SetStorageWithKey("key",emptyBytes);	//0.1GAS
}


```

