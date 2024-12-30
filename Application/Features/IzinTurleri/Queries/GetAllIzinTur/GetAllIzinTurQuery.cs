using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinTurleri.Queries.GetAllIzinTur
{
    public class GetAllIzinTurQuery : IRequest<List<GetAllIzinTurResponse>>
    {
        public class GetAllIzinTurQueryHandler : IRequestHandler<GetAllIzinTurQuery, List<GetAllIzinTurResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IIzinTurleriRepository _izinTurleriRepository;

            public GetAllIzinTurQueryHandler(IMapper mapper, IIzinTurleriRepository izinTurleriRepository)
            {
                _mapper = mapper;
                _izinTurleriRepository = izinTurleriRepository;
            }

            public async Task<List<GetAllIzinTurResponse>> Handle(GetAllIzinTurQuery request, CancellationToken cancellationToken)
            {
                List<IzinTur> izinTurleri = await _izinTurleriRepository.GetAllAsync(cancellationToken: cancellationToken);


                var response = _mapper.Map<List<GetAllIzinTurResponse>>(izinTurleri);

                return response;
            }
        }
    }
}
