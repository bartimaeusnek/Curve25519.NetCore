# Curve25519.NetCore
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT) [![nuget](https://img.shields.io/nuget/v/Curve25519.NetCore.Updated.svg)](https://www.nuget.org/packages/Curve25519.NetCore.Updated/)

### Updated version of [this project!](https://github.com/TimothyMeadows/Curve25519.NetCore)

An elliptic curve offering 128 bits of security and designed for use with the elliptic curve Diffie–Hellman (ECDH) key agreement scheme. It is one of the fastest ECC curves and is not covered by any known patents. This Updated version does not depend on [SecureRandom.NetCore](https://github.com/TimothyMeadows/SecureRandom.NetCore) anymore.

# Install

From a command prompt
```
dotnet add package Curve25519.NetCore.Updated
```

```
Install-Package Curve25519.NetCore.Updated
```

You can also search for package via your nuget ui / website:

https://www.nuget.org/packages/Curve25519.NetCore.Updated/

# Examples

```csharp
var curve25519 = new Curve25519();
var alicePrivate = curve25519.CreateRandomPrivateKey();
var alicePublic = curve25519.GetPublicKey(alicePrivate);

var bobPrivate = curve25519.CreateRandomPrivateKey();
var bobPublic = curve25519.GetPublicKey(bobPrivate);

var aliceShared = curve25519.GetSharedSecret(alicePrivate, bobPublic);
var bobShared = curve25519.GetSharedSecret(bobPrivate, alicePublic);
var equal = aliceShared.SequenceEqual(bobShared);
```
