using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreBoyTests
{
    [TestClass]
    public class Cpu8BitAlu : CpuTestBase
    {
        [TestMethod]
        public void TestAdd_ZeroFlag()
        {
            InitCpu(new byte[]
            {
                0x87, // a+a
                0x80, // a+b
                0x81, // a+c
                0x82, // a+d
                0x83, // a+e
                0x84, // a+h
                0x85, // a+l
                // todo test 0x86 // a+(hl)
                0xc6, // a+n
                0x00,
            });

            cpu.a = 0x0;
            cpu.RunOne();
            Assert.AreEqual(0x0, cpu.a, "didn't add properly");
            TestFlags(true, false, false, false);

            cpu.a = 0x0;
            cpu.b = 0x0;
            cpu.RunOne();
            Assert.AreEqual(0x0, cpu.a, "didn't add properly");
            TestFlags(true, false, false, false);

            cpu.a = 0x0;
            cpu.c = 0x0;
            cpu.RunOne();
            Assert.AreEqual(0x0, cpu.a, "didn't add properly");
            TestFlags(true, false, false, false);

            cpu.a = 0x0;
            cpu.d = 0x0;
            cpu.RunOne();
            Assert.AreEqual(0x0, cpu.a, "didn't add properly");
            TestFlags(true, false, false, false);

            cpu.a = 0x0;
            cpu.e = 0x0;
            cpu.RunOne();
            Assert.AreEqual(0x0, cpu.a, "didn't add properly");
            TestFlags(true, false, false, false);

            cpu.a = 0x0;
            cpu.h = 0x0;
            cpu.RunOne();
            Assert.AreEqual(0x0, cpu.a, "didn't add properly");
            TestFlags(true, false, false, false);

            cpu.a = 0x0;
            cpu.l = 0x0;
            cpu.RunOne();
            Assert.AreEqual(0x0, cpu.a, "didn't add properly");
            TestFlags(true, false, false, false);

            cpu.a = 0x0;
            cpu.RunOne();
            Assert.AreEqual(0x0, cpu.a, "didn't add properly");
            TestFlags(true, false, false, false);
        }

        [TestMethod]
        public void TestAdd_HalfCarryFlag()
        {
            InitCpu(new byte[]
            {
                0x87, // a+a
                0x80, // a+b
                0x81, // a+c
                0x82, // a+d
                0x83, // a+e
                0x84, // a+h
                0x85, // a+l
                // todo test 0x86 // a+(hl)
                0xc6, // a+n
                0x0f,
            });

            cpu.a = 0x08;
            cpu.RunOne();
            Assert.AreEqual(0x10, cpu.a, "didn't add properly");
            TestFlags(false, false, false, true);

            cpu.a = 0x08;
            cpu.b = 0x09;
            cpu.RunOne();
            Assert.AreEqual(0x11, cpu.a, "didn't add properly");
            TestFlags(false, false, false, true);

            cpu.a = 0x08;
            cpu.c = 0x18;
            cpu.RunOne();
            Assert.AreEqual(0x20, cpu.a, "didn't add properly");
            TestFlags(false, false, false, true);

            cpu.a = 0x08;
            cpu.d = 0x29;
            cpu.RunOne();
            Assert.AreEqual(0x31, cpu.a, "didn't add properly");
            TestFlags(false, false, false, true);

            cpu.a = 0x48;
            cpu.e = 0x48;
            cpu.RunOne();
            Assert.AreEqual(0x90, cpu.a, "didn't add properly");
            TestFlags(false, false, false, true);

            cpu.a = 0x7f;
            cpu.h = 0x6f;
            cpu.RunOne();
            Assert.AreEqual(0xee, cpu.a, "didn't add properly");
            TestFlags(false, false, false, true);

            cpu.a = 0x7f;
            cpu.l = 0x7f;
            cpu.RunOne();
            Assert.AreEqual(0xfe, cpu.a, "didn't add properly");
            TestFlags(false, false, false, true);

            cpu.a = 0x0f;
            cpu.RunOne();
            Assert.AreEqual(0x1e, cpu.a, "didn't add properly");
            TestFlags(false, false, false, true);
        }

        [TestMethod]
        public void TestAdd_NoHalfCarryFlag()
        {
            test = new byte[]
            {
                0x87, // a+a
                0x80, // a+b
                0x81, // a+c
                0x82, // a+d
                0x83, // a+e
                0x84, // a+h
                0x85, // a+l
                // todo test 0x86 // a+(hl)
                0xc6, // a+n
                0xf7,
            };

            InitCpu();

            cpu.a = 0x07;
            cpu.RunOne();
            Assert.AreEqual(0x0e, cpu.a, "didn't add properly");
            TestFlags(false, false, false, false);

            cpu.a = 0x08;
            cpu.b = 0x07;
            cpu.RunOne();
            Assert.AreEqual(0x0f, cpu.a, "didn't add properly");
            TestFlags(false, false, false, false);

            cpu.a = 0x07;
            cpu.c = 0x8;
            cpu.RunOne();
            Assert.AreEqual(0x0f, cpu.a, "didn't add properly");
            TestFlags(false, false, false, false);

            cpu.a = 0x17;
            cpu.d = 0x18;
            cpu.RunOne();
            Assert.AreEqual(0x2f, cpu.a, "didn't add properly");
            TestFlags(false, false, false, false);

            cpu.a = 0xf0;
            cpu.e = 0x0e;
            cpu.RunOne();
            Assert.AreEqual(0xfe, cpu.a, "didn't add properly");
            TestFlags(false, false, false, false);

            cpu.a = 0xff;
            cpu.h = 0x00;
            cpu.RunOne();
            Assert.AreEqual(0xff, cpu.a, "didn't add properly");
            TestFlags(false, false, false, false);

            cpu.a = 0x00;
            cpu.l = 0xff;
            cpu.RunOne();
            Assert.AreEqual(0xff, cpu.a, "didn't add properly");
            TestFlags(false, false, false, false);

            cpu.a = 0x08;
            cpu.RunOne();
            Assert.AreEqual(0xff, cpu.a, "didn't add properly");
            TestFlags(false, false, false, false);
        }

        [TestMethod]
        public void TestAdd_CarryFlag()
        {
            InitCpu(new byte[] {
                0x87, // a+a
                0x80, // a+b
                0x81, // a+c
                0x82, // a+d
                0x83, // a+e
                0x84, // a+h
                0x85, // a+l
                // todo test 0x86 // a+(hl)
                0xc6, // a+n
                0x81,
            });

            cpu.a = 0x91;
            cpu.RunOne();
            Assert.AreEqual(0x22, cpu.a, "didn't add properly");
            TestFlags(false, false, true, false);

            cpu.a = 0xf1;
            cpu.b = 0x11;
            cpu.RunOne();
            Assert.AreEqual(0x02, cpu.a, "didn't add properly");
            TestFlags(false, false, true, false);

            cpu.a = 0x11;
            cpu.c = 0xf1;
            cpu.RunOne();
            Assert.AreEqual(0x02, cpu.a, "didn't add properly");
            TestFlags(false, false, true, false);

            cpu.a = 0xf7;
            cpu.d = 0xf7;
            cpu.RunOne();
            Assert.AreEqual(0xee, cpu.a, "didn't add properly");
            TestFlags(false, false, true, false);

            cpu.a = 0x9f;
            cpu.e = 0x70;
            cpu.RunOne();
            Assert.AreEqual(0x0f, cpu.a, "didn't add properly");
            TestFlags(false, false, true, false);

            cpu.a = 0x81;
            cpu.h = 0x81;
            cpu.RunOne();
            Assert.AreEqual(0x02, cpu.a, "didn't add properly");
            TestFlags(false, false, true, false);

            cpu.a = 0x81;
            cpu.l = 0x81;
            cpu.RunOne();
            Assert.AreEqual(0x02, cpu.a, "didn't add properly");
            TestFlags(false, false, true, false);

            cpu.a = 0x81;
            cpu.RunOne();
            Assert.AreEqual(0x02, cpu.a, "didn't add properly");
            TestFlags(false, false, true, false);
        }
    }
}
