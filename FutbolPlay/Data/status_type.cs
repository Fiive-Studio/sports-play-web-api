namespace FutbolPlay
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class status_type
    {
        [Key]
        public int id_status { get; set; }

        [Required]
        [StringLength(30)]
        public string name { get; set; }
    }
}
