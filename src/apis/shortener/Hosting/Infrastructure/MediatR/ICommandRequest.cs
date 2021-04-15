using MediatR;

namespace Hosting.Infrastructure.MediatR
{
    public interface ICommandRequest : IRequest<IRequestResult>
    {
        
    }
}