using System;
using System.IO;

namespace CoreBoy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1) {
                Console.Error.WriteLine("Missing rom path argument");
                return;
            }

            byte[] rom;

            var romPath = args[0];
            using (var fileStream = File.Open(romPath, FileMode.Open))
            using (var romStream = new MemoryStream())
            {
                fileStream.CopyTo(romStream);

                rom = romStream.ToArray();
            }

            var cpu = new Cpu()
            {
                Rom = rom
            };
            cpu.Run();
        }
    }
}
