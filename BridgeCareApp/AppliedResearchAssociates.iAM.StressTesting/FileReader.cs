using System;
using System.IO;

namespace AppliedResearchAssociates.iAM.StressTesting
{
    public class FileReader
    {
        public static string ReadAllTextInGitIgnoredFile(string fileName)
        {
            var directory = Directory.GetCurrentDirectory();
            var folder = Path.Combine(directory, "GitIgnored");
            var file = Path.Combine(folder, fileName);
            if (File.Exists(file))
            {
                var text = File.ReadAllText(file);
                return text;
            } else
            {
                var message = $"{file} does not exist!";
                throw new Exception(message);
            }
        }

        public static void WriteTextToGitIgnoredFile(string filename, string text)
        {
            var directory = Directory.GetCurrentDirectory();
            var folder = Path.Combine(directory, "GitIgnored");
            var file = Path.Combine(folder, filename);
            File.Delete(file);
            File.WriteAllText(file, text);
        }
    }
}
