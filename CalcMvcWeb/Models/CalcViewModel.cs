using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CalcMvcWeb.Models
{
    public class CalcViewModel
    {
        public string Expression { get; set; } = string.Empty;
        public string Result { get; set; } = "0";
        public double Memory { get; set; } = 0;
    }

}
