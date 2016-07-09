using System;
using CoreBoy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreBoyTests
{
    [TestClass]
    public abstract class CpuTestBase
    {
        protected Cpu cpu;
        protected byte[] test;

        [TestInitialize]
        public void Initialize()
        {
            cpu = null;
            test = null;
        }

        protected void InitCpu()
        {
            cpu = new Cpu(0x0)
            {
                Rom = test
            };
        }

        protected void TestFlags(bool zeroSet, bool negativeSet, bool carrySet, bool halfCarrySet)
        {
            Assert.AreEqual(zeroSet, cpu.IsFlagSet(Cpu.FlagZero), "zero flag should have been " + (zeroSet ? "set" : "reset" ));
            Assert.AreEqual(negativeSet, cpu.IsFlagSet(Cpu.FlagNegative), "negative flag should have been " + (negativeSet ? "set" : "reset"));
            Assert.AreEqual(carrySet, cpu.IsFlagSet(Cpu.FlagCarry), "carry flag should have been " + (carrySet ? "set" : "reset"));
            Assert.AreEqual(halfCarrySet, cpu.IsFlagSet(Cpu.FlagHalfCarry), "half carry flag should have been " + (halfCarrySet ? "set" : "reset"));
        }
    }
}
