using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicMacro.Exceptions
{
    class LoopIdentationException : Exception
    {

        public LoopIdentationException(string context = "") : base($"Could not parse the loop identation : {context}")
        {
        }

    }
}
