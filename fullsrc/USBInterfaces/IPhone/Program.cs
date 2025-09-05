// dotnet add package LibUsbDotNet
using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.Info;
using LibUsbDotNet.Descriptors;

class Program
{
    static void Main()
    {
        ushort vid = 0x203A, pid = 0xFFF9; // Parallels iPhone Camera
        using UsbDevice? dev = UsbDevice.OpenUsbDevice(new UsbDeviceFinder(vid, pid));
        if (dev == null) { Console.WriteLine("Device not found."); return; }

        var whole = dev as IUsbDevice;
        whole?.SetConfiguration(1);
        whole?.ClaimInterface(0); // if claimable; ignore errors for read-only inspection

        Console.WriteLine("=== Device Strings ===");
        try
        {
            Console.WriteLine("Manufacturer: " + dev.Info.ManufacturerString);
            Console.WriteLine("Product     : " + dev.Info.ProductString);
            Console.WriteLine("Serial      : " + dev.Info.SerialString);
        }
        catch { /* some devices omit strings */ }

        Console.WriteLine("\n=== Configs / Interfaces / Endpoints ===");
        foreach (UsbConfigInfo cfg in dev.Configs)
        {
            Console.WriteLine($"Config {cfg.Descriptor.ConfigID}");
            foreach (UsbInterfaceInfo ii in cfg.InterfaceInfoList)
            {
                //foreach (UsbInterfaceDescriptor idesc in ii.DescriptorList)
                //{
                //    Console.WriteLine($"  IF {idesc.InterfaceID}, Alt {idesc.AlternateID}, Class 0x{idesc.Class:X2}, Sub 0x{idesc.SubClass:X2}, Prot 0x{idesc.Protocol:X2}");
                //}
                //foreach (UsbEndpointInfo ep in ii.EndpointInfoList)
                //{
                //    var d = ep.Descriptor;
                //    Console.WriteLine($"    EP 0x{d.EndpointID:X2}  {(d.IsIn ? "IN " : "OUT")}  Attr={d.Attributes}  MaxPkt={d.MaxPacketSize}");
                //}
            }
        }

        // Optional: issue a standard GET_DESCRIPTOR (DEVICE) control request
        // (purely read-only; proves control pipe works)
        //var setup = new UsbSetupPacket(
        //    (byte)(UsbCtrlFlags.Direction_In | UsbCtrlFlags.RequestType_Standard | UsbCtrlFlags.Recipient_Device),
        //    (byte)StandardDeviceRequest.GetDescriptor,
        //    (short)((DescriptorType.Device << 8) | 0), // wValue: type<<8 | index
        //    0,
        //    18); // device descriptor size
        byte[] buf = new byte[18];
        //if (dev.ControlTransfer(ref setup, buf, buf.Length, out int len))
        //    Console.WriteLine($"\nGET_DESCRIPTOR(DEVICE): {len} bytes OK");
        //else
        //    Console.WriteLine("\nGET_DESCRIPTOR(DEVICE): failed");

        try { whole?.ReleaseInterface(0); } catch { }
    }

    enum StandardDeviceRequest : byte { GetDescriptor = 6 }
    enum DescriptorType : byte { Device = 1 }
}
