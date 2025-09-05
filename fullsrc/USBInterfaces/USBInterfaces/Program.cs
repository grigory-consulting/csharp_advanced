using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Enumerating USB devices ===");
        foreach (UsbRegistry reg in UsbDevice.AllDevices)
        {
            Console.WriteLine($"{reg.Vid:X4}:{reg.Pid:X4}  {reg.FullName}");
        }
    }
}
