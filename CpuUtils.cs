using System;

namespace CoreBoy
{
    public static class CpuUtils
    {
        public static byte Get8BitRegister(this Cpu cpu, string register) {
            switch (register) {
                case "a":
                    return cpu.a;
                case "b":
                    return cpu.b;
                case "c":
                    return cpu.c;
                case "d":
                    return cpu.d;
                case "e":
                    return cpu.e;
                case "f":
                    return cpu.f;
                case "h":
                    return cpu.h;
                case "l":
                    return cpu.l;
            }
            throw new Exception($"Unknown 8-bit register: {register}");
        }

        public static ushort Get16BitRegister(this Cpu cpu, string register) {
            switch (register) {
                case "af":
                    return cpu.af;
                case "bc":
                    return cpu.bc;
                case "de":
                    return cpu.de;
                case "hl":
                    return cpu.hl;
            }
            throw new Exception($"Unknown 16-bit register: {register}");
        }
    }
}