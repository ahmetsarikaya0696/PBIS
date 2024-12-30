using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinOnayTanimlari.Commands.Delete
{
    public class DeleteIzinOnayTanimByIdCommand : IRequest<string>
    {
        public int Id { get; set; }

        public class DeleteIzinOnayTanimByIdCommandHandler : IRequestHandler<DeleteIzinOnayTanimByIdCommand, string>
        {
            private readonly IIzinHareketleriRepository _izinHareketleriRepository;
            private readonly IIzinGrupIzinOnayTanimRepository _izinGrupIzinOnayTanimRepository;
            private readonly IIzinOnayTanimlariRepository _izinOnayTanimlariRepository;

            public DeleteIzinOnayTanimByIdCommandHandler(IIzinHareketleriRepository izinHareketleriRepository, IIzinGrupIzinOnayTanimRepository izinGrupIzinOnayTanimRepository, IIzinOnayTanimlariRepository izinOnayTanimlariRepository)
            {
                _izinHareketleriRepository = izinHareketleriRepository;
                _izinGrupIzinOnayTanimRepository = izinGrupIzinOnayTanimRepository;
                _izinOnayTanimlariRepository = izinOnayTanimlariRepository;
            }

            public async Task<string> Handle(DeleteIzinOnayTanimByIdCommand request, CancellationToken cancellationToken)
            {
                IzinOnayTanim silinecekIzinOnayTanim = await _izinOnayTanimlariRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken)
                                                                ?? throw new ClientsideException("Belirtilen ID ' ye sahip izin onay tanımı bulunamadı!");

                bool iliskiliIzinGrubuVerisiMevcut = await _izinGrupIzinOnayTanimRepository.AnyAsync(predicate: x => x.IzinOnayTanimId == request.Id, cancellationToken: cancellationToken);
                if (iliskiliIzinGrubuVerisiMevcut) throw new ClientsideException("İlişkili izin grubu verisi bulundu!");

                bool iliskiliIzinHareketVerisiMevcut = await _izinHareketleriRepository.AnyAsync(predicate: x => x.IzinOnayTanimId == request.Id, cancellationToken: cancellationToken);
                if (iliskiliIzinHareketVerisiMevcut) throw new ClientsideException("İlişkili izin hareketi verisi bulundu!");

                IzinOnayTanim silinmisIzinOnayTanim = await _izinOnayTanimlariRepository.DeleteAsync(silinecekIzinOnayTanim);

                return silinecekIzinOnayTanim.Aciklama;
            }
        }
    }
}
