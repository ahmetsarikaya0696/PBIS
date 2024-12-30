using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinTurleri.Commands.Delete
{
    public class DeleteIzinTurByIdCommand : IRequest<string>
    {
        public int Id { get; set; }

        public class DeleteIzinTurByIdCommandHandler : IRequestHandler<DeleteIzinTurByIdCommand, string>
        {
            private readonly IIzinTurleriRepository _izinTurleriRepository;
            private readonly IIzinlerRepository _izinlerRepository;

            public DeleteIzinTurByIdCommandHandler(IIzinTurleriRepository izinTurleriRepository, IIzinlerRepository izinlerRepository)
            {
                _izinTurleriRepository = izinTurleriRepository;
                _izinlerRepository = izinlerRepository;
            }

            public async Task<string> Handle(DeleteIzinTurByIdCommand request, CancellationToken cancellationToken)
            {
                IzinTur silinecekIzinTur = await _izinTurleriRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken)
                                                ?? throw new ClientsideException("Belirtilen ID ' ye sahip izin türü bulunamadı!");

                bool iliskiliIzinMevcut = await _izinlerRepository.AnyAsync(predicate: x => x.IzinTurId == request.Id, cancellationToken: cancellationToken);
                if (iliskiliIzinMevcut) throw new ClientsideException("İlişkili izin verisi bulundu!");

                IzinTur silinmisIzinTur = await _izinTurleriRepository.DeleteAsync(silinecekIzinTur);

                return silinecekIzinTur.Aciklama;
            }
        }
    }
}
