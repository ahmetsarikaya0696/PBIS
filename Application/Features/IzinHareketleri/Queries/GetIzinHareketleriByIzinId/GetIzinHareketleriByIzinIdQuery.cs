using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.IzinHareketleri.Queries.GetIzinHareketleriByIzinId
{
    public class GetIzinHareketleriByIzinIdQuery : IRequest<List<GetIzinHareketleriByIzinIdResponse>>
    {
        public int IzinId { get; set; }

        public int CalisanId { get; set; }

        public class GetIzinHareketleriByIzinIdQueryHandler : IRequestHandler<GetIzinHareketleriByIzinIdQuery, List<GetIzinHareketleriByIzinIdResponse>>
        {
            private readonly ICalisanlarRepository _calisanlarRepository;
            private readonly IIzinHareketleriRepository _izinHareketleriRepository;
            private readonly IMapper _mapper;

            public GetIzinHareketleriByIzinIdQueryHandler(ICalisanlarRepository calisanlarRepository, IIzinHareketleriRepository izinHareketleriRepository, IMapper mapper)
            {
                _calisanlarRepository = calisanlarRepository;
                _izinHareketleriRepository = izinHareketleriRepository;
                _mapper = mapper;
            }

            public async Task<List<GetIzinHareketleriByIzinIdResponse>> Handle(GetIzinHareketleriByIzinIdQuery request, CancellationToken cancellationToken)
            {
                List<IzinHareket> izinHareketleri = await _izinHareketleriRepository.GetAllAsync(predicate: x => x.IzinId == request.IzinId,
                                                                                                 include: source => source.Include(x => x.Calisan).ThenInclude(x => x.Unvan)
                                                                                                                          .Include(x => x.IzinDurum)
                                                                                                                          .Include(x => x.IzinOnayTanim),
                                                                                                 orderBy: source => source.OrderBy(x => x.IslemTarihi),
                                                                                                 cancellationToken: cancellationToken);

                var response = _mapper.Map<List<GetIzinHareketleriByIzinIdResponse>>(izinHareketleri);


                // Düzenleme veya iptal işlemini çalışanın kendisi yaptığı için tabloda "Kendisi olarak gösterildi"
                Calisan calisan = await _calisanlarRepository.GetAsync(predicate: x => x.Id == request.CalisanId,
                                                                        include: source => source.Include(x => x.Unvan),
                                                                        cancellationToken: cancellationToken);

                response = response.Select(x =>
                {
                    if ((x.IzinDurum == "Düzenlendi" || x.IzinDurum == "İptal Edildi") && x.IslemYapanCalisanAdSoyad == $"{calisan.Unvan.Aciklama} {calisan.Ad} {calisan.Soyad}")
                    {
                        x.IslemYapanCalisanAdSoyad = "Kendisi";
                    }

                    return x;
                }).ToList();


                return response;
            }
        }
    }
}
