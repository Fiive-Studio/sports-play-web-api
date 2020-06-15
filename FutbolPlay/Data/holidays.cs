namespace FutbolPlay
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class holidays
    {
        [Key]
        [Column(Order = 0)]
        public int id_holiday { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime date { get; set; }
    }
}
