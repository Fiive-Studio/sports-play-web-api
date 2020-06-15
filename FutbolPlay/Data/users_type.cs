namespace FutbolPlay
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class users_type
    {
        [Key]
        public int id_users_type { get; set; }

        [StringLength(50)]
        public string name { get; set; }
    }
}
