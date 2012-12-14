using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Assembler
{
    class Program
    {
        private static List<string> _inputLines;
        private static string _outputText;
        public static Dictionary<int, string> Labels;

        static void Main(string[] args)
        {
            Labels = new Dictionary<int, string>();
            if (args.Length != 1 || !File.Exists(args[0]))
            {
                Console.WriteLine("Please enter only 1 parameter, a valid input file to compile.");
                return;
            }
            _inputLines = new List<string>();
            var sr = new StreamReader(args[0]);
            while (!sr.EndOfStream)
            {
                _inputLines.Add(sr.ReadLine());
            }
            sr.Close();
            var pass1 = Pass1();
            if (!pass1.Success)
            {
                Console.WriteLine("Syntax errors in input file on line " + pass1.ErrorLine);
                Console.WriteLine(pass1.ErrorDescription);
                Console.WriteLine("Press Enter to Exit...");
                Console.Read();
                return;
            }
            var pass2 = Pass2();
            if (!pass2.Success)
            {
                Console.WriteLine("Couldn't compile your code, error on line " + pass2.ErrorLine);
                Console.WriteLine(pass2.ErrorDescription);
                Console.Read();
                Console.WriteLine("Press Enter to Exit...");
                return;
            }
            Console.WriteLine("Compiled successfully with no errors. Any warnings are be displayed above.");
            Console.WriteLine("Press enter to exit...");
            WriteOutput(args[0].Split('.').ToList().First());
            Console.Read();
        }

        static Pass1 Pass1()
        {
            var pass1 = new Pass1();
            for (var i = 0; i < _inputLines.Count; i++)
            {
                var line = _inputLines[i].Trim(' ', '\t');
                if (line.Count(f => f == '(') != line.Count(f => f == ')'))
                {
                    pass1.Success = false;
                    pass1.ErrorLine = i + 1;
                    pass1.ErrorDescription = "Mismatched parentheses";
                    return pass1;
                }
                var containsLabel = line.Contains(":");
                if (!containsLabel) continue;
                if (i > 63)
                {
                    Console.WriteLine("Label " + line.Substring(0, line.IndexOf(':')) + " is on line " + i +
                                      " which is too high to skip to. It must be below line 64");
                }
                Labels.Add(i, line.Substring(0, line.IndexOf(':')));
            }
            return pass1;
        }

        static Pass2 Pass2()
        {
            var pass2 = new Pass2();
            _outputText += "DEPTH = " + Depth() + ";\n";
            _outputText += "WIDTH = " + Width() + ";\n";
            _outputText += "ADDRESS_RADIX = " + AddressRadix() + ";\n";
            _outputText += "DATA_RADIX = " + DataRadix() + ";\n";
            _outputText += "\nCONTENT\n\tBEGIN\n";
            _outputText += "\t[0..255]\t:\t0000000000000000;\n";

            for (var i = 0; i < _inputLines.Count; i++)
            {
                var formattedLine = _inputLines[i];
                if (Labels.ContainsKey(i))
                {
                    formattedLine = _inputLines[i].Replace(Labels[i] + ":", "");
                }
                try
                {
                    var line = new Line(formattedLine.Trim(' ', '\t'));
                    _outputText += "\t" + i + "\t:\t";
                    _outputText += DecToBinary(line.OpCode).PadLeft(4, '0');
                    switch (line.Format)
                    {
                        case Format.R:
                            _outputText += line.GetRd(i + 1);
                            if (line.Type.Equals("jr"))
                            {
                                _outputText += "000000000";
                            }
                            else
                            {
                                _outputText += line.GetRs(i + 1);
                                _outputText += line.GetRt(i + 1);
                                _outputText += DecToBinary(line.AluCode).PadLeft(3, '0');
                            }
                            break;
                        case Format.I:
                            _outputText += line.GetRd(i + 1);
                            _outputText += line.GetRs(i + 1);
                            _outputText += line.GetImmediate(i + 1);
                            break;
                        case Format.J:
                            _outputText += "000000"; // unused stuff
                            _outputText += line.GetImmediate(i + 1);
                            break;
                    }
                    _outputText += ";\n";
                }
                catch (Exception e)
                {
                    pass2.ErrorLine = i + 1;
                    pass2.ErrorDescription = e.Message;
                    pass2.Success = false;
                    return pass2;
                }
            }

            _outputText += "END;";
            return pass2;
        }

        public static string Depth()
        {
            return "256";
        }

        public static string Width()
        {
            return "16";
        }

        public static string AddressRadix()
        {
            return "DEC";
        }

        public static string DataRadix()
        {
            return "BIN";
        }

        public static string DecToBinary(string dec)
        {
            var value = Int32.Parse(dec);
            return Convert.ToString(value, 2);
        }

        public static string DecToBinary(int dec)
        {
            return DecToBinary(dec.ToString(CultureInfo.InvariantCulture));
        }

        public static string DecToHex(int dec)
        {
            var hex = Convert.ToString(dec, 16);
            return hex.PadLeft(2, '0');
        }

        public static string DecToHex(string dec)
        {
            return DecToHex(Int32.Parse(dec));
        }

        public static string HexToBinary(string hex)
        {
            return DecToBinary(Convert.ToInt32(hex, 16).ToString(CultureInfo.InvariantCulture));
        }

        public static void WriteOutput(String fileName)
        {
            var file = new StreamWriter(fileName + ".MIF");
            file.WriteLine(_outputText);
            file.Close();
        }
    }
}
