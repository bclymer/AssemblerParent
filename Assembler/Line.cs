using System;
using System.Collections.Generic;
using System.Linq;

namespace Assembler
{
    class Line
    {
        public List<Arg> Args;
        public string Type;
        public Format Format;
        public string OpCode;
        public string AluCode;

        public Line(string line)
        {
            line = line.Trim(' ', '\t');
            var parts = line.Split(' ').ToList();
            if (parts.Count > 2)
            {
                throw new MoreThanTwoPartsException();
            }
            Type = parts[0];
            switch (Type)
            {
                case "and":
                    Format = Format.R;
                    OpCode = "0";
                    AluCode = "0";
                    break;
                case "or":
                    Format = Format.R;
                    OpCode = "0";
                    AluCode = "1";
                    break;
                case "xor":
                    Format = Format.R;
                    OpCode = "0";
                    AluCode = "2";
                    break;
                case "sll":
                    Format = Format.R;
                    OpCode = "0";
                    AluCode = "3";
                    break;
                case "srl":
                    Format = Format.R;
                    OpCode = "0";
                    AluCode = "4";
                    break;
                case "add":
                    Format = Format.R;
                    OpCode = "0";
                    AluCode = "5";
                    break;
                case "sub":
                    Format = Format.R;
                    OpCode = "0";
                    AluCode = "6";
                    break;
                case "addiu":
                    Format = Format.I;
                    OpCode = "1";
                    break;
                case "subiu":
                    Format = Format.I;
                    OpCode = "2";
                    break;
                case "addi":
                    Format = Format.I;
                    OpCode = "3";
                    break;
                case "subi":
                    Format = Format.I;
                    OpCode = "4";
                    break;
                case "j":
                    Format = Format.J;
                    OpCode = "5";
                    break;
                case "jr":
                    Format = Format.R;
                    OpCode = "6";
                    break;
                case "jal":
                    Format = Format.J;
                    OpCode = "7";
                    break;
                case "beq":
                    Format = Format.I;
                    OpCode = "8";
                    break;
                case "bne":
                    Format = Format.I;
                    OpCode = "9";
                    break;
                case "slt":
                    Format = Format.R;
                    OpCode = "10";
                    break;
                case "lw":
                    Format = Format.I;
                    OpCode = "11";
                    break;
                case "sw":
                    Format = Format.I;
                    OpCode = "12";
                    break;
                default:
                    throw new OperationNotFoundException();
            }
            Args = new List<Arg>();
            foreach (var s in parts[1].Split(','))
            {
                var containsLabel = (Type.Equals("bne") || Type.Equals("beq") || Type.Equals("j"));
                Args.Add(new Arg(s, containsLabel));
            }
        }

        public string GetRd()
        {
            if (Args[0].Type == ArgType.HasDollarSign && Args[0].Value == 0 && !Type.Equals("jr"))
                Console.WriteLine("Program attempts to write to register 0, this will fail during execution!!!");
            switch (Args[0].Type)
            {
                case ArgType.HasDollarSign:
                    return Program.DecToBinary(Args[0].Value).PadLeft(3, '0');
                case ArgType.HasParaenthesis:
                    return Program.DecToBinary(Args[0].Value).PadLeft(3, '0');
                case ArgType.JustAValue:
                    return Program.DecToBinary(Args[0].Value).PadLeft(3, '0');
            }
            throw new ArgRdTypeNotSetException();
        }

        public string GetRs()
        {
            switch (Args[1].Type)
            {
                case ArgType.HasDollarSign:
                    return Program.DecToBinary(Args[1].Value).PadLeft(3, '0');
                case ArgType.HasParaenthesis:
                    return Program.DecToBinary(Args[1].Value).PadLeft(3, '0');
                case ArgType.JustAValue:
                    return Program.DecToBinary(Args[1].Value).PadLeft(3, '0');
            }
            throw new ArgRsTypeNotSetException();
        }

        public string GetRt()
        {
            switch (Args[2].Type)
            {
                case ArgType.HasDollarSign:
                    return Program.DecToBinary(Args[2].Value).PadLeft(3, '0');
                case ArgType.HasParaenthesis:
                    return Program.DecToBinary(Args[2].Value).PadLeft(3, '0');
                case ArgType.JustAValue:
                    return Program.DecToBinary(Args[2].Value).PadLeft(3, '0');
            }
            throw new ArgRtTypeNotSetException();
        }

        public string GetImmediate()
        {
            KeyValuePair<int, string> lineOfLabel;
            switch (Format)
            {
                case Format.I:
                    switch (Type)
                    {
                        case "bne":
                        case "beq":
                            lineOfLabel = Program.Labels.FirstOrDefault(x => x.Value.Equals(Args[2].Label));
                            return Program.DecToBinary(lineOfLabel.Key).PadLeft(6, '0');
                        case "lw":
                        case "sw":
                            return Program.DecToBinary(Args[1].OutsideValue).PadLeft(6, '0');
                        default:
                            return Program.DecToBinary(Args[2].Value).PadLeft(6, '0');
                    }
                case Format.J:
                    switch (Type)
                    {
                        case "j":
                            lineOfLabel = Program.Labels.FirstOrDefault(x => x.Value.Equals(Args[0].Label));
                            return Program.DecToBinary(lineOfLabel.Key).PadLeft(6, '0');
                        default:
                            return "000000";
                    }
            }
            throw new FormatDoesntUseImmediateException();
        }

        internal class FormatDoesntUseImmediateException : Exception
        {
        }

        internal class MoreThanTwoPartsException : Exception
        {
        }

        internal class ArgRtTypeNotSetException : Exception
        {
        }

        internal class ArgRdTypeNotSetException : Exception
        {
        }

        internal class ArgRsTypeNotSetException : Exception
        {
        }

        internal class OperationNotFoundException : Exception
        {
        }
    }
}
