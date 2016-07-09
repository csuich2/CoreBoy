using System;
using CoreBoy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreBoyTests
{
    [TestClass]
    public class Cpu16BitTests : CpuTestBase
    {
        [TestMethod]
        public void TestLoadImmediates()
        {
            test = new byte[]
            {
                0x01, // nn -> bc
                0x11,
                0x22,
                0x11, // nn -> de
                0x33,
                0x44,
                0x21, // nn -> hl
                0x55,
                0x66,
                0x31, // nn -> sp
                0x77,
                0x88,
            };

            InitCpu();

            cpu.RunOne();
            Assert.AreEqual(0x1122, cpu.bc, "register bc loaded the wrong value");

            cpu.RunOne();
            Assert.AreEqual(0x3344, cpu.de, "register de loaded the wrong value");

            cpu.RunOne();
            Assert.AreEqual(0x5566, cpu.hl, "register hl loaded the wrong value");

            cpu.RunOne();
            Assert.AreEqual(0x7788, cpu.sp, "register sp loaded the wrong value");
        }

        [TestMethod]
        public void TestLoadHlIntoSp()
        {
            test = new byte[]
            {
                0xf9, // hl -> sp
            };

            InitCpu();

            var expectedValue = (ushort)0xabcd;
            cpu.hl = expectedValue;

            cpu.RunOne();
            Assert.AreEqual(expectedValue, cpu.sp, "register sp loaded the wrong value");
        }

        [TestMethod]
        public void TestLoadSpNIntoHl()
        {
            test = new byte[]
            {
                0xf8, // sp+n -> hl, no carry, no half carry
                0x34,
                0xf8, // sp+n -> hl, no carry, half carry
                0xff,
                0xf8, // sp+n -> hl, carry, half carry
                0xff,
            };

            InitCpu();

            cpu.sp = 0x1200;
            cpu.SetFlag(Cpu.FlagZero | Cpu.FlagNegative | Cpu.FlagCarry | Cpu.FlagHalfCarry);

            cpu.RunOne();
            Assert.AreEqual(0x1234, cpu.hl, "register hl loaded the wrong value");
            TestFlags(false, false, false, false);

            cpu.sp = 0x1201;
            cpu.SetFlag(Cpu.FlagZero | Cpu.FlagNegative | Cpu.FlagCarry);
            cpu.ResetFlag(Cpu.FlagHalfCarry);
            cpu.RunOne();
            Assert.AreEqual(0x1300, cpu.hl, "register hl loaded the wrong value");
            TestFlags(false, false, false, true);

            cpu.sp = 0xff01;
            cpu.SetFlag(Cpu.FlagZero | Cpu.FlagNegative);
            cpu.ResetFlag(Cpu.FlagCarry | Cpu.FlagHalfCarry);
            cpu.RunOne();
            Assert.AreEqual(0x0000, cpu.hl, "register hl loaded the wrong value");
            TestFlags(false, false, true, true);
        }

        [TestMethod]
        public void TestLoadSpIntoN()
        {
            test = new byte[ushort.MaxValue];
            test[0x0000] = 0x08;
            test[0x0001] = 0xcd;
            test[0x0002] = 0xab;

            InitCpu();

            var expectedValue = (ushort)0x1234;
            cpu.sp = expectedValue;

            cpu.RunOne();
            Assert.AreEqual(expectedValue, cpu.ReadShort(0xabcd), "didn't load sp into n");
        }

        [TestMethod]
        public void TestLoadOntoStack()
        {
            test = new byte[ushort.MaxValue];
            test[0x0000] = 0xf5; // af -> stack
            test[0x0001] = 0xc5; // bc -> stack
            test[0x0002] = 0xd5; // de -> stack
            test[0x0003] = 0xe5; // hl -> stack

            InitCpu();

            var afValue = (ushort)0x1122;
            var bcValue = (ushort)0x3344;
            var deValue = (ushort)0x5566;
            var hlValue = (ushort)0x7788;

            cpu.af = afValue;
            cpu.bc = bcValue;
            cpu.de = deValue;
            cpu.hl = hlValue;

            cpu.RunOne();
            Assert.AreEqual(afValue, cpu.Pop(), "didn't get af value from stack");

            cpu.RunOne();
            Assert.AreEqual(bcValue, cpu.Pop(), "didn't get bc value from stack");

            cpu.RunOne();
            Assert.AreEqual(deValue, cpu.Pop(), "didn't get de value from stack");

            cpu.RunOne();
            Assert.AreEqual(hlValue, cpu.Pop(), "didn't get hl value from stack");
        }

        [TestMethod]
        public void TestLoadFromStack()
        {
            test = new byte[ushort.MaxValue];
            test[0x0000] = 0xf1; // stack -> af
            test[0x0001] = 0xc1; // stack -> bc
            test[0x0002] = 0xd1; // stack -> de
            test[0x0003] = 0xe1; // stack -> hl

            InitCpu();

            var afValue = (ushort)0x1122;
            var bcValue = (ushort)0x3344;
            var deValue = (ushort)0x5566;
            var hlValue = (ushort)0x7788;

            cpu.Push(hlValue);
            cpu.Push(deValue);
            cpu.Push(bcValue);
            cpu.Push(afValue);
            

            cpu.RunOne();
            Assert.AreEqual(afValue, cpu.af, "didn't get af value from stack");

            cpu.RunOne();
            Assert.AreEqual(bcValue, cpu.bc, "didn't get bc value from stack");

            cpu.RunOne();
            Assert.AreEqual(deValue, cpu.de, "didn't get de value from stack");

            cpu.RunOne();
            Assert.AreEqual(hlValue, cpu.hl, "didn't get hl value from stack");
        }
    }
}
