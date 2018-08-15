#!/bin/bash

NeunityPathSource="../../Neunity"
NeunityPathDest="./Client/Assets"

PathAdapter="/Neunity/Adapters"
PathTools="/Neunity/Tools"
PathApp="/Neunity/App"
PathContract="/Neunity/Contract"
echo
echo "----------------------------------------"
echo "Updating Neunity..."
echo "[Source Path] ${NeunityPathSource}"
echo "[Destination Path] ${NeunityPathDest}"
echo "----------------------------------------"
echo "Copying Adapters..."
cp $NeunityPathSource$PathAdapter/Unity.cs ${NeunityPathDest}${PathAdapter}/Unity.cs
cp $NeunityPathSource$PathAdapter/SimpleJSON.cs ${NeunityPathDest}${PathAdapter}/SimpleJSON.cs
echo "Copying Tools..."
cp $NeunityPathSource$PathTools/*.cs ${NeunityPathDest}${PathTools}
echo "Copying Apps..."
cp $NeunityPathSource$PathApp/*.cs ${NeunityPathDest}${PathApp}
echo
echo "DONE"
echo