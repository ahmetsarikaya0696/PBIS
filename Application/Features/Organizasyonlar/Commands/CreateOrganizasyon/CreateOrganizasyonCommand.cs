using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.OrganizasyonSemasi.Commands.CreateOrganizasyon
{
    public class CreateOrganizasyonCommand : IRequest<CreateOrganizasyonResponse>
    {
        public string Aciklama_TR { get; set; }
        public string Aciklama_EN { get; set; }
        public int? OrganizasyonKodu { get; set; }
        public bool AnaBirim { get; set; }
        public bool Aktif { get; set; }
        public int? BirimId { get; set; }
        public int? UstBirimId { get; set; }

        public class CreateOrganizasyonCommandHandler : IRequestHandler<CreateOrganizasyonCommand, CreateOrganizasyonResponse>
        {
            private readonly IOrganizasyonlarRepository _organizasyonSemasiRepository;
            private readonly IMapper _mapper;

            public CreateOrganizasyonCommandHandler(IOrganizasyonlarRepository organizasyonSemasiRepository, IMapper mapper)
            {
                _organizasyonSemasiRepository = organizasyonSemasiRepository;
                _mapper = mapper;
            }

            public async Task<CreateOrganizasyonResponse> Handle(CreateOrganizasyonCommand request, CancellationToken cancellationToken)
            {
                // Organizasyon olustur
                var olusturulacakOrganizasyon = _mapper.Map<Organizasyon>(request);

                var olusturulanOrganizasyon = await _organizasyonSemasiRepository.AddAsync(olusturulacakOrganizasyon);

                var response = _mapper.Map<CreateOrganizasyonResponse>(olusturulanOrganizasyon);

                return response;
            }
        }
    }
}
