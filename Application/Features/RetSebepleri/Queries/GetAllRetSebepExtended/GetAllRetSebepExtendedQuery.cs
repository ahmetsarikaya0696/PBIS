using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.RetSebepleri.Queries.GetAllRetSebepExtended
{
    public class GetAllRetSebepExtendedQuery : IRequest<List<GetAllRetSebepExtendedResponse>>
    {
        public class GetAllRetSebepExtendedQueryHandler : IRequestHandler<GetAllRetSebepExtendedQuery, List<GetAllRetSebepExtendedResponse>>
        {
            private readonly IRetSebepleriRepository _retSebepleriRepository;
            private readonly IMapper _mapper;

            public GetAllRetSebepExtendedQueryHandler(IRetSebepleriRepository retSebepleriRepository, IMapper mapper)
            {
                _retSebepleriRepository = retSebepleriRepository;
                _mapper = mapper;
            }

            public async Task<List<GetAllRetSebepExtendedResponse>> Handle(GetAllRetSebepExtendedQuery request, CancellationToken cancellationToken)
            {
                List<RetSebep> retSebepleri = await _retSebepleriRepository.GetAllAsync(cancellationToken: cancellationToken);

                var response = _mapper.Map<List<GetAllRetSebepExtendedResponse>>(retSebepleri);

                return response;
            }
        }
    }
}
