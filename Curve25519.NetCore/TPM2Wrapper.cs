/*
MIT License

Copyright (c) 2021-2023 bartimaeusnek

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
namespace Curve25519.NetCore
{
    using System;
    using Tpm2Lib;

    public class TPM2Wrapper : IDisposable
    {
        private readonly Tpm2Device _device;
        private readonly Tpm2 _tpm2;
        
        private readonly bool _canStirRandom;
        private readonly bool _canGetRandom;
        
        public TPM2Wrapper()
        {
            try
            {
                #if NET5_0_OR_GREATER
                _device = (OperatingSystem.IsLinux() ? (Tpm2Device) new LinuxTpmDevice() : OperatingSystem.IsWindows() ? (Tpm2Device)new TbsDevice() : throw new NotSupportedException());
                #else
                var os = Environment.OSVersion;
                var platform = os.Platform;
                switch (platform)
                {
                    case PlatformID.Unix:
                        _device = new LinuxTpmDevice();
                        break;
                    case PlatformID.Win32NT:
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.WinCE:
                        _device = new TbsDevice();
                        break;
                    case PlatformID.MacOSX:
                    case PlatformID.Xbox:
                    default:
                        throw new NotSupportedException();
                }
                #endif
                _device.Connect();
                _tpm2 = new Tpm2(_device);
                _tpm2.GetCapability(Cap.Commands, (uint)TpmCc.First, TpmCc.Last - TpmCc.First + 1, out var caps);
                foreach (TpmCc tmpcc in ((CcaArray) caps).commandAttributes)
                {
                    if (tmpcc == TpmCc.GetRandom)
                    {
                        _canGetRandom = true;
                    }
                    else if (tmpcc == TpmCc.StirRandom)
                    {
                        _canStirRandom = true;
                    }
                    if (_canGetRandom && _canStirRandom)
                        break;
                }
            }
            catch (Exception)
            {
                _canStirRandom = false;
                _canGetRandom = false;
            }
        }
        
        public bool TryGetRandom(byte[] random)
        {
            if (!_canStirRandom)
                return false;
            
            try
            {
                _tpm2.StirRandom(random);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public bool TryGetRandom(ushort requestedBytes, out byte[] random)
        {
            if (requestedBytes > 32)
                throw new ArgumentException("Maximum is 32 bytes!", nameof(requestedBytes));
            if (_canGetRandom)
            {
                random = _tpm2.GetRandom(requestedBytes);
                return random.Length == requestedBytes;
            }
            
            random = new byte[requestedBytes];
            return false;
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            _tpm2?.Dispose();
            _device?.Dispose();
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TPM2Wrapper()
        {
            Dispose(false);
        }
    }
}