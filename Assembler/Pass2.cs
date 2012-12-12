
namespace Assembler
{
    class Pass2
    {
        public bool Success { get; set; }
        public int ErrorLine { get; set; }
        public string ErrorDescription { get; set; }

        public Pass2()
        {
            Success = true;
        }
    }
}
