using System;

namespace Hosting.Domain.Database.Model
{
    public class ShortenUrl
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Url { get; set; }
        public string UrlHash { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}