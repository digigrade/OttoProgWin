using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digigrade.Otto.Comms.Commands
{
    public class CommandHeaders
    {
        public const string BeginProgramMode = "{FF958AAA-A619-4AE9-B2D4-F7A4804BCE20}";
        public const string BeginNormalMode = "{4773D560-86BA-4530-BED5-755C8FF44C0F}";
        public const string GetSettings = "{87B4F95D-DDF1-4814-9454-44FC550C321B}";
        public const string SaveSettings = "{D52C4C8C-103F-4619-94CE-F273F587D209}";
        public const string ClearScripts = "{292F618E-0752-4926-8B3D-772CEE469F54}";
        public const string AddScript = "{B37E8FC4-8D97-4539-ABAC-FCCFCAAD275F}";
        public const string ForceReset = "{C7F1C4CD-D574-4338-9A15-1D4F08F3E099}";
    }
}
