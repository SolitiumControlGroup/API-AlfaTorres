using System.Security.Claims;

namespace API_AlfaTorres.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
        public List<Claim> Claims { get; set; } 
    }
}
