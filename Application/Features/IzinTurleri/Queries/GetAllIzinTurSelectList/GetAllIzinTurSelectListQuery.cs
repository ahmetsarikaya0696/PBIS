using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinTurleri.Queries.GetAllIzinTurSelectList
{
    public class GetAllIzinTurSelectListQuery : IRequest<List<GetAllIzinTurSelectListResponse>>
    {
        public class GetAllIzinTurSelectListQueryHandler : IRequestHandler<GetAllIzinTurSelectListQuery, List<GetAllIzinTurSelectListResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IIzinTurleriRepository _izinTurleriRepository;

            public GetAllIzinTurSelectListQueryHandler(IMapper mapper, IIzinTurleriRepository izinTurleriRepository)
            {
                _mapper = mapper;
                _izinTurleriRepository = izinTurleriRepository;
            }

            public async Task<List<GetAllIzinTurSelectListResponse>> Handle(GetAllIzinTurSelectListQuery request, CancellationToken cancellationToken)
            {
                List<IzinTur> izinTurleri = await _izinTurleriRepository.GetAllAsync(cancellationToken: cancellationToken);


                var response = _mapper.Map<List<GetAllIzinTurSelectListResponse>>(izinTurleri);

                return response;
            }
        }
    }
}
