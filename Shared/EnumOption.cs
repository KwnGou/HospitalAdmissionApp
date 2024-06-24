using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAdmissionApp.Shared
{
    public class EnumOption
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public const string SexOptionsString = "SexOptions";
        public const int Male = 1;
        public const int Female = 2;
        public const int Other = 3;

        public const string InsuranceOptionsString = "InsuranceOptions";
        public const int FirstClass = 1;
        public const int SecondClass = 2;
        public const int ThirdClass = 3;
    }
}
