using System.Text;

namespace Digigrade.Otto.Scripts
{
    public class OttoSourceCodeFile
    {
        public string FileName { get; set; } = string.Empty;
        public List<string> Lines { get; set; } = new List<string>();
        public string Content
        {
            get { return GenerateFileContent(); }
        }

        private string GenerateFileContent()
        {
            var returnValue = new StringBuilder();

            foreach(var line in Lines)
            {
                returnValue.AppendLine(line);
            }

            return returnValue.ToString();
        }
    }
}
