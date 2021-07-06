using System.ComponentModel.DataAnnotations;

namespace BackendCore.Models.Request
{
    public class RegisterRequest
    {
        [Required]
        public string Address { get; set; }
    }
}
