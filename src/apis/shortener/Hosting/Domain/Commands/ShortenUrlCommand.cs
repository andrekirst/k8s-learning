using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Hosting.Domain.Database;
using Hosting.Infrastructure.MediatR;
using Hosting.Infrastructure.MediatR.Results;
using Hosting.Services;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Hosting.Domain.Commands
{
    public class ShortenUrlCommand : ICommand
    {
        public string Url { get; }

        public ShortenUrlCommand(string url)
        {
            Url = url;
        }
    }

    public class ShortenUrlCommandValidator : AbstractValidator<ShortenUrlCommand>
    {
        public ShortenUrlCommandValidator()
        {
            RuleFor(x => x.Url)
                .NotEmpty();
        }
    }

    public class ShortenUrlCommandHandler : IRequestHandler<ShortenUrlCommand, IRequestResult>
    {
        private readonly IUrlHasher _urlHasher;
        private readonly IShortenUrlRepository _shortenUrlRepository;
        private readonly ICodeServiceClient _codeServiceClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IDistributedCache _distributedCache;

        public ShortenUrlCommandHandler(
            IUrlHasher urlHasher,
            IShortenUrlRepository shortenUrlRepository,
            ICodeServiceClient codeServiceClient,
            IPublishEndpoint publishEndpoint,
            IDistributedCache distributedCache)
        {
            _urlHasher = urlHasher;
            _shortenUrlRepository = shortenUrlRepository;
            _codeServiceClient = codeServiceClient;
            _publishEndpoint = publishEndpoint;
            _distributedCache = distributedCache;
        }

        public async Task<IRequestResult> Handle(ShortenUrlCommand request, CancellationToken cancellationToken)
        {
            var url = request.Url;

            var cachedCode = await GetCachedCodeByUrl(url, cancellationToken);

            if (cachedCode != null)
            {
                return new OkObjectResult(new ShortenUrlResponse
                {
                    Code = cachedCode,
                    Url = url
                });
            }

            var urlHash = _urlHasher.CreateHash(url);

            cachedCode = await GetCachedCodeByUrlHash(urlHash, cancellationToken);

            if (cachedCode != null)
            {
                return new OkObjectResult(new ShortenUrlResponse
                {
                    Code = cachedCode,
                    Url = url
                });
            }

            var existsEntry = await _shortenUrlRepository.ExistsEntryByUrlHash(urlHash, cancellationToken);

            if (existsEntry)
            {
                var codeFromRepository = await _shortenUrlRepository.GetCodeByUrlHash(urlHash, cancellationToken);
                await CacheCodeWithUrlHash(urlHash, codeFromRepository, cancellationToken);
                await CacheCodeWithUrl(url, codeFromRepository, cancellationToken);

                return new OkObjectResult(new ShortenUrlResponse
                {
                    Code = codeFromRepository,
                    Url = url
                });
            }

            var code = await _codeServiceClient.CreateCode(cancellationToken);

            await _shortenUrlRepository.CreateEntry(code, url, urlHash, cancellationToken);
            await CacheCodeWithUrlHash(urlHash, code, cancellationToken);
            await CacheCodeWithUrl(url, code, cancellationToken);

            var shortUrlCreatedEvent = new ShortUrlCreatedEvent
            {
                Code = code,
                Url = url
            };
            await _publishEndpoint.Publish(shortUrlCreatedEvent, cancellationToken);

            return new OkObjectResult(new ShortenUrlResponse
            {
                Code = code,
                Url = url
            });
        }

        private Task<string?> GetCachedCodeByUrlHash(string urlHash, CancellationToken cancellationToken)
            => _distributedCache.GetStringAsync($"urlHash:{urlHash}", cancellationToken);

        private Task CacheCodeWithUrlHash(string urlHash, string code, CancellationToken cancellationToken) =>
            _distributedCache.SetStringAsync($"urlHash:{urlHash}", code,
                new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)},
                cancellationToken);

        private Task<string?> GetCachedCodeByUrl(string url, CancellationToken cancellationToken)
            => _distributedCache.GetStringAsync($"url:{url}", cancellationToken);

        private Task CacheCodeWithUrl(string url, string code, CancellationToken cancellationToken) =>
            _distributedCache.SetStringAsync($"url:{url}", code,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) },
                cancellationToken);
    }

    public class ShortUrlCreatedEvent
    {
        public string Code { get; set; }
        public string Url { get; set; }
    }

    public class ShortenUrlRequestModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Url can not be empty.")]
        public string Url { get; set; }
    }

    public class ShortenUrlResponse
    {
        public string Url { get; set; }
        public string Code { get; set; }
    }
}