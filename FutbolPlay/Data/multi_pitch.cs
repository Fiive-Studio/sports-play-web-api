namespace FutbolPlay
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class multi_pitch
    {
        [Key]
        public int id_multi_pitch { get; set; }

        public int? id_pitch_multiple { get; set; }

        public int? id_pitch_single { get; set; }

        public int? id_place { get; set; }

        [StringLength(50)]
        public string description { get; set; }

        public bool? status { get; set; }
    }
}
