namespace FutbolPlay
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("pitch")]
    public partial class pitch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public pitch()
        {
            reservation = new HashSet<reservation>();
        }

        [Key]
        public int id_pitch { get; set; }

        public int? id_place { get; set; }

        [StringLength(50)]
        public string description { get; set; }

        public bool? status { get; set; }

        public int? id_pitch_type { get; set; }

        public int? id_sport_type { get; set; }

        public virtual sports_type sports_type { get; set; }

        public virtual place place { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<reservation> reservation { get; set; }
    }
}
