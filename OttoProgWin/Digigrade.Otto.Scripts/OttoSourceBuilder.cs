using Digigrade.Otto.Scripts.Debug;

namespace Digigrade.Otto.Scripts
{
    /// <summary>
    /// The main class that generates files from scripts.
    /// </summary>
    public class OttoSourceBuilder
    {
        /// <summary>
        /// Compiles and returns a list of source files from scripts.  Returns null if errors are found during compiliation.
        /// </summary>
        public static List<OttoSourceCodeFile>? Build(List<OttoScript> scripts, out List<DebugResult> debugResults)
        {
            var returnValue = new List<OttoSourceCodeFile>();
            debugResults = new List<DebugResult>();

            CheckScripts(scripts, out List<DebugResult> checkDebugResults);
            debugResults.AddRange(checkDebugResults);

            if (CanBuild(checkDebugResults))
            {
                debugResults.Add(new DebugResult()
                {
                    ResultType = ResultType.Info,
                    Title = "Ready to Build",
                    Message = "Checked all scripts and found no errors.",
                });

                // Create files from Scripts
            }

            return returnValue;
        }

        /// <summary>
        /// Returns true if no debug errors are found.
        /// </summary>
        /// <returns></returns>
        public static bool CanBuild(List<OttoScript> scripts)
        {
            CheckScripts(scripts, out List<DebugResult> debugResults);
            return CanBuild(debugResults);
        }

        /// <summary>
        /// Checks all the scripts and outs debug results.
        /// </summary>
        /// <param name="scripts"></param>
        /// <param name="debugResults"></param>
        public static void CheckScripts(List<OttoScript> scripts, out List<DebugResult> debugResults)
        {
            debugResults = new List<DebugResult>();
            foreach (var item in scripts)
            {
                item.CheckScript(out List<DebugResult> scriptCheckDebugResults);
                debugResults.AddRange(scriptCheckDebugResults);

                if (item.IsIgnored)
                {
                    debugResults.Add(new DebugResult()
                    {
                        Originator = item,
                        ResultType = ResultType.Warning,
                        Title = "Script is ignored",
                        Message = item.ReasonIgnored,
                    });
                }
                else if (!item.IsValid)
                {
                    debugResults.Add(new DebugResult()
                    {
                        Originator = item,
                        ResultType = ResultType.Error,
                        Title = "Script is invalid",
                        Message = item.ReasonInvalid,
                    });
                }
            }
        }

        /// <summary>
        /// Returns true if no debug errors are found.
        /// </summary>
        /// <returns></returns>
        private static bool CanBuild(List<DebugResult> debugResults)
        {
            foreach (var item in debugResults)
            {
                if (item.ResultType == ResultType.Error)
                {
                    return false;
                }
            }

            return true;
        }
    }
}