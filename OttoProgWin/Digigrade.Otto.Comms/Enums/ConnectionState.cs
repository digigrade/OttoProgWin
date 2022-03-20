using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digigrade.Otto.Comms
{
    public enum ConnectionState
    {
        Offline = 0,
        Failed = 1,
        Busy = 2,
        Connecting = 3,
        ProgramMode = 4,
        NormalMode = 5,
    }
}
