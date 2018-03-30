//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CodeAcademyAttendanceSystemAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Groups
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Groups()
        {
            this.Qr_Codes = new HashSet<Qr_Codes>();
            this.Students = new HashSet<Students>();
        }
    
        public int group_id { get; set; }
        public string group_name { get; set; }
        public Nullable<System.DateTime> group_start_date { get; set; }
        public Nullable<System.DateTime> group_end_date { get; set; }
        public Nullable<int> group_lesson_times_id { get; set; }
        public Nullable<int> group_teacher_id { get; set; }
        public Nullable<int> group_group_type_id { get; set; }
        public Nullable<bool> group_status { get; set; }
    
        public virtual Group_Types Group_Types { get; set; }
        public virtual Lesson_Times Lesson_Times { get; set; }
        public virtual Teachers Teachers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Qr_Codes> Qr_Codes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Students> Students { get; set; }
    }
}