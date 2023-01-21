using System.Linq;
using NUnit.Framework;

namespace Curve25519.NetCore.Examples;

internal class Curve25519Tests
{
    [DatapointSource]
    public bool[] NShouldUseTpm = {
        true,
        false
    };
    
    [Theory]
    public void TestEquality(bool tpm)
    {
        using var curve25519 = new Curve25519(tpm);
        
        var alicePrivate = curve25519.CreateRandomPrivateKey();
        var alicePublic  = curve25519.GetPublicKey(alicePrivate);
        
        Assert.False(alicePrivate.SequenceEqual(alicePublic));
        
        var bobPrivate = curve25519.CreateRandomPrivateKey();
        var bobPublic  = curve25519.GetPublicKey(bobPrivate);
        
        Assert.False(bobPrivate.SequenceEqual(bobPublic));
        Assert.False(alicePrivate.SequenceEqual(bobPrivate));
        
        var aliceShared = curve25519.GetSharedSecret(alicePrivate, bobPublic);
        var bobShared   = curve25519.GetSharedSecret(bobPrivate,   alicePublic);
        
        Assert.True(aliceShared.SequenceEqual(bobShared));
    }
}