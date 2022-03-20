using Digigrade.Otto.Scripts.Components;
using Digigrade.Otto.Scripts.Debug;

namespace Digigrade.Otto.Scripts
{
    public class OttoScript
    {
        public string Comment { get; set; } = string.Empty;
        public List<ScriptEvent> Events { get; set; } = new List<ScriptEvent>();
        public Guid Id { get; } = Guid.NewGuid();
        public bool IsIgnored { get; set; } = false;
        public bool IsValid { get; set; } = true;
        public string Name { get; set; } = string.Empty;
        public string ReasonIgnored { get; set; } = string.Empty;
        public string ReasonInvalid { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;

        public void CheckScript(out List<DebugResult> results)
        {
            results = new List<DebugResult>();
            foreach (var scriptEvent in Events)
            {
            }
        }
    }
}