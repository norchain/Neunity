rm ./SmartContract/SmartContract/*.cs
cp ./NeoUnity/Assets/Scripts/SmartContract/*.cs ./SmartContract/SmartContract/
cd ./SmartContract/SmartContract/
dotnet msbuild
dotnet ~/Projects/BlockhainProjects/3rdParty/NEO/neo-compiler/neon/bin/Debug/netcoreapp2.0/neon.dll ~/Projects/BlockhainProjects/Competitions/NEOSample/SmartContract/SmartContract/bin/Debug/netstandard2.0/SmartContract.dll
cp ~/Projects/BlockhainProjects/Competitions/NEOSample/SmartContract/SmartContract/bin/Debug/netstandard2.0/SmartContract.avm ~/Desktop/