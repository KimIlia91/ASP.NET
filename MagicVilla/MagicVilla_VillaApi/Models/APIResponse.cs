using System.Net;

namespace MagicVilla_VillaApi.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public bool IsSuccess { get; set; } = false;

        public List<string> ErrorMessage { get; set; } = new List<string>();

        public object Result { get; set; }
    }
}
