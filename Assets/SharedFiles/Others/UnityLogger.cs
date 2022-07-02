using NetSockets.Interfaces;
using System.Linq;
using UnityEngine;

namespace Bomberman.SharedFiles.Others
{
    public class UnityLogger : NetSockets.Interfaces.ILogger
    {
        public void WriteLine(string msg, string stackTrace = null, params LogLevel[] logLevels)
        {
            var prefix = logLevels != null && logLevels.Length > 0 ?
                    ("{" + string.Join(",", logLevels) + "}: ") : "";
            if (logLevels.Any(a => a == LogLevel.Exception))
                Debug.LogError(prefix + msg + (stackTrace != null ? ("\n" + stackTrace) : ""));
            else
                Debug.Log(prefix + msg + (stackTrace != null ? ("\n" + stackTrace) : ""));
        }
    }
}
