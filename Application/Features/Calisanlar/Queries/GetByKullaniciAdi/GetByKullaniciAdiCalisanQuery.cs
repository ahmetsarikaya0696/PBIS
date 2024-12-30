using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Calisanlar.Queries.GetByKullaniciAdi
{
    public class GetByKullaniciAdiCalisanQuery : IRequest<GetByKullaniciAdiCalisanResponse>
    {
        public string KullaniciAdi { get; set; }

        public class GetByKullaniciAdiCalisanQueryHandler : IRequestHandler<GetByKullaniciAdiCalisanQuery, GetByKullaniciAdiCalisanResponse>
        {
            private readonly IMapper _mapper;
            private readonly ICalisanlarRepository _calisanlarRepository;

            public GetByKullaniciAdiCalisanQueryHandler(IMapper mapper, ICalisanlarRepository calisanlarRepository)
            {
                _mapper = mapper;
                _calisanlarRepository = calisanlarRepository;
            }

            public async Task<GetByKullaniciAdiCalisanResponse> Handle(GetByKullaniciAdiCalisanQuery request, CancellationToken cancellationToken)
            {
                Calisan calisan = await _calisanlarRepository.GetAsync(predicate: x => x.KullaniciAdi == $"{request.KullaniciAdi}@baskent.edu.tr",
                                                                       include: source => source.Include(x => x.Unvan)
                                                                                                .Include(x => x.Birim).ThenInclude(x => x.OrganizasyonSemasi),
                                                                       cancellationToken: cancellationToken);

                GetByKullaniciAdiCalisanResponse response = _mapper.Map<GetByKullaniciAdiCalisanResponse>(calisan);

                response.BirimAmiri = await _calisanlarRepository.BirimAmiriMiAsync(calisan.Id);

                return response;
            }
        }
    }
}
