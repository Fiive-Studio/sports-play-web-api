namespace FutbolPlay
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("opening")]
    public partial class opening
    {
        [Key]
        public int id_opening { get; set; }

        public int id_day { get; set; }

        public TimeSpan hour_start { get; set; }

        public TimeSpan hour_end { get; set; }

        public int id_place { get; set; }
    }
}
