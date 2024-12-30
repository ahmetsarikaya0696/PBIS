using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Izinler.Commands.Create
{
    public class CreateIzinCommand : IRequest<CreatedIzinResponse>
    {
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

        public class CreateIzinCommandHandler : IRequestHandler<CreateIzinCommand, CreatedIzinResponse>
        {
            private readonly IMapper _mapper;
            private readonly ICalisanlarRepository _calisanlarRepository;
            private readonly IIzinlerRepository _izinlerRepository;
            private readonly IDogrulamaRepository _dogrulamaRepository;

            public CreateIzinCommandHandler(IMapper mapper, ICalisanlarRepository calisanlarRepository, IIzinlerRepository izinlerRepository, IDogrulamaRepository dogrulamaRepository)
            {
                _mapper = mapper;
                _calisanlarRepository = calisanlarRepository;
                _izinlerRepository = izinlerRepository;
                _dogrulamaRepository = dogrulamaRepository;
            }

            public async Task<CreatedIzinResponse> Handle(CreateIzinCommand request, CancellationToken cancellationToken)
            {
                // Validate Kod
                Dogrulama dogrulama = await _dogrulamaRepository.GetAsync(predicate: x => x.CalisanId == request.CalisanId &&
                                                                                     x.Kod == request.Kod &&
                                                                                     x.SonKullanimTarihi > DateTime.Now &&
                                                                                     x.Gecerli == true &&
                                                                                     x.IzinId == null,
                                                                          cancellationToken: cancellationToken);

                bool dogrulamaGecersiz = dogrulama == null;
                if (dogrulamaGecersiz) throw new ClientsideException("Kod geçersiz!");

                // İzin istenilen tarihteki calisandan getirilecek İsyeriId, BirimKodu, Unvan kodu
                Calisan calisan = await _calisanlarRepository.GetAsync(predicate: x => x.Id == request.CalisanId, cancellationToken: cancellationToken);

                // calisandan İsyeriId, BirimKodu, Unvan kodu getirldi.
                Izin izin = _mapper.Map<Izin>(calisan);

                izin = _mapper.Map(request, izin);

                Izin createdIzin = await _izinlerRepository.AddAsync(izin);

                CreatedIzinResponse createdIzinResponse = _mapper.Map<CreatedIzinResponse>(izin);

                // Kod geçerliyse kodu geçersiz yap ve olusturulan iznin izinId'sini dogrulama tablosuna yaz
                dogrulama.Gecerli = false;
                dogrulama.IzinId = createdIzin.Id;
                await _dogrulamaRepository.UpdateAsync(dogrulama);

                return createdIzinResponse;
            }
        }
    }
}
