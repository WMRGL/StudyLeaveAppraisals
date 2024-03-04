using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace StudyLeaveAppraisals.Models
{
    [Table("STAFF", Schema = "dbo")]
    public class UserDetails
    {
        [Required]
        [DisplayName("User ID")]
        public string EMPLOYEE_NUMBER { get; set; }
        [Required]
        [DisplayName("Password")]
        public string PASSWORD { get; set; }
    }
}
