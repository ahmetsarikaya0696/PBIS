using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinGruplari.Commands.Delete
{
    public class DeleteIzinGrupByIdCommand : IRequest<string>
    {
        public int Id { get; set; }

        public class DeleteIzinGrupByIdCommandHandler : IRequestHandler<DeleteIzinGrupByIdCommand, string>
        {
            private readonly IIzinGruplariRepository _izinGruplariRepository;

            public DeleteIzinGrupByIdCommandHandler(IIzinGruplariRepository izinGruplariRepository)
            {
                _izinGruplariRepository = izinGruplariRepository;
            }

            public async Task<string> Handle(DeleteIzinGrupByIdCommand request, CancellationToken cancellationToken)
            {
                IzinGrup silinecekIzinGrubu = await _izinGruplariRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken)
                                                    ?? throw new ClientsideException("Belirtilen ID ' ye sahip izin grubu bulunamadı!");

                IzinGrup silinenIzinGrubu = await _izinGruplariRepository.DeleteAsync(silinecekIzinGrubu);

                return silinecekIzinGrubu.Aciklama;
            }
        }
    }
}
