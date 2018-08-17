#!/bin/bash

PathAdapter="/Neunity/Adapters"
PathTools="/Neunity/Tools"
PathApp="/Neunity/App"
PathContract="/Neunity/Contract"

source path.config

echo
echo "----------------------------------------"
echo "Updating Neunity..."
echo "[Source Path] ${NeunityPathSource}"
echo "[Client Destination Path] ${NeunityPathClientDest}"
echo "[SC Destination Path] ${NeunityPathSCDest}"
echo "----------------------------------------"
echo "Copying Adapters..."
test -d ${NeunityPathClientDest}${PathAdapter} || mkdir -p ${NeunityPathClientDest}${PathAdapter} 
cp $NeunityPathSource$PathAdapter/Unity.cs ${NeunityPathClientDest}${PathAdapter}
cp $NeunityPathSource$PathAdapter/Imitation.cs ${NeunityPathClientDest}${PathAdapter}
cp $NeunityPathSource$PathAdapter/SimpleJSON.cs ${NeunityPathClientDest}${PathAdapter}

echo "Copying Tools..."
test -d ${NeunityPathClientDest}${PathTools} || mkdir -p ${NeunityPathClientDest}${PathTools} && cp $NeunityPathSource$PathTools/*.cs ${NeunityPathClientDest}${PathTools}

echo "Copying Apps..."
test -d ${NeunityPathClientDest}${PathApp} || mkdir -p ${NeunityPathClientDest}${PathApp} && cp $NeunityPathSource$PathApp/*.cs ${NeunityPathClientDest}${PathApp}
echo
echo "DONE"
echo