using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooglePlaces.Models.Options
{
    public class EmailOptions
    {
        public string SmtpUsername { get; set; }

        public string SmtpPassword { get; set; }

        public int Port { get; set; }

        public string  SmtpServer { get; set; }

        public string SmtpMailTarget { get; set; }
    }
}
