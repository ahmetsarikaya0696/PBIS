using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.Izinler.Queries.GetBitisTarihi
{
    public class GetBitisTarihiQuery : IRequest<GetBitisTarihiResponse>
    {
        public int IzinTurId { get; set; }

        public DateTime BaslangicTarihi { get; set; }

    }

    public class GetBitisTarihiQueryHandler : IRequestHandler<GetBitisTarihiQuery, GetBitisTarihiResponse>
    {
        private readonly IIzinTurleriRepository _izinTurleriRepository;
        private readonly ITatillerRepository _tatillerRepository;

        public GetBitisTarihiQueryHandler(IIzinTurleriRepository izinTurleriRepository, ITatillerRepository tatillerRepository)
        {
            _izinTurleriRepository = izinTurleriRepository;
            _tatillerRepository = tatillerRepository;
        }

        public async Task<GetBitisTarihiResponse> Handle(GetBitisTarihiQuery request, CancellationToken cancellationToken)
        {
            // İzin turu sabit gün olan izin günleri için bitiş tarihi hesapla
            IzinTur izinTur = await _izinTurleriRepository.GetAsync(predicate: x => x.Id == request.IzinTurId, cancellationToken: cancellationToken);
            int sabitGunSayisi = izinTur.SabitGunSayisi - 1 ?? throw new ClientsideException("Seçtiğiniz iznin sabit izin gün sayısı yoktur!");
            bool tatilGunleriSayilir = izinTur.TatilGunleriSayilir; // Tatil Günleri
            DateTime bitisTarihi = request.BaslangicTarihi.AddDays(sabitGunSayisi);


            if (!tatilGunleriSayilir)
            {
                for (DateTime tarih = request.BaslangicTarihi; tarih <= bitisTarihi; tarih = tarih.AddDays(1))
                {
                    if (tarih.DayOfWeek == DayOfWeek.Sunday || await IzinGunu(tarih, cancellationToken))
                        bitisTarihi = bitisTarihi.AddDays(1);
                }
            }

            return new GetBitisTarihiResponse()
            {
                BitisTarihi = bitisTarihi.ToString("dd.MM.yyyy")
            };


            // Helper Function
            async Task<bool> IzinGunu(DateTime tarih, CancellationToken cancellationToken)
            {
                return await _tatillerRepository.AnyAsync(predicate: x => x.Tarih == tarih, cancellationToken: cancellationToken);
            }
        }
    }
}