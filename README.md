Language : c#

Framework : .NET Core 8.0

Project : c# Class Library

목적 : PLC LS산전프로토콜(Ethernet)의 Header를 생성 해주는 클래스

기능 :
1. PLCCpuType, fenetPosition, invokeId, plcInfo, DataLength를 이용하여 LS산전프로토콜의 클라이언트Header를 생성
2. PLCCpuType, fenetPosition, invokeId, plcInfo, List<byte> 또는 byte[]형의 데이터 이용하여 LS산전프로토콜의 클라이언트Header를 생성하고 data와 함께 전체 데이터를 반환
