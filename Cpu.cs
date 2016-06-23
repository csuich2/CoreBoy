using System;
using System.Collections.Generic;

namespace CoreBoy
{
    public class Cpu
    {
        delegate void Instruction();

        // 8-bit registers
        public byte a { get; private set; }
        public byte b { get; private set; }
        public byte c { get; private set; }
        public byte d { get; private set; }
        public byte e { get; private set; }
        public byte f { get; private set; }
        public byte h { get; private set; }
        public byte l { get; private set; }

        // 16-bit "virtual" registers
        public ushort af {
            get {
                return (ushort) ((ushort)a << 8 | f);
            }
            set {
                var temp = af - value;
                a = (byte)(af >> 8);
                f = (byte)(af & 0xff);
            }
        }

        public ushort bc {
            get {
                return (ushort) ((ushort)b << 8 | c);
            }
            set {
                var temp = bc - value;
                b = (byte)(bc >> 8);
                c = (byte)(bc & 0xff);
            }
        }

        public ushort de { 
            get {
                return (ushort) ((ushort)d << 8 | e);
            }
            set {
                var temp = de - value;
                d = (byte)(de >> 8);
                e = (byte)(de & 0xff);
            }
        }

        public ushort hl {
            get {
                return (ushort) ((ushort)h << 8 | l);
            }
            set {
                var temp = hl - value;
                a = (byte)(hl >> 8);
                f = (byte)(hl & 0xff);
            }
        }

        byte flags;
        // Flag register looks like this:
        // 7 6 5 4 3 2 1 0
        // Z N H C 0 0 0 0
        // Z = Zero - set when the result of a math op is zero or two values match with a compare
        const byte FlagZero = 0x80;
        // N = Subtract - set if a subtraction was performed in the last math instruction
        const byte FlagNegative = 0x40;
        // H = Half Carry - set if a carry ocurred from the lower nibble in the last math instruction
        const byte FlagHalfCarry = 0x20;
        // C = Carry - set if a carry occurred from the last math operation or if register A is smaller with a compare
        const byte FlagCarry = 0x10;

        public bool IsFlagSet(byte flag) { return (flags & flag) != 0; }
        public void SetFlag(byte flag) { flags |= flag; }
        public void ResetFlag(byte flag) { flags &= (byte)~flag; }

        ushort pc = 0x0100; // Defaults to 100 hex
        ushort sp = 0xfffe; // Initialized to fffe hex

        // Memory
        byte[] Ram = new byte[8096];
        byte[] VideoRam = new byte[8096];
        byte[] Rom;
        
        IDictionary<ushort, Instruction> Instructions = new Dictionary<ushort, Instruction>();

        public void Run(byte[] rom)
        {
            Rom = rom;
        }

        private byte ReadNextRomByte()
        {
            return Rom[pc++];
        }

        private ushort ReadNextRomShort()
        {
            var temp = ReadShort(pc);
            pc += 2;
            return temp; 
        }

        private byte ReadByte(ushort pos)
        {
            // TODO this needs to do way more
            return Rom[pos];
        }

        private ushort ReadShort(ushort pos)
        {
            return (ushort)(((ushort)Rom[pos]) << 8 & (ushort)Rom[pos+1]);
        }

        private void WriteByte(ushort pos, byte val)
        {
            // TODO This needs to do way more
            Ram[pos] = val;
        }

        private void WriteShort(ushort pos, ushort val)
        {
            // TODO This needs to do way more
            Ram[pos] = (byte)(val & 0x00ff);
            Ram[pos+1] = (byte)((val & 0xff00) >> 8);
        }

        private void Push(ushort val)
        {
            sp -= 2;
            WriteShort(sp, val);
        }

        private ushort Pop()
        {
            var temp = ReadShort(sp);
            sp += 2;
            return temp;
        }

        private void SetupInstructions()
        {
            Setup8bitLoads();
            Setup16bitLoads();
            Setup8bitAlu();
            Setup16bitAlu();
            SetupMisc();
            SetupRotatesShifts();
            SetupBitOpcodes();
            SetupJumps();
            SetupCalls();
            SetupRestarts();
            SetupReturns();
        }

        public void Setup8bitLoads()
        {
            // 8-bit loads
            AddInstruction(0x02, () => WriteByte(bc, a));
            AddInstruction(0x06, () => b = ReadNextRomByte());
            AddInstruction(0x0a, () => a = ReadByte(bc));
            AddInstruction(0x0e, () => c = ReadNextRomByte());

            AddInstruction(0x12, () => WriteByte(de, a));
            AddInstruction(0x16, () => d = ReadNextRomByte());
            AddInstruction(0x1a, () => a = ReadByte(de));
            AddInstruction(0x1e, () => e = ReadNextRomByte());

            AddInstruction(0x22, () => WriteByte(hl++, a));
            AddInstruction(0x26, () => h = ReadNextRomByte());
            AddInstruction(0x2a, () => a = ReadByte(hl++));
            AddInstruction(0x2e, () => l = ReadNextRomByte());

            AddInstruction(0x32, () => WriteByte(hl--, a));
            AddInstruction(0x3a, () => a = ReadByte(hl--));
            AddInstruction(0x36, () => WriteByte(hl, ReadByte(hl)));
            AddInstruction(0x3e, () => a = ReadNextRomByte());

            AddInstruction(0x40, () => {});
            AddInstruction(0x41, () => b = c);
            AddInstruction(0x42, () => b = d);
            AddInstruction(0x43, () => b = e);
            AddInstruction(0x44, () => b = h);
            AddInstruction(0x45, () => b = l);
            AddInstruction(0x46, () => b = ReadByte(hl));
            AddInstruction(0x47, () => b = a);
            AddInstruction(0x48, () => c = b);
            AddInstruction(0x49, () => {});
            AddInstruction(0x4a, () => c = d);
            AddInstruction(0x4b, () => c = e);
            AddInstruction(0x4c, () => c = h);
            AddInstruction(0x4d, () => c = l);
            AddInstruction(0x4e, () => c = ReadByte(hl));
            AddInstruction(0x4f, () => c = a);

            AddInstruction(0x50, () => d = b);
            AddInstruction(0x51, () => d = c);
            AddInstruction(0x52, () => {});
            AddInstruction(0x53, () => d = e);
            AddInstruction(0x54, () => d = h);
            AddInstruction(0x55, () => d = l);
            AddInstruction(0x56, () => d = ReadByte(hl));
            AddInstruction(0x57, () => d = a);
            AddInstruction(0x58, () => e = b);
            AddInstruction(0x59, () => e = c);
            AddInstruction(0x5a, () => {});
            AddInstruction(0x5b, () => e = d);
            AddInstruction(0x5c, () => e = h);
            AddInstruction(0x5d, () => e = l);
            AddInstruction(0x5e, () => e = ReadByte(hl));
            AddInstruction(0x5f, () => e = a);

            AddInstruction(0x60, () => h = b);
            AddInstruction(0x61, () => h = c);
            AddInstruction(0x62, () => h = d);
            AddInstruction(0x64, () => h = e);
            AddInstruction(0x63, () => {});
            AddInstruction(0x65, () => h = l);
            AddInstruction(0x66, () => h = ReadByte(hl));
            AddInstruction(0x67, () => h = a);
            AddInstruction(0x68, () => l = b);
            AddInstruction(0x69, () => l = c);
            AddInstruction(0x6a, () => l = d);
            AddInstruction(0x6b, () => l = e);
            AddInstruction(0x6c, () => {});
            AddInstruction(0x6d, () => l = h);
            AddInstruction(0x6e, () => l = ReadByte(hl));
            AddInstruction(0x6f, () => l = a);

            AddInstruction(0x70, () => WriteByte(hl, b));
            AddInstruction(0x71, () => WriteByte(hl, c));
            AddInstruction(0x72, () => WriteByte(hl, d));
            AddInstruction(0x73, () => WriteByte(hl, e));
            AddInstruction(0x74, () => WriteByte(hl, h));
            AddInstruction(0x75, () => WriteByte(hl, l));

            AddInstruction(0x77, () => WriteByte(hl, a));
            AddInstruction(0x78, () => a = b);
            AddInstruction(0x79, () => a = c);
            AddInstruction(0x7a, () => a = d);
            AddInstruction(0x7b, () => a = e);
            AddInstruction(0x7c, () => a = h);
            AddInstruction(0x7d, () => a = l);
            AddInstruction(0x7e, () => a = ReadByte(hl));
            AddInstruction(0x7f, () => {});

            AddInstruction(0xe0, () => WriteByte((ushort)(0xff00+ReadNextRomByte()), a));
            AddInstruction(0xe2, () => WriteByte((ushort)(0xff00 + c), a));
            AddInstruction(0xea, () => WriteByte(ReadNextRomByte(), a));

            AddInstruction(0xf0, () => a = ReadByte((ushort)(0xff00+ReadNextRomByte())));
            AddInstruction(0xf2, () => a = ReadByte((ushort)(0xff00 + c)));
            AddInstruction(0xfa, () => a = ReadByte(ReadNextRomByte()));
        }

        public void Setup16bitLoads()
        {
            // 16-bit loads
            AddInstruction(0x01, () => bc = ReadNextRomShort());
            AddInstruction(0x08, () => WriteShort(ReadNextRomShort(), sp));

            AddInstruction(0x11, () => de = ReadNextRomShort());

            AddInstruction(0x21, () => hl = ReadNextRomShort());

            AddInstruction(0x31, () => sp = ReadNextRomShort());

            AddInstruction(0xc1, () => bc = Pop());
            AddInstruction(0xc5, () => Push(bc));

            AddInstruction(0xd1, () => de = Pop());
            AddInstruction(0xd5, () => Push(de));

            AddInstruction(0x11, () => hl = Pop());
            AddInstruction(0xe5, () => Push(hl));

            AddInstruction(0xf1, () => af = Pop());
            AddInstruction(0xf5, () => Push(af));
            AddInstruction(0xf8, GetSpPlusN());
            AddInstruction(0xf9, () => sp = hl);
        }

        public void Setup8bitAlu()
        {
            // 8-bit ALU
            AddInstruction(0x04, () => b = Inc(b));
            AddInstruction(0x04, () => b = Dec(b));
            AddInstruction(0x0c, () => c = Inc(c));
            AddInstruction(0x0c, () => c = Dec(c));

            AddInstruction(0x3c, () => a = Inc(a));
            AddInstruction(0x3c, () => a = Dec(a));

            AddInstruction(0x14, () => d = Inc(d));
            AddInstruction(0x14, () => d = Dec(d));
            AddInstruction(0x1c, () => e = Inc(e));
            AddInstruction(0x1c, () => e = Dec(e));

            AddInstruction(0x24, () => h = Inc(h));
            AddInstruction(0x24, () => h = Dec(h));
            AddInstruction(0x2c, () => l = Inc(l));
            AddInstruction(0x2c, () => l = Dec(l));

            AddInstruction(0x34, () => WriteByte(hl, Inc(ReadByte(hl))));
            AddInstruction(0x34, () => WriteByte(hl, Dec(ReadByte(hl))));

            AddInstruction(0x80, () => Add(b));
            AddInstruction(0x81, () => Add(c));
            AddInstruction(0x82, () => Add(d));
            AddInstruction(0x83, () => Add(e));
            AddInstruction(0x84, () => Add(h));
            AddInstruction(0x85, () => Add(l));
            AddInstruction(0x86, () => Add(ReadByte(hl)));
            AddInstruction(0x87, () => Add(a));
            AddInstruction(0x88, () => Adc(b));
            AddInstruction(0x89, () => Adc(c));
            AddInstruction(0x8a, () => Adc(d));
            AddInstruction(0x8b, () => Adc(e));
            AddInstruction(0x8c, () => Adc(h));
            AddInstruction(0x8d, () => Adc(l));
            AddInstruction(0x8e, () => Adc(ReadByte(hl)));
            AddInstruction(0x8f, () => Adc(a));

            AddInstruction(0x90, () => Sub(b));
            AddInstruction(0x91, () => Sub(c));
            AddInstruction(0x92, () => Sub(d));
            AddInstruction(0x93, () => Sub(e));
            AddInstruction(0x94, () => Sub(h));
            AddInstruction(0x95, () => Sub(l));
            AddInstruction(0x96, () => Sub(ReadByte(hl)));
            AddInstruction(0x97, () => Sub(a)); // OPTIMIZATION - Isn't this always 0?
            AddInstruction(0x98, () => Subc(b));
            AddInstruction(0x99, () => Subc(c));
            AddInstruction(0x9a, () => Subc(d));
            AddInstruction(0x9b, () => Subc(e));
            AddInstruction(0x9c, () => Subc(h));
            AddInstruction(0x9d, () => Subc(l));
            AddInstruction(0x9e, () => Subc(ReadByte(hl)));
            AddInstruction(0x9f, () => Subc(a));

            AddInstruction(0xa0, () => And(b));
            AddInstruction(0xa1, () => And(c));
            AddInstruction(0xa2, () => And(d));
            AddInstruction(0xa3, () => And(e));
            AddInstruction(0xa4, () => And(h));
            AddInstruction(0xa5, () => And(l));
            AddInstruction(0xa6, () => And(ReadByte(hl)));
            AddInstruction(0xa7, () => And(a)); // OPTIMIZATION - Isn't this always 0xff?
            AddInstruction(0xa8, () => Xor(b));
            AddInstruction(0xa9, () => Xor(c));
            AddInstruction(0xaa, () => Xor(d));
            AddInstruction(0xab, () => Xor(e));
            AddInstruction(0xac, () => Xor(h));
            AddInstruction(0xad, () => Xor(l));
            AddInstruction(0xae, () => Xor(ReadByte(hl)));
            AddInstruction(0xaf, () => Xor(a)); // OPTIMIZATION - Isn't this always 0x0?

            AddInstruction(0xb0, () => Or(b));
            AddInstruction(0xb1, () => Or(c));
            AddInstruction(0xb2, () => Or(d));
            AddInstruction(0xb3, () => Or(e));
            AddInstruction(0xb4, () => Or(h));
            AddInstruction(0xb5, () => Or(l));
            AddInstruction(0xb6, () => Or(ReadByte(hl)));
            AddInstruction(0xb7, () => Or(a)); // OPTIMIZATION - Isn't this always 'a'?
            AddInstruction(0xb8, () => Compare(b));
            AddInstruction(0xb9, () => Compare(c));
            AddInstruction(0xba, () => Compare(d));
            AddInstruction(0xbb, () => Compare(e));
            AddInstruction(0xbc, () => Compare(h));
            AddInstruction(0xbd, () => Compare(l));
            AddInstruction(0xbe, () => Compare(ReadByte(hl)));
            AddInstruction(0xbf, () => Compare(a)); // OPTIMIZATION - Isn't this always 'a'?

            AddInstruction(0xc6, () => Add(ReadNextRomByte()));
            AddInstruction(0xce, () => Adc(ReadNextRomByte()));

            AddInstruction(0xd6, () => Sub(ReadNextRomByte()));

            AddInstruction(0xe6, () => And(ReadNextRomByte()));
            AddInstruction(0xee, () => Xor(ReadNextRomByte()));

            AddInstruction(0xf6, () => Or(ReadNextRomByte()));
            AddInstruction(0xfe, () => Compare(ReadNextRomByte()));
        }
        
        public void Setup16bitAlu()
        {
            // 16-bit ALU
            AddInstruction(0x03, () => bc++);
            AddInstruction(0x09, () => Add(bc));
            AddInstruction(0x0b, () => bc--);

            AddInstruction(0x13, () => de++);
            AddInstruction(0x19, () => Add(de));
            AddInstruction(0x1b, () => de--);

            AddInstruction(0x23, () => hl++);
            AddInstruction(0x29, () => Add(hl));
            AddInstruction(0x2b, () => hl--);

            AddInstruction(0x33, () => sp++);
            AddInstruction(0x39, () => Add(sp));
            AddInstruction(0x3b, () => sp--);

            AddInstruction(0xe8, GetAddSpN());
        }

        public void SetupMisc()
        {
            // Misc
            AddInstruction(0x00, () => {});

            AddInstruction(0x10, GetStop());

            AddInstruction(0x27, GetDaa());
            AddInstruction(0x2f, GetCpl());

            AddInstruction(0x37, GetScf());
            AddInstruction(0x3f, GetCcf());

            // TODO Support extended operations?
            // AddInstruction(0x30, () => b = Swap(b));
            // AddInstruction(0x31, () => c = Swap(c));
            // AddInstruction(0x32, () => d = Swap(d));
            // AddInstruction(0x33, () => e = Swap(e));
            // AddInstruction(0x34, () => h = Swap(h));
            // AddInstruction(0x35, () => l = Swap(l));
            // AddInstruction(0x36, () => WriteByte(hl, Swap(ReadByte(hl))));
            // AddInstruction(0x37, () => a = Swap(a));

            AddInstruction(0x76, () => GetHalt());

            AddInstruction(0xf3, GetDi());
            AddInstruction(0xfb, GetEi());
        }
        
        public void SetupRotatesShifts()
        {
            // Rotates & Shifts
            AddInstruction(0x07, GetRlca());
            AddInstruction(0x0f, GetRrca());

            AddInstruction(0x17, GetRla());
            AddInstruction(0x1f, GetRra());
        }

        public void SetupBitOpcodes()
        {
            // Bit Opcodes
        }

        public void SetupJumps()
        {   
            // Jumps
            AddInstruction(0x18, () => pc += ReadNextRomByte());

            AddInstruction(0x20, () => { if (!IsFlagSet(FlagZero)) pc += ReadNextRomByte(); });
            AddInstruction(0x28, () => { if (IsFlagSet(FlagZero)) pc += ReadNextRomByte(); });

            AddInstruction(0x30, () => { if (!IsFlagSet(FlagCarry)) pc += ReadNextRomByte(); });
            AddInstruction(0x38, () => { if (IsFlagSet(FlagCarry)) pc += ReadNextRomByte(); });

            AddInstruction(0xc2, () => { if (!IsFlagSet(FlagZero)) pc = ReadNextRomShort(); });
            AddInstruction(0xc3, () => pc = ReadNextRomShort());
            AddInstruction(0xca, () => { if (IsFlagSet(FlagZero)) pc = ReadNextRomShort(); });

            AddInstruction(0xd2, () => { if (!IsFlagSet(FlagCarry)) pc = ReadNextRomShort(); });
            AddInstruction(0xda, () => { if (IsFlagSet(FlagCarry)) pc = ReadNextRomShort(); });

            AddInstruction(0xe9, () => pc = hl);
        }

        public void SetupCalls()
        {
            // Calls
            AddInstruction(0xc4, () => { if (!IsFlagSet(FlagZero)) pc = ReadNextRomShort(); });
            AddInstruction(0xcc, () => { if (IsFlagSet(FlagZero)) pc = ReadNextRomShort(); });
            AddInstruction(0xcd, () => { Push(pc); pc = ReadNextRomShort(); });

            AddInstruction(0xd4, () => { if (!IsFlagSet(FlagCarry)) pc = ReadNextRomShort(); });
            AddInstruction(0xdc, () => { if (IsFlagSet(FlagCarry)) pc = ReadNextRomShort(); });
        }

        public void SetupRestarts()
        {
            // Restarts
            AddInstruction(0xc7, () => Restart(0x0000));
            AddInstruction(0xcf, () => Restart(0x0008));

            AddInstruction(0xd7, () => Restart(0x0010));
            AddInstruction(0xdf, () => Restart(0x0018));

            AddInstruction(0xe7, () => Restart(0x0020));
            AddInstruction(0xef, () => Restart(0x0028));

            AddInstruction(0xf7, () => Restart(0x0030));
            AddInstruction(0xff, () => Restart(0x0038));
        }

        public void SetupReturns()
        {
            // Returns
            AddInstruction(0xc0, () => { if (!IsFlagSet(FlagZero)) Return(); });
            AddInstruction(0xc8, () => { if (IsFlagSet(FlagZero)) Return(); });
            AddInstruction(0xc9, () => Return());

            AddInstruction(0xd0, () => { if (!IsFlagSet(FlagCarry)) Return(); });
            AddInstruction(0xd8, () => { if (IsFlagSet(FlagCarry)) Return(); });
            AddInstruction(0xd9, () => { Return(); GetEi().Invoke(); }); // TODO Fix this
        }

        private void Add(byte val)
        {
            Add(a, val);
        }

        private void Adc(byte val)
        {
            Add(val, (byte)(IsFlagSet(FlagCarry) ? 1 : 0));
        }

        private void Add(byte val1, byte val2)
        {
            var temp = val1 + val2;
            ResetFlag(FlagNegative);

            if (temp == 0)
            {
                SetFlag(FlagZero);
            }
            else
            {
                ResetFlag(FlagZero);
            }

            if ((temp & 0xffff0000) != 0)
            {
                SetFlag(FlagCarry);
            }
            else
            {
                ResetFlag(FlagCarry);
            }

            if ((val1 & 0x0f) + (val2 & (0x0f)) > 0x0f)
            {
                SetFlag(FlagHalfCarry);
            }
            else
            {
                ResetFlag(FlagHalfCarry);
            }

            a = (byte)(temp & 0xff);
        }

        private void Sub(byte val)
        {
            Sub(a, val);
        }

        private void Subc(byte val)
        {
            Sub(a, (byte)(val + (byte)(IsFlagSet(FlagCarry) ? 1 : 0)));
        }

        private void Sub(byte val1, byte val2)
        {
            var temp = val1 - val2;
            SetFlag(FlagNegative);

            if (temp == 0)
            {
                SetFlag(FlagZero);
            }
            else
            {
                ResetFlag(FlagZero);
            }

            if (val2 > val1)
            {
                SetFlag(FlagCarry);
            }
            else
            {
                ResetFlag(FlagCarry);
            }

            if ((val2 & 0x0f) > (val1 & 0x0f))
            {
                SetFlag(FlagHalfCarry);
            }
            else
            {
                ResetFlag(FlagHalfCarry);
            }

            a = (byte)(temp & 0xff);
        }

        private void And(byte val)
        {
            a = (byte)(a & val);
            
            if (a == 0)
            {
                SetFlag(FlagZero);
            }
            else
            {
                ResetFlag(FlagZero);
            }

            ResetFlag(FlagNegative | FlagCarry);
            SetFlag(FlagHalfCarry);
        }

        private void Or(byte val)
        {
            a = (byte)(a | val);

            if (a == 0)
            {
                SetFlag(FlagZero);
            }
            else
            {
                ResetFlag(FlagZero);
            }

            ResetFlag(FlagNegative | FlagCarry | FlagHalfCarry); // OPTIMIZATION - reset all flags then just set zero if needed
        }

        private void Xor(byte val)
        {
            a = (byte)(a ^ val);

            if (a == 0)
            {
                SetFlag(FlagZero);
            }
            else
            {
                ResetFlag(FlagZero);
            }

            ResetFlag(FlagNegative | FlagCarry | FlagHalfCarry); // OPTIMIZATION - reset all flags then just set zero if needed
        }

        private void Compare(byte val)
        {
            if (a == val)
            {
                SetFlag(FlagZero);
            }
            else
            {
                ResetFlag(FlagZero);
            }

            SetFlag(FlagNegative);

            if (a < val)
            {
                SetFlag(FlagCarry);
            }
            else
            {
                ResetFlag(FlagCarry);
            }

            if ((a & 0x0f) > (val & 0x0f))
            {
                SetFlag(FlagHalfCarry);
            }
            else
            {
                ResetFlag(FlagHalfCarry);
            }
        }

        private byte Inc(byte val)
        {
            if ((val & 0x0f) == 0x0f)
            {
                SetFlag(FlagHalfCarry);
            }
            else
            {
                ResetFlag(FlagHalfCarry);
            }

            ResetFlag(FlagNegative);

            val++;

            if (val == 0)
            {
                SetFlag(FlagZero);
            }
            else
            {
                ResetFlag(FlagZero);
            }

            return val;
        }

        private byte Dec(byte val)
        {
            if ((val & 0x0f) == 0)
            {
                SetFlag(FlagHalfCarry);
            }
            else
            {
                ResetFlag(FlagHalfCarry);
            }

            SetFlag(FlagNegative);

            val--;

            if (val == 0)
            {
                SetFlag(FlagZero);
            }
            else
            {
                ResetFlag(FlagZero);
            }

            return val;
        }

        private void Add(ushort val)
        {
            var temp = hl + val;

            ResetFlag(FlagNegative);

            if ((temp & 0xffff0000) != 0)
            {
                SetFlag(FlagCarry);
            }
            else
            {
                ResetFlag(FlagCarry);
            }

            hl = (ushort)(temp & 0xffff);

            if ((hl & 0x0f) + (val & 0x0f) > 0x0f)
            {
                SetFlag(FlagHalfCarry);
            }
            else
            {
                ResetFlag(FlagHalfCarry);
            }
        }
        
        private Instruction GetSpPlusN()
        {
            return () => {
                // Use int here, so we can tell if the upper bits carried
                var operand = ReadNextRomByte();
                int temp = sp + operand;
                ResetFlag(FlagZero | FlagNegative);

                // If there is a value beyond the capacity of a ushort, then a carry occurred
                if ((temp & 0xffff0000) != 0)
                {
                    SetFlag(FlagCarry);
                } else {
                    ResetFlag(FlagCarry);
                }

                // Check for overflow of the lower bits
                if ((sp & 0x0f) + (operand & 0x0f) > 0x0f)
                {
                    SetFlag(FlagHalfCarry);
                } else {
                    ResetFlag(FlagHalfCarry);
                }

                // Convert the result back to a short
                hl = (ushort)temp;
            };
        }

        private Instruction GetAddSpN()
        {
            return () => {
                var operand = ReadNextRomByte();
                int temp = sp + operand;

                ResetFlag(FlagZero | FlagNegative);

                // If there is a value beyond the capacity of a ushort, then a carry occurred
                if ((temp & 0xffff0000) != 0)
                {
                    SetFlag(FlagCarry);
                } else {
                    ResetFlag(FlagCarry);
                }

                // Check for overflow of the lower bits
                if ((sp & 0x0f) + (operand & 0x0f) > 0x0f)
                {
                    SetFlag(FlagHalfCarry);
                } else {
                    ResetFlag(FlagHalfCarry);
                }

                sp = (ushort)(temp & 0x0ffff);
            };
        }

        private byte Swap(byte val)
        {
            var temp = (byte)((((val & 0x0f) << 4)) | (((val & 0xf0) >> 4)));

            if (temp == 0)
            {
                SetFlag(FlagZero);
            }
            else
            {
                ResetFlag(FlagZero);
            }

            ResetFlag(FlagNegative | FlagCarry | FlagHalfCarry); // OPTIMIZATION - reset all flags then just set zero if needed

            return temp;
        }

        // Thanks to https://github.com/CTurt/Cinoop/blob/master/source/cpu.c for this one!
        private Instruction GetDaa()
        {
            return () => {
                ushort temp = a; // Force this to an int

                if (IsFlagSet(FlagNegative))
                {
                    if (IsFlagSet(FlagHalfCarry)) temp = (byte)((temp - 0x06) & 0xff);
                    if (IsFlagSet(FlagCarry)) temp -= 0x60;
                }
                else
                {
                    if (IsFlagSet(FlagHalfCarry) || (temp & 0x0f) > 9) temp += 0x06;
                    if (IsFlagSet(FlagCarry) || temp > 0x9f) temp += 0x60;
                }

                a = (byte)(temp & 0xff);

                ResetFlag(FlagHalfCarry);

                if (a == 0)
                {
                    SetFlag(FlagZero);
                }
                else
                {
                    ResetFlag(FlagZero);
                }

                if (temp >= 0x100)
                {
                    SetFlag(FlagCarry);
                }
                else
                {
                    ResetFlag(FlagCarry);
                }
            };
        }

        private Instruction GetCpl()
        {
            return () => {
                a ^= a;

                SetFlag(FlagNegative | FlagHalfCarry);
            };
        }

        private Instruction GetCcf()
        {
            return () => {
                // OPTIMIZATION - Make a helper for this
                if (IsFlagSet(FlagCarry))
                {
                    ResetFlag(FlagCarry);
                }
                else
                {
                    SetFlag(FlagCarry);
                }

                ResetFlag(FlagNegative | FlagHalfCarry);
            };
        }

        private Instruction GetScf()
        {
            return () => {
                SetFlag(FlagCarry);
                ResetFlag(FlagNegative | FlagHalfCarry);
            };
        }

        private Instruction GetHalt()
        {
            return () => {
                // TODO Implement halting & interupts
            };
        }

        private Instruction GetStop()
        {
            return () => {
                // TODO Implement stop and buttons
            };
        }

        private Instruction GetDi()
        {
            return () => {
                // TODO Implement disable interupts
            };
        }

        private Instruction GetEi()
        {
            return () => {
                // TODO Implement enable interupts
            };
        }

        private Instruction GetRlca()
        {
            return () => {
                var temp = (byte)((a & 0x80) >> 7);
                a <<= 1;
                a += temp;

                if (a == 0)
                {
                    SetFlag(FlagZero);
                }
                else
                {
                    ResetFlag(FlagZero);
                }

                if (temp == 0)
                {
                    ResetFlag(FlagCarry);
                }
                else
                {
                    SetFlag(FlagCarry);
                }

                ResetFlag(FlagNegative | FlagHalfCarry); // OPTIMIZATION - Always rest all flags?
            };
        }

        private Instruction GetRla()
        {
            return () => {
                var temp = (byte)(IsFlagSet(FlagCarry) ? 1 : 0);

                if ((a & 0x80) == 0)
                {
                    ResetFlag(FlagCarry);
                }
                else
                {
                    SetFlag(FlagCarry);
                }

                a <<= 1;
                a += temp;

                if (a == 0)
                {
                    SetFlag(FlagZero);
                }
                else
                {
                    ResetFlag(FlagZero);
                }

                ResetFlag(FlagNegative | FlagHalfCarry); // OPTIMIZATION - Always rest all flags?
            };
        }

        private void Restart(ushort pos)
        {
            Push(pc);
            pc = pos;
        }

        private void Return()
        {
            var pos = Pop();
            pc = pos;
        }

        private Instruction GetRrca()
        {
            return () => {
                var temp = (byte)(a & 0x01);

                if (temp == 0)
                {
                    ResetFlag(FlagCarry);
                }
                else
                {
                    SetFlag(FlagCarry);
                }

                a >>= 1;
                if (temp != 0) a |= 0x80;

                if (a == 0)
                {
                    SetFlag(FlagZero);
                }
                else
                {
                    ResetFlag(FlagZero);
                }

                ResetFlag(FlagNegative | FlagHalfCarry); // OPTIMIZATION - Always rest all flags?
            };
        }

        private Instruction GetRra()
        {
            return () => {
                var temp = (byte)(IsFlagSet(FlagCarry) ? 1 : 0) << 7;

                if ((a & 0x01) == 0)
                {
                    ResetFlag(FlagCarry);
                }
                else
                {
                    SetFlag(FlagCarry);
                }

                a >>= 1;
                if (temp != 0) a |= 0x80;

                if (a == 0)
                {
                    SetFlag(FlagZero);
                }
                else
                {
                    ResetFlag(FlagZero);
                }

                ResetFlag(FlagNegative | FlagHalfCarry); // OPTIMIZATION - Always rest all flags?
            };
        }

        private void AddInstruction(ushort opCode, Instruction action)
        {
            Instructions.Add(opCode, action);
        }
    }
}