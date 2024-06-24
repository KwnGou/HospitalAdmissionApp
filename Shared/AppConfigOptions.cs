using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAdmissionApp.Shared
{
    public class AppConfigOptions
    {
        public const string AppConfigOptionsSection = "AppConfigOptions";

        public string PatientIdRx { get; set; }
        public  string PatientIdRxError { get; set; }

    }
}
