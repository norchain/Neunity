#!/bin/bash
# a="abc"
# b="${a} bcd"
# echo $a $b

NeunityPathSource="../../Neunity"
NeunityPathDest="./Client/Assets/Neunity"

PathAdapter="/Adapters"
PathTools="/Tools"
PathApp="/App"
PathContract="/Contract"


echo "Updating Neunity..."
echo "[Source Path] ${NeunityPathSource}"
echo "[Destination Path] ${NeunityPathDest}"

cp "${NeunityPathSource}${PathAdapter}/*.cs" "${NeunityPathDest}${PathAdapter}/"