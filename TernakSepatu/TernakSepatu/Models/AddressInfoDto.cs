using System.ComponentModel.DataAnnotations;

namespace TernakSepatu.Models
{
    public class AddressInfoDto
    {
        public int Id { get; set; }

        public string street { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        public string postal_code { get; set; }

        public string country { get; set; }
    }
}
