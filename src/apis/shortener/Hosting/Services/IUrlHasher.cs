using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Hosting.Services
{
    public interface IUrlHasher
    {
        string CreateHash(string url);
    }

    public class UrlHasher : IUrlHasher
    {
        public string CreateHash(string url) =>
            string.Join("", new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(url)).Select(x => x.ToString("X2")).ToArray());
    }
}