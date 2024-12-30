using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Izinler.Commands.Update
{
    public class UpdateIzinCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public int CalisanId { get; set; }

        public string Ip { get; set; }

        public DateTime BaslangicTarihi { get; set; }

        public int GunSayisi { get; set; }

        public DateTime BitisTarihi { get; set; }

        public DateTime IseBaslamaTarihi { get; set; }

        public DateTime? MahsubenBaslangicTarihi { get; set; }

        public string YerineBakacakKisi { get; set; }

        public string Adres { get; set; }

        public string Telefon { get; set; }

        public string Aciklama { get; set; }

        public bool YillikIzinUcretiIstegi { get; set; }

        public int IzinTurId { get; set; }

        public string Kod { get; set; }

        public class UpdateIzinCommandHandler : IRequestHandler<UpdateIzinCommand, bool>
        {
            private readonly IIzinlerRepository _izinlerRepository;
            private readonly IIzinHareketleriRepository _izinHareketleriRepository;
            private readonly IMapper _mapper;
            private readonly IDogrulamaRepository _dogrulamaRepository;
            private readonly IDogrulamaService _dogrulamaService;

            public UpdateIzinCommandHandler(IIzinlerRepository izinlerRepository, IIzinHareketleriRepository izinHareketleriRepository, IMapper mapper, IDogrulamaRepository dogrulamaRepository, IDogrulamaService dogrulamaService)
            {
                _izinlerRepository = izinlerRepository;
                _izinHareketleriRepository = izinHareketleriRepository;
                _mapper = mapper;
                _dogrulamaRepository = dogrulamaRepository;
                _dogrulamaService = dogrulamaService;
            }

            public async Task<bool> Handle(UpdateIzinCommand request, CancellationToken cancellationToken)
            {
                Dogrulama dogrulama = await _dogrulamaRepository.GetAsync(predicate: x => x.CalisanId == request.CalisanId &&
                                                                                     x.Kod == request.Kod &&
                                                                                     x.SonKullanimTarihi > DateTime.Now &&
                                                                                     x.Gecerli == true &&
                                                                                     x.IzinId == null,
                                                                          cancellationToken: cancellationToken);

                bool dogrulamaGecersiz = dogrulama == null;
                if (dogrulamaGecersiz) throw new ClientsideException("Kod geçersiz!");

                Izin izin = await _izinlerRepository.GetAsync(predicate: x => x.Id == request.Id,
                                                              include: source => source.Include(x => x.Calisan),
                                                              cancellationToken: cancellationToken)
                    ?? throw new ClientsideException("Belirtilen ID ' ye sahip izin bulunamadı!");

                int calisanId = izin.Calisan.Id;

                Izin mappedIzin = _mapper.Map(request, izin);

                var updatedIzin = await _izinlerRepository.UpdateAsync(mappedIzin);

                IzinHareket izinHareket = _mapper.Map<IzinHareket>(updatedIzin);

                _mapper.Map(request, izinHareket);

                var eklenenIzinHareket = await _izinHareketleriRepository.AddAsync(izinHareket);

                return updatedIzin != null && eklenenIzinHareket != null;
            }
        }
    }
}
