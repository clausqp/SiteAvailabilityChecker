using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{

    /// <summary>
    /// Shared message format
    /// </summary>
    [Serializable]  // mandatory
    public class Message
    {
        public DateTime Timestamp;
        
        public int SerialNumber;

        public StatusValues Status;

        public string StatusMessage;
    }


}
