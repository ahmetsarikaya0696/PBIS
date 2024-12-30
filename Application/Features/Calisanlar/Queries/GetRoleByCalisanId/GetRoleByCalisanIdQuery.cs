using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Calisanlar.Queries.GetRoleByCalisanId
{
    public class GetRoleByCalisanIdQuery : IRequest<string>
    {
        public int CalisanId { get; set; }

        public class GetRoleByCalisanIdQueryHandler : IRequestHandler<GetRoleByCalisanIdQuery, string>
        {
            private readonly IIzinOnayTanimCalisanRepository _izinOnayTanimCalisanRepository;
            private readonly IYetkililerRepository _yetkililerRepository;

            public GetRoleByCalisanIdQueryHandler(IIzinOnayTanimCalisanRepository izinOnayTanimCalisanRepository, IYetkililerRepository yetkililerRepository)
            {
                _izinOnayTanimCalisanRepository = izinOnayTanimCalisanRepository;
                _yetkililerRepository = yetkililerRepository;
            }

            public async Task<string> Handle(GetRoleByCalisanIdQuery request, CancellationToken cancellationToken)
            {
                bool yetkiliMi = await _yetkililerRepository.AnyAsync(predicate: x => x.CalisanId == request.CalisanId, cancellationToken: cancellationToken);

                if (yetkiliMi)
                    return "Yetkili";

                bool onayYetkilisiMi = await _izinOnayTanimCalisanRepository.AnyAsync(predicate: x => x.CalisanId == request.CalisanId, cancellationToken: cancellationToken);

                if (onayYetkilisiMi)
                    return "Onay_Yetkilisi";

                return "Default";
            }
        }

    }
}
