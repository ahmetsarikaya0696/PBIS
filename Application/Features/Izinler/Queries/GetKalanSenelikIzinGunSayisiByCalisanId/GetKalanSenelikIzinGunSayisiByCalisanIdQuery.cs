using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Izinler.Queries.GetKalanSenelikIzinGunSayisiByCalisanId
{
    public class GetKalanSenelikIzinGunSayisiByCalisanIdQuery : IRequest<GetKalanSenelikIzinGunSayisiByCalisanIdResponse>
    {
        public int CalisanId { get; set; }

        public class GetKalanSenelikIzinGunSayisiByCalisanIdQueryHandler : IRequestHandler<GetKalanSenelikIzinGunSayisiByCalisanIdQuery, GetKalanSenelikIzinGunSayisiByCalisanIdResponse>
        {
            private readonly ICalisanlarRepository _calisanlarRepository;
            private readonly IIzinlerRepository _izinlerRepository;

            public GetKalanSenelikIzinGunSayisiByCalisanIdQueryHandler(ICalisanlarRepository calisanlarRepository, IIzinlerRepository izinlerRepository)
            {
                _calisanlarRepository = calisanlarRepository;
                _izinlerRepository = izinlerRepository;
            }

            public async Task<GetKalanSenelikIzinGunSayisiByCalisanIdResponse> Handle(GetKalanSenelikIzinGunSayisiByCalisanIdQuery request, CancellationToken cancellationToken)
            {
                string tc = (await _calisanlarRepository.GetAsync(predicate: x => x.Id == request.CalisanId, cancellationToken: cancellationToken)).Tc;

                int kalanSenelikIzinGunSayisi = await _izinlerRepository.GetKalanIzinGunSayisiByTcAsync(tc);

                return new GetKalanSenelikIzinGunSayisiByCalisanIdResponse() { KalanSenelikIzinGunSayisi = kalanSenelikIzinGunSayisi };
            }
        }
    }
}
