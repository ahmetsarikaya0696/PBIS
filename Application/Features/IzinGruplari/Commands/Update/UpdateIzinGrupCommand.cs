using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinGruplari.Commands.Update
{
    public class UpdateIzinGrupCommand : IRequest<UpdatedIzinGrupResponse>
    {
        public int Id { get; set; }

        public string Aciklama { get; set; }

        public DateTime BaslangicTarihi { get; set; }

        public DateTime BitisTarihi { get; set; }

        public class UpdateIzinGrupCommandHandler : IRequestHandler<UpdateIzinGrupCommand, UpdatedIzinGrupResponse>
        {
            private readonly IMapper _mapper;
            private readonly IIzinGruplariRepository _izinGruplariRepository;

            public UpdateIzinGrupCommandHandler(IMapper mapper, IIzinGruplariRepository izinGruplariRepository)
            {
                _mapper = mapper;
                _izinGruplariRepository = izinGruplariRepository;
            }

            public async Task<UpdatedIzinGrupResponse> Handle(UpdateIzinGrupCommand request, CancellationToken cancellationToken)
            {
                IzinGrup izinGrup = await _izinGruplariRepository.GetAsync(predicate: x => x.Id == request.Id,
                                                                           cancellationToken: cancellationToken)
                                            ?? throw new ClientsideException("Belirtilen ID ' ye sahip izin grubu bulunamadı!");

                bool ayniAciklmayaSahipAktifVeriBulundu = await _izinGruplariRepository.AnyAsync(predicate: x => x.Id != izinGrup.Id &&
                                                                                             x.Aciklama.Trim().ToLower() == request.Aciklama.Trim().ToLower() &&
                                                                                            (x.BaslangicTarihi <= DateTime.Now && x.BitisTarihi >= DateTime.Now),
                                                                                  cancellationToken: cancellationToken);

                if (ayniAciklmayaSahipAktifVeriBulundu) throw new ClientsideException($"\"{request.Aciklama}\" açıklamasıyla oluşturulmuş mevcut aktif kayıt bulundu!");

                bool ayniOzellikteAktifIzinGrubuVar = await _izinGruplariRepository.AnyAsync(predicate: x => x.Id != izinGrup.Id &&
                                                                                                             x.IsyeriId == izinGrup.IsyeriId &&
                                                                                                             x.BirimId == izinGrup.BirimId &&
                                                                                                             x.UnvanId == izinGrup.UnvanId &&
                                                                                                             x.CalisanId == izinGrup.CalisanId &&
                                                                                                            (x.BaslangicTarihi <= DateTime.Now && x.BitisTarihi >= DateTime.Now),
                                                                                             cancellationToken: cancellationToken);

                if (ayniOzellikteAktifIzinGrubuVar) throw new ClientsideException("Aynı özelliklere sahip aktif mevcut kayıt bulundu!");

                IzinGrup mappedIzinGrup = _mapper.Map(request, izinGrup);

                IzinGrup updatedIzinGrup = await _izinGruplariRepository.UpdateAsync(mappedIzinGrup);

                UpdatedIzinGrupResponse response = _mapper.Map<UpdatedIzinGrupResponse>(updatedIzinGrup);

                return response;
            }
        }
    }
}
