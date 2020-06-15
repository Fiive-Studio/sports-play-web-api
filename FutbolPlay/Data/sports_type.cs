namespace FutbolPlay
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class sports_type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sports_type()
        {
            pitch = new HashSet<pitch>();
        }

        [Key]
        public int id_sport_type { get; set; }

        [Required]
        [StringLength(100)]
        public string name { get; set; }

        [StringLength(350)]
        public string description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pitch> pitch { get; set; }
    }
}
