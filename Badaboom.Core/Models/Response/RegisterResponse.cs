using System.ComponentModel.DataAnnotations;

namespace BackendCore.Models.Response
{
    public class RegisterResponse
    {
        [Required]
        public string Nonce { get; set; }
    }
}
