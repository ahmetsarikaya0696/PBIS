using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Tatiller.Queries.GetTatiller
{
    public class GetTatillerQuery : IRequest<List<GetTatillerResponse>>
    {
        public class GetTatillerQueryHandler : IRequestHandler<GetTatillerQuery, List<GetTatillerResponse>>
        {
            private readonly ITatillerRepository _tatillerRepository;
            private readonly IMapper _mapper;

            public GetTatillerQueryHandler(ITatillerRepository tatillerRepository, IMapper mapper)
            {
                _tatillerRepository = tatillerRepository;
                _mapper = mapper;
            }

            public async Task<List<GetTatillerResponse>> Handle(GetTatillerQuery request, CancellationToken cancellationToken)
            {
                List<Tatil> tatiller = await _tatillerRepository.GetAllAsync(cancellationToken: cancellationToken);

                return _mapper.Map<List<GetTatillerResponse>>(tatiller);
            }
        }
    }
}
