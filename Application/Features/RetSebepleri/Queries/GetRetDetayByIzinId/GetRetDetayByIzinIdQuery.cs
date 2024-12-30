using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.RetSebepleri.Queries.GetRetDetayByIzinId
{
    public class GetRetDetayByIzinIdQuery : IRequest<List<GetRetDetayByIzinIdResponse>>
    {
        public int IzinId { get; set; }
        public class GetRetDetayByIzinIdQueryHandler : IRequestHandler<GetRetDetayByIzinIdQuery, List<GetRetDetayByIzinIdResponse>>
        {
            private readonly IIzinHareketleriRepository _izinHareketleriRepository;
            private readonly IMapper _mapper;

            public GetRetDetayByIzinIdQueryHandler(IIzinHareketleriRepository izinHareketleriRepository, IMapper mapper)
            {
                _izinHareketleriRepository = izinHareketleriRepository;
                _mapper = mapper;
            }

            public async Task<List<GetRetDetayByIzinIdResponse>> Handle(GetRetDetayByIzinIdQuery request, CancellationToken cancellationToken)
            {
                List<IzinHareket> reddedilmisIzinHareketleri = await _izinHareketleriRepository.GetAllAsync(predicate: x => x.IzinId == request.IzinId &&
                                                                                                     (x.IzinDurumId == (int)IzinDurumEnum.Reddedildi ||
                                                                                                      x.IzinDurumId == (int)IzinDurumEnum.Duzeltilmek_Uzere_Geri_Gonderildi),
                                                                                     include: source => source.Include(x => x.RetDetay).ThenInclude(x => x.RetSebep),
                                                                                     orderBy: source => source.OrderByDescending(x => x.IslemTarihi),
                                                                                     cancellationToken: cancellationToken);

                return _mapper.Map<List<GetRetDetayByIzinIdResponse>>(reddedilmisIzinHareketleri);
            }
        }
    }
}
