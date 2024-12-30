using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.IzinGruplari.Queries.GetAllIzinGrup
{
    public class GetAllIzinGrupQuery : IRequest<List<GetAllIzinGrupResponse>>
    {
        public class GetAllIzinGrupQueryHandler : IRequestHandler<GetAllIzinGrupQuery, List<GetAllIzinGrupResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IIzinGruplariRepository _izinGruplariRepository;

            public GetAllIzinGrupQueryHandler(IMapper mapper, IIzinGruplariRepository izinGruplariRepository)
            {
                _mapper = mapper;
                _izinGruplariRepository = izinGruplariRepository;
            }

            public async Task<List<GetAllIzinGrupResponse>> Handle(GetAllIzinGrupQuery request, CancellationToken cancellationToken)
            {
                List<IzinGrup> izinGruplari = await _izinGruplariRepository.GetAllAsync(include: source => source.Include(x => x.Calisan).ThenInclude(y => y.Unvan)
                                                                                                                 .Include(x => x.Unvan)
                                                                                                                 .Include(x => x.Birim)
                                                                                                                 .Include(x => x.Isyeri),
                                                                                        cancellationToken: cancellationToken);

                var response = _mapper.Map<List<GetAllIzinGrupResponse>>(izinGruplari);

                return response;
            }
        }
    }
}
