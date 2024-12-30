using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinTurleri.Commands.Update
{
    public class UpdateIzinTurCommand : IRequest<UpdatedIzinTurResponse>
    {
        public int Id { get; set; }

        public string Aciklama { get; set; }

        public char IzinFormTipi { get; set; }

        public int? SabitGunSayisi { get; set; }

        public bool TatilGunleriSayilir { get; set; }

        public bool Aktif { get; set; }

        public class UpdateIzinTurCommandHandler : IRequestHandler<UpdateIzinTurCommand, UpdatedIzinTurResponse>
        {
            private readonly IMapper _mapper;
            private readonly IIzinTurleriRepository _izinTurleriRepository;

            public UpdateIzinTurCommandHandler(IMapper mapper, IIzinTurleriRepository izinTurleriRepository)
            {
                _mapper = mapper;
                _izinTurleriRepository = izinTurleriRepository;
            }

            public async Task<UpdatedIzinTurResponse> Handle(UpdateIzinTurCommand request, CancellationToken cancellationToken)
            {
                IzinTur guncellenecekIzinTur = await _izinTurleriRepository.GetAsync(predicate: x => x.Id == request.Id,
                                                                                     cancellationToken: cancellationToken)
                                               ?? throw new ClientsideException("Belirtilen ID ' ye sahip izin türü bulunamadı!");

                IzinTur mappedIzinTur = _mapper.Map(request, guncellenecekIzinTur);

                IzinTur guncellenenIzinTur = await _izinTurleriRepository.UpdateAsync(mappedIzinTur);

                UpdatedIzinTurResponse response = _mapper.Map<UpdatedIzinTurResponse>(guncellenenIzinTur);

                return response;
            }
        }
    }
}
