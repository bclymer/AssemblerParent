using System;
using System.Linq;

namespace Assembler
{
    class Arg
    {
        public ArgType Type;
        public int Value;
        public int OutsideValue;
        public string Label;
        public bool IsLabel;

        public Arg(string arg, bool containsLabel)
        {
            if (arg.Contains('('))
            {
                Type = ArgType.HasParaenthesis;
                var args = arg.Split('(');
                args[0] = args[0].Replace("(", "");
                args[0] = args[0].Replace(")", "");
                args[0] = args[0].Replace("$", "");
                args[1] = args[1].Replace("(", "");
                args[1] = args[1].Replace(")", "");
                args[1] = args[1].Replace("$", "");
                OutsideValue = Int32.Parse(args[0]);
                Value = Int32.Parse(args[1]);
            }
            else if (arg.Contains('$'))
            {
                Type = ArgType.HasDollarSign;
                arg = arg.Replace("$", "");
                Value = Int32.Parse(arg);
            }
            else
            {
                if (!containsLabel)
                {
                    Type = ArgType.JustAValue;
                    Value = Int32.Parse(arg);
                }
                else
                {
                    Label = arg;
                    IsLabel = true;
                }
            }
            if (!IsLabel && (Value > 7 || OutsideValue > 63))
            {
                throw new ValueTooLargeException();
            }
        }

        internal class ValueTooLargeException : Exception
        {
        }
    }
}
