using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Izinler.Queries.GetGunSayisiVeIseBaslamaTarihi
{
    public class GetGunSayisiVeIseBaslamaTarihiQuery : IRequest<GetGunSayisiVeIseBaslamaTarihiResponse>
    {
        public int IzinTurId { get; set; }

        public DateTime BaslangicTarihi { get; set; }

        public DateTime BitisTarihi { get; set; }

        public DateTime? MahsubenBaslangicTarihi { get; set; }
    }

    public class GetGunSayisiVeIseBaslamaTarihiQueryHandler : IRequestHandler<GetGunSayisiVeIseBaslamaTarihiQuery, GetGunSayisiVeIseBaslamaTarihiResponse>
    {
        private readonly IIzinlerRepository _izinlerRepository;
        private readonly ITatillerRepository _tatillerRepository;

        public GetGunSayisiVeIseBaslamaTarihiQueryHandler(IIzinlerRepository izinlerRepository, ITatillerRepository tatillerRepository)
        {
            _izinlerRepository = izinlerRepository;
            _tatillerRepository = tatillerRepository;
        }

        public async Task<GetGunSayisiVeIseBaslamaTarihiResponse> Handle(GetGunSayisiVeIseBaslamaTarihiQuery request, CancellationToken cancellationToken)
        {
            return new GetGunSayisiVeIseBaslamaTarihiResponse()
            {
                GunSayisi = await _izinlerRepository.GetIzinGunSayisiAsync(request.BaslangicTarihi, request.BitisTarihi, request.IzinTurId, cancellationToken: cancellationToken),
                MahsubenGunSayisi = request.MahsubenBaslangicTarihi.HasValue ? await _izinlerRepository.GetIzinGunSayisiAsync(request.MahsubenBaslangicTarihi.Value, request.BitisTarihi, request.IzinTurId, cancellationToken: cancellationToken) : null,
                IseBaslamaTarihi = await GetIseBaslamaTarihiAsync(request, cancellationToken)
            };
        }

        private async Task<string> GetIseBaslamaTarihiAsync(GetGunSayisiVeIseBaslamaTarihiQuery request, CancellationToken cancellationToken)
        {
            DateTime iseBaslamaTarihi = request.BitisTarihi.AddDays(1);
            while (await TatilGunuAsync(iseBaslamaTarihi, cancellationToken))
            {
                iseBaslamaTarihi = iseBaslamaTarihi.AddDays(1);
            }

            return iseBaslamaTarihi.ToString("dd.MM.yyyy");
        }

        private async Task<bool> TatilGunuAsync(DateTime date, CancellationToken cancellationToken)
        {
            return date.DayOfWeek == DayOfWeek.Sunday || await _tatillerRepository.AnyAsync(predicate: x => x.Tarih == date, cancellationToken: cancellationToken);
        }
    }
}