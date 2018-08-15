
#!/bin/bash

NeunityPathSource="../../Neunity"
NeunityPathProj="./Client/Assets"
NeunityPathDestSLN="./SmartContract"
NeunityPathDest="./SmartContract/SmartContract"
NeonPath="~/Projects/BlockhainProjects/3rdParty/NEO/neo-compiler/neon/bin/Debug/netcoreapp2.0"
ContractDest="~/Desktop"

PathAdapter="/Neunity/Adapters"
PathTools="/Neunity/Tools"
PathApp="/Neunity/App"
PathContract="/Neunity/Contract"
PathDLL="/bin/Debug/netstandard2.0"

echo
echo "----------------------------------------"
echo "Compiling NEO SmartContract with Neunity..."
echo "[Source Path] ${NeunityPathSource}"
echo "[Destination Path] ${NeunityPathDest}"
echo "----------------------------------------"
echo "Copying Adapters..."
cp $NeunityPathSource$PathAdapter/NEO.cs ${NeunityPathDest}${PathAdapter}/NEO.cs
echo "Copying Tools..."
cp $NeunityPathProj$PathTools/*.cs ${NeunityPathDest}${PathTools}
echo "Copying Contract..."
cp $NeunityPathProj$PathContract/*.cs ${NeunityPathDest}${PathContract}

echo
echo "Compiling Contract..."
cd $NeunityPathDestSLN
dotnet msbuild
cd ..
dotnet ~/Projects/BlockhainProjects/3rdParty/NEO/neo-compiler/neon/bin/Debug/netcoreapp2.0/neon.dll ./SmartContract/SmartContract/bin/Debug/netstandard2.0/SmartContract.dll
cp ./SmartContract/SmartContract/bin/Debug/netstandard2.0/SmartContract.avm ~/Desktop
# dotnet ${NeonPath}/neon.dll "${NeunityPathDest}${PathDLL}/SmartContract.dll"
# dotnet neon.dll $NeunityPathDest$PathDLL
# cp $NeunityPathProj/$PathDLL/SmartContract.avm $ContractDest

echo
echo "DONE. Please check '${ContractDest}/SmartContract.avm'"
echo