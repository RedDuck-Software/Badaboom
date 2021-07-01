using System.ComponentModel.DataAnnotations;

namespace BackendCore.Models.Request
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string SignedNonce { get; set; }
    }
}
