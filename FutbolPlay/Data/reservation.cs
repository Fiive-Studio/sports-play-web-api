namespace FutbolPlay
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("reservation")]
    public partial class reservation
    {
        [Key]
        public int id_reservation { get; set; }

        public int id_pitch { get; set; }

        public int id_place { get; set; }

        public int? id_users { get; set; }

        public int? id_users_offline { get; set; }

        public int id_scheduler { get; set; }

        public TimeSpan hour { get; set; }

        public DateTime date { get; set; }

        public int status { get; set; }

        public DateTime date_insert { get; set; }

        public int? id_users_type { get; set; }

        public decimal? value { get; set; }

        public string description { get; set; }

        public virtual pitch pitch { get; set; }

        public virtual place place { get; set; }
    }
}
