using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteAvailabilityChecker
{
    public class CurrentState
    {


        public static Core.Message StatusMessage;

        static CurrentState()
        {
            StatusMessage = new Core.Message();
            StatusMessage.Timestamp = DateTime.Now;
        }


        public static void UpdateState( StatusValues siteProblem, string siteProblemDetail )
        {
            StatusMessage.SerialNumber++;
            StatusMessage.Timestamp = DateTime.Now;
            StatusMessage.Status = siteProblem;
            StatusMessage.StatusDescription = siteProblemDetail;
        }
    }
}
