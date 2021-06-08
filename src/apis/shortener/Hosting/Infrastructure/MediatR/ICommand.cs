using MediatR;

namespace Hosting.Infrastructure.MediatR
{
    public interface ICommand : IRequest<IRequestResult>
    {
        
    }
}