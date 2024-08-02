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

        public string PatientIdCardRx { get; set; }
        public  string PatientIdCardRxError { get; set; }

    }
}
