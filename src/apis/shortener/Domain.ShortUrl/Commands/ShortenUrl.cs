using System.ComponentModel.DataAnnotations;
using Infrastructure.Api.MediatR;

namespace Domain.ShortUrl.Commands
{
    public class ShortenUrl : ICommandRequest
    {
        public string Url { get; }

        public ShortenUrl(string url)
        {
            Url = url;
        }
    }

    public class ShortenUrlRequestModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Url can not be empty.")]
        public string Url { get; set; }
    }
}