using System;
using CPUx86 = Cosmos.Assembler.x86;
using Cosmos.Assembler.x86;
using Cosmos.Assembler.x86.SSE;
using XSharp.Compiler;

namespace Cosmos.IL2CPU.X86.IL
{
    [Cosmos.IL2CPU.OpCode( ILOpCode.Code.Add )]
    public class Add : ILOp
    {
        public Add( Cosmos.Assembler.Assembler aAsmblr )
            : base( aAsmblr )
        {
        }

        public override void Execute( MethodInfo aMethod, ILOpCode aOpCode )
        {
            var xType = aOpCode.StackPopTypes[0];
            var xSize = SizeOfType(xType);
            var xIsFloat = TypeIsFloat(xType);
            DoExecute(xSize, xIsFloat);
        }

      public static void DoExecute(uint xSize, bool xIsFloat)
      {
        if (xSize > 8)
        {
          //EmitNotImplementedException( Assembler, aServiceProvider, "Size '" + xSize.Size + "' not supported (add)", aCurrentLabel, aCurrentMethodInfo, aCurrentOffset, aNextLabel );
          throw new NotImplementedException("Cosmos.IL2CPU.x86->IL->Add.cs->Error: StackSize > 8 not supported");
        }
        else
        {
          if (xSize > 4)
          {
            if (xIsFloat)
            {
              new CPUx86.x87.FloatLoad { DestinationReg = RegistersEnum.ESP, Size = 64, DestinationIsIndirect = true };
              XS.Add(XSRegisters.ESP, 8);
              new CPUx86.x87.FloatAdd { DestinationReg = CPUx86.RegistersEnum.ESP, DestinationIsIndirect = true, Size = 64 };
              new CPUx86.x87.FloatStoreAndPop { DestinationReg = RegistersEnum.ESP, Size = 64, DestinationIsIndirect = true };
            }
            else
            {
              XS.Pop(XSRegisters.OldToNewRegister(RegistersEnum.EDX)); // low part
              XS.Pop(XSRegisters.OldToNewRegister(RegistersEnum.EAX)); // high part
              XS.Add(XSRegisters.ESP, XSRegisters.EDX, destinationIsIndirect: true);
              XS.AddWithCarry(XSRegisters.ESP, XSRegisters.EAX, destinationDisplacement: 4);
            }
          }
          else
          {
            if (xIsFloat) //float
            {
              XS.SSE.MoveSS(XSRegisters.XMM0, XSRegisters.ESP, sourceIsIndirect: true);
              XS.Add(XSRegisters.OldToNewRegister(RegistersEnum.ESP), 4);
              XS.SSE.MoveSS(XSRegisters.XMM1, XSRegisters.ESP, sourceIsIndirect: true);
              XS.SSE.AddSS(XSRegisters.XMM0, XSRegisters.XMM1);
              XS.SSE.MoveSS(XSRegisters.XMM1, XSRegisters.ESP, sourceIsIndirect: true);
            }
            else //integer
            {
              XS.Pop(XSRegisters.OldToNewRegister(RegistersEnum.EAX));
              XS.Add(XSRegisters.ESP, XSRegisters.EAX, destinationIsIndirect: true);
            }
          }
        }
      }
    }
}
