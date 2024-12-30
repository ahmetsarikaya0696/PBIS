using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinTurleri.Commands.Create
{
    public class CreateIzinTurCommand : IRequest<CreatedIzinTurResponse>
    {
        public string Aciklama { get; set; }

        public char IzinFormTipi { get; set; }

        public int? SabitGunSayisi { get; set; }

        public bool TatilGunleriSayilir { get; set; }

        public bool Aktif { get; set; }

        public class CreateIzinTurCommandHandler : IRequestHandler<CreateIzinTurCommand, CreatedIzinTurResponse>
        {
            private readonly IMapper _mapper;
            private readonly IIzinTurleriRepository _izinTurleriRepository;

            public CreateIzinTurCommandHandler(IMapper mapper, IIzinTurleriRepository izinTurleriRepository)
            {
                _mapper = mapper;
                _izinTurleriRepository = izinTurleriRepository;
            }

            public async Task<CreatedIzinTurResponse> Handle(CreateIzinTurCommand request, CancellationToken cancellationToken)
            {
                bool aciklamaUniqueDegil = await _izinTurleriRepository.AnyAsync(predicate: x => x.Aciklama.Trim().ToLower() == request.Aciklama.Trim().ToLower(),
                                                                                 cancellationToken: cancellationToken);

                if (aciklamaUniqueDegil) throw new ClientsideException($"\"{request.Aciklama}\" açıklaması ile oluşturulmuş mevcut veri bulundu. İzin türü oluşturulamadı!");

                IzinTur mappedIzinTur = _mapper.Map<IzinTur>(request);

                IzinTur eklenmisIzinTur = await _izinTurleriRepository.AddAsync(mappedIzinTur);

                var response = _mapper.Map<CreatedIzinTurResponse>(eklenmisIzinTur);

                return response;
            }
        }
    }
}
