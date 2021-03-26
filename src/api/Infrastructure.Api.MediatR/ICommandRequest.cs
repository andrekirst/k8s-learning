using MediatR;

namespace Infrastructure.Api.MediatR
{
    public interface ICommandRequest : IRequest<IRequestResult>
    {
        
    }
}