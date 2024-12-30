using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tatiller.Commands.Update
{
    public class UpdateTatilCommand : IRequest<UpdatedTatilResponse>
    {
        public int Id { get; set; }
        public DateTime Tarih { get; set; }
        public string Aciklama { get; set; }

        public class UpdateTatilCommandHandler : IRequestHandler<UpdateTatilCommand, UpdatedTatilResponse>
        {
            private readonly ITatillerRepository _tatillerRepository;
            private readonly IIzinlerRepository _izinlerRepository;
            private readonly IMapper _mapper;

            public UpdateTatilCommandHandler(ITatillerRepository tatillerRepository, IIzinlerRepository izinlerRepository, IMapper mapper)
            {
                _tatillerRepository = tatillerRepository;
                _izinlerRepository = izinlerRepository;
                _mapper = mapper;
            }

            public async Task<UpdatedTatilResponse> Handle(UpdateTatilCommand request, CancellationToken cancellationToken)
            {
                var tatil = await _tatillerRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken)
                    ?? throw new ClientsideException("Belirtilen ID ' ye sahip veri bulunamadı!");

                var iliskiliIzinler = await _izinlerRepository.GetAllAsync(predicate: x => x.BaslangicTarihi <= tatil.Tarih && x.BitisTarihi >= tatil.Tarih && !x.IzinTur.TatilGunleriSayilir,
                                                                          include: source => source.Include(x => x.IzinTur),
                                                                          cancellationToken: cancellationToken);

                bool iliskiliKayitVar = iliskiliIzinler?.Count > 0;

                bool tarihDegisti = tatil.Tarih != request.Tarih;

                if (iliskiliKayitVar && tarihDegisti) throw new ClientsideException("İlişkili kaydı bulunan izin gününün tarihi değiştirilemez!");

                var mappedTatil = _mapper.Map<Tatil>(request);
                var updatedTatil = await _tatillerRepository.UpdateAsync(mappedTatil);

                return _mapper.Map<UpdatedTatilResponse>(updatedTatil);
            }
        }
    }
}
