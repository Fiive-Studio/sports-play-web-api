namespace FutbolPlay
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("scheduler")]
    public partial class scheduler
    {
        [Key]
        public int id_schedule { get; set; }

        public TimeSpan hour { get; set; }

        public decimal value { get; set; }

        public int id_day { get; set; }

        public int id_pitch { get; set; }

        public int id_pitch_type { get; set; }

        public DateTime date_insert { get; set; }
    }
}
