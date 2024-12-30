using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Izinler.Commands.IptalEt
{
    public class IptalEtIzinCommand : IRequest<bool>
    {
        public int IzinId { get; set; }

        public class IptalEtIzinCommandHandler : IRequestHandler<IptalEtIzinCommand, bool>
        {
            private readonly IIzinlerRepository _izinlerRepository;
            private readonly IIzinHareketleriRepository _izinHareketleriRepository;

            public IptalEtIzinCommandHandler(IIzinlerRepository izinlerRepository, IIzinHareketleriRepository izinHareketleriRepository)
            {
                _izinlerRepository = izinlerRepository;
                _izinHareketleriRepository = izinHareketleriRepository;
            }

            public async Task<bool> Handle(IptalEtIzinCommand request, CancellationToken cancellationToken)
            {
                Izin izin = await _izinlerRepository.GetAsync(predicate: x => x.Id == request.IzinId, cancellationToken: cancellationToken);

                izin.IzinDurumId = (int)IzinDurumEnum.Iptal_Edildi;

                Izin guncellenenIzin = await _izinlerRepository.UpdateAsync(izin);

                IzinHareket izinHareket = await _izinHareketleriRepository.AddAsync(new()
                {
                    IzinId = guncellenenIzin.Id,
                    Ip = guncellenenIzin.Ip,
                    CalisanId = guncellenenIzin.CalisanId,
                    IslemTarihi = DateTime.Now,
                    IzinDurumId = guncellenenIzin.IzinDurumId,
                    IzinOnayTanimId = null,
                });

                return guncellenenIzin != null && izinHareket != null;
            }
        }
    }
}
