namespace FutbolPlay
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class reservation_multipitch
    {
        [Key]
        public int id_reservation_multipitch { get; set; }

        public int? id_pitch_multiple { get; set; }

        public int? id_pitch_single { get; set; }

        public int? id_place { get; set; }

        public int? id_reservation { get; set; }

        public TimeSpan hour { get; set; }

        public DateTime date { get; set; }

        public int? status { get; set; }

        public string type_pitch_insert { get; set; }
    }
}
