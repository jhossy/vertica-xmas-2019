using System;
using System.IO;

namespace Xmas2019.Library.Infrastructure
{
    public class FileReader
    {
        public string Read(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath)) return string.Empty;

            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return string.Empty;
        }
    }
}
