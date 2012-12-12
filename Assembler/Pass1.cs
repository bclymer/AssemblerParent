
namespace Assembler
{
    class Pass1
    {
        public bool Success { get; set; }
        public int ErrorLine { get; set; }
        public string ErrorDescription { get; set; }

        public Pass1()
        {
            Success = true;
        }
    }
}
