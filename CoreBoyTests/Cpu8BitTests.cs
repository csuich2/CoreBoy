using System;
using CoreBoy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreBoyTests
{
    [TestClass]
    public class Cpu8BitTests : CpuTestBase
    {
        [TestMethod]
        public void TestLoadImmediates()
        {
            test = new byte[]
            {
                0x06, // Load into b
                0xf0,
                0x0e, // Load into c
                0xf1,
                0x16, // Load into d
                0xf2,
                0x1e, // Load into e
                0xf3,
                0x26, // Load into h
                0xf4,
                0x2e, // Load into l
                0xf5,
            };

            InitCpu();

            Assert.AreEqual(cpu.b, 0, $"register b should have been initialized to 0");
            Assert.AreEqual(cpu.c, 0, $"register c should have been initialized to 0");
            Assert.AreEqual(cpu.d, 0, $"register d should have been initialized to 0");
            Assert.AreEqual(cpu.e, 0, $"register e should have been initialized to 0");
            Assert.AreEqual(cpu.h, 0, $"register h should have been initialized to 0");
            Assert.AreEqual(cpu.l, 0, $"register l should have been initialized to 0");

            cpu.Run();

            Assert.AreEqual(cpu.b, 0xf0, $"register b loaded the wrong value");
            Assert.AreEqual(cpu.c, 0xf1, $"register c loaded the wrong value");
            Assert.AreEqual(cpu.d, 0xf2, $"register d loaded the wrong value");
            Assert.AreEqual(cpu.e, 0xf3, $"register e loaded the wrong value");
            Assert.AreEqual(cpu.h, 0xf4, $"register h loaded the wrong value");
            Assert.AreEqual(cpu.l, 0xf5, $"register l loaded the wrong value");
        }

        [TestMethod]
        public void TestLoadIntoA()
        {
            test = new byte[]
            {
                0x7f, // a -> a
                0x78, // b -> a
                0x79, // c -> a
                0x7a, // d -> a
                0x7b, // e -> a
                0x7c, // h -> a
                0x7d, // l -> a
                // TODO test 0x7e
                // TODO test 0a, 1a, 7e, fa
                0x3e, // # -> a
                0xe0,
            };

            InitCpu();

            var aValue = (byte)0xd0;
            var bValue = (byte)0xd1;
            var cValue = (byte)0xd2;
            var dValue = (byte)0xd3;
            var eValue = (byte)0xd4;
            var hValue = (byte)0xd6;
            var lValue = (byte)0xd7;

            cpu.a = aValue;
            cpu.b = bValue;
            cpu.c = cValue;
            cpu.d = dValue;
            cpu.e = eValue;
            cpu.h = hValue;
            cpu.l = lValue;

            Action step = () => {
                cpu.RunOne();
            };

            step();
            Assert.AreEqual(aValue, cpu.a, $"register a loaded the wrong value");

            step();
            Assert.AreEqual(bValue, cpu.a, $"register a loaded the wrong value");

            step();
            Assert.AreEqual(cValue, cpu.a, $"register a loaded the wrong value");

            step();
            Assert.AreEqual(dValue, cpu.a, $"register a loaded the wrong value");

            step();
            Assert.AreEqual(eValue, cpu.a, $"register a loaded the wrong value");

            step();
            Assert.AreEqual(hValue, cpu.a, $"register a loaded the wrong value");

            step();
            Assert.AreEqual(lValue, cpu.a, $"register a loaded the wrong value");

            step();
            Assert.AreEqual(0xe0, cpu.a, $"register a loaded the wrong value");
        }

        [TestMethod]
        public void TestLoadIntoB()
        {
            test = new byte[]
            {
                0x40, // b -> b
                0x41, // c -> b
                0x42, // d -> b
                0x43, // e -> b
                0x44, // h -> b
                0x45, // l -> b
                // TODO test 0x46
            };

            InitCpu();

            var bValue = (byte)0xd1;
            var cValue = (byte)0xd2;
            var dValue = (byte)0xd3;
            var eValue = (byte)0xd4;
            var hValue = (byte)0xd6;
            var lValue = (byte)0xd7;

            cpu.b = bValue;
            cpu.c = cValue;
            cpu.d = dValue;
            cpu.e = eValue;
            cpu.h = hValue;
            cpu.l = lValue;

            Action step = () =>
            {
                cpu.RunOne();
            };

            step();
            Assert.AreEqual(bValue, cpu.b, $"register b loaded the wrong value");

            step();
            Assert.AreEqual(cValue, cpu.b, $"register b loaded the wrong value");

            step();
            Assert.AreEqual(dValue, cpu.b, $"register b loaded the wrong value");

            step();
            Assert.AreEqual(eValue, cpu.b, $"register b loaded the wrong value");

            step();
            Assert.AreEqual(hValue, cpu.b, $"register b loaded the wrong value");

            step();
            Assert.AreEqual(lValue, cpu.b, $"register b loaded the wrong value");
        }

        [TestMethod]
        public void TestLoadIntoC()
        {
            test = new byte[]
            {
                0x49, // c -> c
                0x48, // b -> c
                0x4a, // d -> c
                0x4b, // e -> c
                0x4c, // h -> c
                0x4d, // l -> c
                // TODO test 0x4e
            };

            InitCpu();

            var bValue = (byte)0xd1;
            var cValue = (byte)0xd2;
            var dValue = (byte)0xd3;
            var eValue = (byte)0xd4;
            var hValue = (byte)0xd6;
            var lValue = (byte)0xd7;

            cpu.b = bValue;
            cpu.c = cValue;
            cpu.d = dValue;
            cpu.e = eValue;
            cpu.h = hValue;
            cpu.l = lValue;

            Action step = () =>
            {
                cpu.RunOne();
            };

            step();
            Assert.AreEqual(cValue, cpu.c, $"register c loaded the wrong value");

            step();
            Assert.AreEqual(bValue, cpu.c, $"register c loaded the wrong value");

            step();
            Assert.AreEqual(dValue, cpu.c, $"register c loaded the wrong value");

            step();
            Assert.AreEqual(eValue, cpu.c, $"register c loaded the wrong value");

            step();
            Assert.AreEqual(hValue, cpu.c, $"register c loaded the wrong value");

            step();
            Assert.AreEqual(lValue, cpu.c, $"register c loaded the wrong value");
        }

        [TestMethod]
        public void TestLoadIntoD()
        {
            test = new byte[]
            {
                0x52, // d -> d
                0x50, // b -> d
                0x51, // c -> d
                0x53, // e -> d
                0x54, // h -> d
                0x55, // l -> d
                // TODO test 0x56
            };

            InitCpu();

            var bValue = (byte)0xd1;
            var cValue = (byte)0xd2;
            var dValue = (byte)0xd3;
            var eValue = (byte)0xd4;
            var hValue = (byte)0xd6;
            var lValue = (byte)0xd7;

            cpu.b = bValue;
            cpu.c = cValue;
            cpu.d = dValue;
            cpu.e = eValue;
            cpu.h = hValue;
            cpu.l = lValue;

            Action step = () =>
            {
                cpu.RunOne();
            };

            step();
            Assert.AreEqual(dValue, cpu.d, $"register d loaded the wrong value");

            step();
            Assert.AreEqual(bValue, cpu.d, $"register d loaded the wrong value");

            step();
            Assert.AreEqual(cValue, cpu.d, $"register d loaded the wrong value");

            step();
            Assert.AreEqual(eValue, cpu.d, $"register d loaded the wrong value");

            step();
            Assert.AreEqual(hValue, cpu.d, $"register d loaded the wrong value");

            step();
            Assert.AreEqual(lValue, cpu.d, $"register d loaded the wrong value");
        }

        [TestMethod]
        public void TestLoadIntoE()
        {
            test = new byte[]
            {
                0x5b, // e -> e
                0x58, // b -> e
                0x59, // c -> e
                0x5a, // d -> e
                0x5c, // h -> e
                0x5d, // l -> e
                // TODO test 0x5e
            };

            InitCpu();

            var bValue = (byte)0xd1;
            var cValue = (byte)0xd2;
            var dValue = (byte)0xd3;
            var eValue = (byte)0xd4;
            var hValue = (byte)0xd6;
            var lValue = (byte)0xd7;

            cpu.b = bValue;
            cpu.c = cValue;
            cpu.d = dValue;
            cpu.e = eValue;
            cpu.h = hValue;
            cpu.l = lValue;

            Action step = () =>
            {
                cpu.RunOne();
            };

            step();
            Assert.AreEqual(eValue, cpu.e, $"register e loaded the wrong value");

            step();
            Assert.AreEqual(bValue, cpu.e, $"register e loaded the wrong value");

            step();
            Assert.AreEqual(cValue, cpu.e, $"register e loaded the wrong value");

            step();
            Assert.AreEqual(dValue, cpu.e, $"register e loaded the wrong value");

            step();
            Assert.AreEqual(hValue, cpu.e, $"register e loaded the wrong value");

            step();
            Assert.AreEqual(lValue, cpu.e, $"register e loaded the wrong value");
        }

        [TestMethod]
        public void TestLoadIntoH()
        {
            test = new byte[]
            {
                0x64, // h -> h
                0x60, // b -> h
                0x61, // c -> h
                0x62, // d -> h
                0x63, // e -> h
                0x65, // l -> h
                // TODO test 0x66
            };

            InitCpu();

            var bValue = (byte)0xd1;
            var cValue = (byte)0xd2;
            var dValue = (byte)0xd3;
            var eValue = (byte)0xd4;
            var hValue = (byte)0xd6;
            var lValue = (byte)0xd7;

            cpu.b = bValue;
            cpu.c = cValue;
            cpu.d = dValue;
            cpu.e = eValue;
            cpu.h = hValue;
            cpu.l = lValue;

            Action step = () =>
            {
                cpu.RunOne();
            };

            step();
            Assert.AreEqual(hValue, cpu.h, $"register h loaded the wrong value");

            step();
            Assert.AreEqual(bValue, cpu.h, $"register h loaded the wrong value");

            step();
            Assert.AreEqual(cValue, cpu.h, $"register h loaded the wrong value");

            step();
            Assert.AreEqual(dValue, cpu.h, $"register h loaded the wrong value");

            step();
            Assert.AreEqual(eValue, cpu.h, $"register h loaded the wrong value");

            step();
            Assert.AreEqual(lValue, cpu.h, $"register h loaded the wrong value");
        }

        [TestMethod]
        public void TestLoadIntoL()
        {
            test = new byte[]
            {
                0x6d, // l -> l
                0x68, // b -> l
                0x69, // c -> l
                0x6a, // d -> l
                0x6b, // e -> l
                0x6c, // h -> l
                // TODO test 0x6e
            };

            InitCpu();

            var bValue = (byte)0xd1;
            var cValue = (byte)0xd2;
            var dValue = (byte)0xd3;
            var eValue = (byte)0xd4;
            var hValue = (byte)0xd6;
            var lValue = (byte)0xd7;

            cpu.b = bValue;
            cpu.c = cValue;
            cpu.d = dValue;
            cpu.e = eValue;
            cpu.h = hValue;
            cpu.l = lValue;

            Action step = () =>
            {
                cpu.RunOne();
            };

            step();
            Assert.AreEqual(lValue, cpu.l, $"register l loaded the wrong value");

            step();
            Assert.AreEqual(bValue, cpu.l, $"register l loaded the wrong value");

            step();
            Assert.AreEqual(cValue, cpu.l, $"register l loaded the wrong value");

            step();
            Assert.AreEqual(dValue, cpu.l, $"register l loaded the wrong value");

            step();
            Assert.AreEqual(eValue, cpu.l, $"register l loaded the wrong value");

            step();
            Assert.AreEqual(hValue, cpu.l, $"register l loaded the wrong value");
        }

        // TODO test 70 - 75, 36

        [TestMethod]
        public void TestLoadAInto()
        {
            test = new byte[]
            {
                0x7f, // a -> a
                0x47, // a -> b
                0x4f, // a -> c
                0x57, // a -> d
                0x5f, // a -> e
                0x67, // a -> h
                0x6f, // a -> l
                // TODO test 02, 12, 77, ea
            };

            InitCpu();

            var aValue = (byte)0xd0;
            var bValue = (byte)0xd1;
            var cValue = (byte)0xd2;
            var dValue = (byte)0xd3;
            var eValue = (byte)0xd4;
            var hValue = (byte)0xd5;
            var lValue = (byte)0xd6;

            cpu.a = aValue;
            cpu.b = bValue;
            cpu.c = cValue;
            cpu.d = dValue;
            cpu.e = eValue;
            cpu.h = hValue;
            cpu.l = lValue;

            Action<byte> step = (byte val) =>
            {
                cpu.a = val;
                cpu.RunOne();
            };

            step(aValue);
            Assert.AreEqual(aValue, cpu.a, $"register a loaded the wrong value");

            step(bValue);
            Assert.AreEqual(bValue, cpu.b, $"register b loaded the wrong value");

            step(cValue);
            Assert.AreEqual(cValue, cpu.c, $"register c loaded the wrong value");

            step(dValue);
            Assert.AreEqual(dValue, cpu.d, $"register d loaded the wrong value");

            step(eValue);
            Assert.AreEqual(eValue, cpu.e, $"register e loaded the wrong value");

            step(hValue);
            Assert.AreEqual(hValue, cpu.h, $"register h loaded the wrong value");

            step(lValue);
            Assert.AreEqual(lValue, cpu.l, $"register l loaded the wrong value");
        }

        [TestMethod]
        public void TestLoadAddressCIntoA()
        {
            var expectedValue = (byte)0xd0;
            test = new byte[ushort.MaxValue];
            test[0x0000] = 0xf2;
            test[0xff23] = expectedValue;

            InitCpu();

            cpu.c = 0x23;

            cpu.RunOne();

            Assert.AreEqual(expectedValue, cpu.a, "didn't load 0xff+regc into a");
        }

        [TestMethod]
        public void TestLoadAIntoAddressC()
        {
            test = new byte[ushort.MaxValue];
            test[0x0000] = 0xe2;

            InitCpu();

            var expectedValue = (byte)0xd0;
            cpu.a = expectedValue;
            cpu.c = 0x23;

            cpu.RunOne();

            Assert.AreEqual(expectedValue, test[0xff23], "didn't load a into 0xff+regc");
        }

        // TODO test 3a

        // TODO test 32

        // TODO test 2a

        // TODO test 22

        [TestMethod]
        public void TestLoadAIntoAddressN()
        {
            test = new byte[ushort.MaxValue];
            test[0x0000] = 0xe0;
            test[0x0001] = 0x23;

            InitCpu();

            var expectedValue = (byte)0xd0;
            cpu.a = expectedValue;

            cpu.RunOne();

            Assert.AreEqual(expectedValue, test[0xff23], "didn't load a into 0xff+n");
        }

        [TestMethod]
        public void TestLoadAddressNIntoA()
        {
            var expectedValue = (byte)0xd0;
            test = new byte[ushort.MaxValue];
            test[0x0000] = 0xf0;
            test[0x0001] = 0x23;
            test[0xff23] = expectedValue;

            InitCpu();

            cpu.RunOne();

            Assert.AreEqual(expectedValue, cpu.a, "didn't load 0xff+n into a");
        }
    }
}
