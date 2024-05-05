using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TernakSepatu.Models
{
    [Table("address_info")] // Sesuaikan dengan nama tabel di database

    public class AddressInfo
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string street { get; set; }

        [MaxLength(100)]
        public string city { get; set; }

        [MaxLength(100)]
        public string state { get; set; }

        [MaxLength(20)]
        public string postal_code { get; set; }

        [MaxLength(100)]
        public string country { get; set; }

        // Properti user_id
        [MaxLength(450)] // Panjang maksimum ID pengguna
        public string user_id { get; set; }
    }
}
