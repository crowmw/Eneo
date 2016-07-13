using System.Text;

namespace Eneo.Helpers
{
    public static class StringHelper
    {
        public static string RemovePolishLetters(string value)
        {
            value = value.Replace(" ", "");
            byte[] tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(value);
            string asciiStr = Encoding.UTF8.GetString(tempBytes);
            return asciiStr;
        }
    }
}