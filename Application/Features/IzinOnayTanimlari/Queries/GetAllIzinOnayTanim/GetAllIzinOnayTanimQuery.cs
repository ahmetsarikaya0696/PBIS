using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinOnayTanimlari.Queries.GetAllIzinOnayTanim
{
    public class GetAllIzinOnayTanimQuery : IRequest<List<GetAllIzinOnayTanimResponse>>
    {
        public class GetAllIzinOnayTanimQueryHandler : IRequestHandler<GetAllIzinOnayTanimQuery, List<GetAllIzinOnayTanimResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IIzinOnayTanimlariRepository _izinOnayTanimlariRepository;

            public GetAllIzinOnayTanimQueryHandler(IMapper mapper, IIzinOnayTanimlariRepository izinOnayTanimlariRepository)
            {
                _mapper = mapper;
                _izinOnayTanimlariRepository = izinOnayTanimlariRepository;
            }

            public async Task<List<GetAllIzinOnayTanimResponse>> Handle(GetAllIzinOnayTanimQuery request, CancellationToken cancellationToken)
            {
                List<IzinOnayTanim> izinOnayTanimlari = await _izinOnayTanimlariRepository.GetAllAsync(cancellationToken: cancellationToken);

                List<GetAllIzinOnayTanimResponse> response = _mapper.Map<List<GetAllIzinOnayTanimResponse>>(izinOnayTanimlari);

                return response;
            }
        }
    }
}
