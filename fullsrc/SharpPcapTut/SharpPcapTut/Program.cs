using PacketDotNet;
using SharpPcap;
using System;
using System.Net.NetworkInformation;

namespace SharpPcapDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var devices = CaptureDeviceList.Instance;
            if (devices.Count < 1)
            {
                Console.WriteLine("No capture devices found. Install Npcap/WinPcap?");
                return;
            }

            Console.WriteLine("Available devices:");
            for (int i = 0; i < devices.Count; i++)
            {
                Console.WriteLine($"{i}: {devices[i].Description}");
            }

            Console.Write("Select device number: ");
            if (!int.TryParse(Console.ReadLine(), out int selected) || selected < 0 || selected >= devices.Count)
            {
                Console.WriteLine("Invalid device number.");
                return;
            }

            var device = devices[selected];
            device.Open();

            var sourceHwAddress = device.MacAddress;
            var destHwAddress = PhysicalAddress.Parse("FF-FF-FF-FF-FF-FF"); // Broadcast address

            var ethernetPacket = new EthernetPacket(
                sourceHwAddress,
                destHwAddress,

                EthernetType.IPv4  
            );
            var rawData = ethernetPacket.Bytes;

            device.SendPacket(rawData);

            device.Close();
        }
    }
}
