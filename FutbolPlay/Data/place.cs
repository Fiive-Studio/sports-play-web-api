namespace FutbolPlay
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("place")]
    public partial class place
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public place()
        {
            pitch = new HashSet<pitch>();
            reservation = new HashSet<reservation>();
        }

        [Key]
        public int id_place { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        [StringLength(50)]
        public string phone { get; set; }

        [StringLength(200)]
        public string description { get; set; }

        [StringLength(50)]
        public string address { get; set; }

        public bool? status { get; set; }

        [StringLength(20)]
        public string latitude { get; set; }

        [StringLength(20)]
        public string longitude { get; set; }

        public int? max_days_reservation { get; set; }

        public bool? autoconfirm { get; set; }

        [StringLength(2)]
        public string format_hour { get; set; }

        public int? max_time_cancelation { get; set; }

        public DateTime? end_date_test { get; set; }

        [StringLength(50)]
        public string profile_img { get; set; }

        public int? id_city { get; set; }

        public virtual cities cities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<pitch> pitch { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<reservation> reservation { get; set; }
    }
}
