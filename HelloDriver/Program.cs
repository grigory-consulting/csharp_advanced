using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace HelloWorldDriverUser
{
    class Program
    {
        // Define the same IOCTL as in driver (must match!)
        const uint FILE_DEVICE_UNKNOWN = 0x00000022;
        const uint METHOD_BUFFERED = 0;
        const uint FILE_ANY_ACCESS = 0;

        // Same as in the driver
        static uint CTL_CODE(uint deviceType, uint function, uint method, uint access) =>
            ((deviceType << 16) | (access << 14) | (function << 2) | method);

        static readonly uint IOCTL_HELLO =
            CTL_CODE(FILE_DEVICE_UNKNOWN, 0x800, METHOD_BUFFERED, FILE_ANY_ACCESS);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern SafeFileHandle CreateFile(
            string lpFileName, uint dwDesiredAccess, uint dwShareMode,
            IntPtr lpSecurityAttributes, uint dwCreationDisposition,
            uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            byte[] lpOutBuffer, // using byte[] for output
            uint nOutBufferSize,
            out uint lpBytesReturned,
            IntPtr lpOverlapped);

        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint OPEN_EXISTING = 3;

        static void Main(string[] args)
        {
            // Note: must match the symbolic link in driver
            var devicePath = @"\\.\HelloWorldDevice";
            Console.WriteLine($"Opening {devicePath}...");

            using (var handle = CreateFile(
                devicePath, GENERIC_READ | GENERIC_WRITE, 0,
                IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero))
            {
                if (handle.IsInvalid)
                {
                    Console.WriteLine("Could not open device. Run as administrator?");
                    return;
                }

                byte[] outputBuffer = new byte[256];
                uint returned;

                bool result = DeviceIoControl(
                    handle,
                    IOCTL_HELLO,
                    IntPtr.Zero, 0,           // No input buffer for this example
                    outputBuffer, (uint)outputBuffer.Length,
                    out returned,
                    IntPtr.Zero);

                if (result && returned > 0)
                {
                    string reply = System.Text.Encoding.ASCII.GetString(outputBuffer, 0, (int)returned);
                    Console.WriteLine("Got reply: " + reply);
                }
                else
                {
                    Console.WriteLine($"DeviceIoControl failed. LastError={Marshal.GetLastWin32Error()}");
                }
            }
        }
    }
}
