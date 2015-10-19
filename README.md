ThreesandUO
=====

[![AppVeyor Build Status](https://ci.appveyor.com/api/projects/status/4tjo91e4qotjtsgq?svg=true)](https://ci.appveyor.com/project/ms/runuo) [![Travis Build Status](https://travis-ci.org/runuo/runuo.svg)](https://travis-ci.org/runuo/runuo)

Required Client Files
----
UO Client Download Link:	[UOClassicSetup_7_0_15_1.exe](https://mega.nz/#!UUAiQKra!C33PPezZxNhxLWzCC9gTYeW-c5EkrQg00AE-SNXjdXI)

Razor:						[Razor Installer.exe](http://www.uogamers.com/razor/)



Server Build Commands
---

Typical Windows Build

PS C:\ThreesandUO> C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc /optimize /unsafe /t:exe /out:ThreesandUO.exe /win32icon:Server\runuo.ico /recurse:Server\\*.cs


Typical Linux Build (MONO)

~/ThreesandUO$ dmcs -optimize+ -unsafe -t:exe -out:ThreesandUO.exe -win32icon:Server/runuo.ico -nowarn:219,414 -d:MONO -recurse:Server/*.cs


zlib is required for certain functionality. Windows zlib builds are packaged with releases and can also be obtained separately here: https://github.com/msturgill/zlib/releases/latest

RunUO supports Intel's hardware random number generator (Secure Key, Bull Mountain, rdrand, etc). If rdrand32.dll/rdrand64.dll are present in the base directory and the hardware supports that functionality, it will be used automatically. You can find those libraries here: https://github.com/msturgill/rdrand/releases/latest



ThreesandUO is based on a fork of [RunUO](https://github.com/runuo/runuo)
