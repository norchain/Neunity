# Neunity

## The Purpose

One of the most outstanding challenge of developing NEO Dapps/Smart Contract(SC) is the blockchain interaction. To test/debug the SC we have to face the following facts, comparing with the conventional app development:

* **Chain Setup**: We need to setup a private chain or apply for some GAS from testnet, which sometimes kills hours. [neocompiler.io](neocompiler.io) is the best acceleration we found so far, with execellent development experience and the whole gamut of tools. [privatenet docker image](https://github.com/CityOfZion/neo-privatenet-docker) is another smart choice if you have to work offline (in subway or on airplane, a café with phishing WIFI). Another advantage of local privatenet is that you would never worry about your hands frozen during winter - Well, I'm in Canada and I guarantee it.
* **Deployment**: To test even a tiny value change, you have to re-deploy the smart contract. If the wallet is lack of GAS, we need to wait for a few blocks for GAS claiming even we are the richest in the world. Then we wait another couple of blocks for SC deployment. 
* **Logging/Debug**: No breakpoint, no step in, The only thing we lean on is `Runtime.Notify` supporting only byte array (Not sure `Runtime.Log`, I didn't try it). Then we do the type conversion to understand the symptom, feeling like some sort of encrypted telegram from a spy.
* **Language Feature Restrictions**: NEO smart contract does not support all the .Net framework syntax, frameworks, or language features (type conversion, byte munipulation, class method declaration, etc.). But the compiler fails to complain some of them, which causes runtime confusion. My friend once encountered an issue per Ternary which blocked the project a few days.

![NeunityStructure](pics/NeunityStructure.jpg)