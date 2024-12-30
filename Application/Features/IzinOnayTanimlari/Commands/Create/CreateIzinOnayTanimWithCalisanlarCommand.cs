using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinOnayTanimlari.Commands.Create
{
    public class CreateIzinOnayTanimWithCalisanlarCommand : IRequest<CreatedIzinOnayTanimWithCalisanlarResponse>
    {
        public string Aciklama { get; set; }

        public bool MerkezMuduruYetkisi { get; set; }

        public bool PersonelSubeYetkisi { get; set; }

        public bool Aktif { get; set; }

        public List<int> CalisanIdleri { get; set; }

        public class CreateIzinOnayTanimWithCalisanlarQueryHandler : IRequestHandler<CreateIzinOnayTanimWithCalisanlarCommand, CreatedIzinOnayTanimWithCalisanlarResponse>
        {
            private readonly IMapper _mapper;
            private readonly IIzinOnayTanimlariRepository _izinOnayTanimlariRepository;

            public CreateIzinOnayTanimWithCalisanlarQueryHandler(IMapper mapper, IIzinOnayTanimlariRepository izinOnayTanimlariRepository)
            {
                _mapper = mapper;
                _izinOnayTanimlariRepository = izinOnayTanimlariRepository;
            }

            public async Task<CreatedIzinOnayTanimWithCalisanlarResponse> Handle(CreateIzinOnayTanimWithCalisanlarCommand request, CancellationToken cancellationToken)
            {
                bool aciklamaUniqueDegil = await _izinOnayTanimlariRepository.GetAsync(predicate: x => x.Aciklama == request.Aciklama, cancellationToken: cancellationToken) != null;

                if (aciklamaUniqueDegil) throw new ClientsideException($"\"{request.Aciklama}\" açıklamasıyla oluşturulmuş mevcut bir veri var! Veri eklenemedi!");

                IzinOnayTanim mappedIzinOnayTanim = _mapper.Map<IzinOnayTanim>(request);

                bool isSuccessfull = await _izinOnayTanimlariRepository.CreateIzinOnayTanimWithCalisanlar(request.CalisanIdleri, mappedIzinOnayTanim);
                if (!isSuccessfull) throw new ClientsideException("İzin onay tanımı oluşturulurken bir hata meydana geldi!");

                var response = _mapper.Map<CreatedIzinOnayTanimWithCalisanlarResponse>(mappedIzinOnayTanim);

                return response;
            }
        }
    }
}
