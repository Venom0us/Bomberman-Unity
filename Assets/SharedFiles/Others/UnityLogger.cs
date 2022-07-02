using NetSockets.Interfaces;
using System.Linq;
using UnityEngine;

namespace Bomberman.SharedFiles.Others
{
    public class UnityLogger : NetSockets.Interfaces.ILogger
    {
        public void WriteLine(string msg, params LogLevel[] logLevels)
        {
            if (logLevels.Any(a => a == LogLevel.Exception))
                Debug.LogError(msg);
            else
                Debug.Log(msg);
        }
    }
}
