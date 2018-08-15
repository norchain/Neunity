

rm ./SmartContract/SmartContract/Contract/*.cs
rm ./SmartContract/SmartContract/Neunity/*.*
cp ../../Neunity/Neunity/Adapters/NEO.cs ./SmartContract/SmartContract/Neunity/Adapters/NEO.cs
cp ../../Neunity/Neunity/Tools/*.cs ./SmartContract/SmartContract/Neunity/Tools/
cp ../../Neunity/Neunity/Tools/*.cs ./SmartContract/SmartContract/Neunity/Tools/


cd ./SmartContract/SmartContract/
dotnet msbuild
dotnet ~/Projects/BlockhainProjects/3rdParty/NEO/neo-compiler/neon/bin/Debug/netcoreapp2.0/neon.dll ~/Projects/BlockhainProjects/Competitions/NEOSample/SmartContract/SmartContract/bin/Debug/netstandard2.0/SmartContract.dll
cp ~/Projects/BlockhainProjects/Competitions/NEOSample/SmartContract/SmartContract/bin/Debug/netstandard2.0/SmartContract.avm ~/Desktop/