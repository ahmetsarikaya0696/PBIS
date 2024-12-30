using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinOnayTanimlari.Commands.Update
{
    public class UpdateIzinOnayTanimWithCalisanlarCommand : IRequest<UpdatedIzinOnayTanimWithCalisanlarResponse>
    {
        public int Id { get; set; }

        public string Aciklama { get; set; }

        public bool MerkezMuduruYetkisi { get; set; }

        public bool PersonelSubeYetkisi { get; set; }

        public bool Aktif { get; set; }

        public List<int> CalisanIdleri { get; set; }

        public class UpdateIzinOnayTanimWithCalisanlarCommandHandler : IRequestHandler<UpdateIzinOnayTanimWithCalisanlarCommand, UpdatedIzinOnayTanimWithCalisanlarResponse>
        {
            private readonly IMapper _mapper;
            private readonly IIzinOnayTanimlariRepository _izinOnayTanimlariRepository;

            public UpdateIzinOnayTanimWithCalisanlarCommandHandler(IMapper mapper, IIzinOnayTanimlariRepository izinOnayTanimlariRepository)
            {
                _mapper = mapper;
                _izinOnayTanimlariRepository = izinOnayTanimlariRepository;
            }

            public async Task<UpdatedIzinOnayTanimWithCalisanlarResponse> Handle(UpdateIzinOnayTanimWithCalisanlarCommand request, CancellationToken cancellationToken)
            {
                IzinOnayTanim izinOnayTanim = await _izinOnayTanimlariRepository.GetAsync(predicate: x => x.Id == request.Id,
                                                                                          cancellationToken: cancellationToken)
                                    ?? throw new ClientsideException("Belirtilen ID ' ye sahip izin onay tanımı bulunamadı!");

                IzinOnayTanim mappedIzinOnayTanim = _mapper.Map<IzinOnayTanim>(request);

                bool isSuccessfull = await _izinOnayTanimlariRepository.UpdateIzinOnayTanimWithCalisanlar(request.CalisanIdleri, mappedIzinOnayTanim, cancellationToken);
                if (!isSuccessfull) throw new ClientsideException("İzin onay tanımları güncellenirken bir hata meydana geldi!");

                UpdatedIzinOnayTanimWithCalisanlarResponse response = _mapper.Map<UpdatedIzinOnayTanimWithCalisanlarResponse>(mappedIzinOnayTanim);

                return response;
            }
        }
    }
}
