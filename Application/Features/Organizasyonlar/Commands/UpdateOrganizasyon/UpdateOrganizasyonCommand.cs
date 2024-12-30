using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Organizasyonlar.Commands.UpdateOrganizasyon
{
    public class UpdateOrganizasyonCommand : IRequest<UpdatedOrganizasyonResponse>
    {
        public int Id { get; set; }
        public string Kod { get; set; }
        public string Aciklama_TR { get; set; }
        public string Aciklama_EN { get; set; }
        public int? OrganizasyonKodu { get; set; }
        public bool AnaBirim { get; set; }
        public bool Aktif { get; set; }
        public int? BirimId { get; set; }
        public int? UstBirimId { get; set; }

        public class UpdateOrganizasyonCommandHandler : IRequestHandler<UpdateOrganizasyonCommand, UpdatedOrganizasyonResponse>
        {
            private readonly IOrganizasyonlarRepository _organizasyonlarRepository;
            private readonly IMapper _mapper;

            public UpdateOrganizasyonCommandHandler(IOrganizasyonlarRepository organizasyonlarRepository, IMapper mapper)
            {
                _organizasyonlarRepository = organizasyonlarRepository;
                _mapper = mapper;
            }

            public async Task<UpdatedOrganizasyonResponse> Handle(UpdateOrganizasyonCommand request, CancellationToken cancellationToken)
            {
                Organizasyon guncellenecekOrganizasyon = _mapper.Map<Organizasyon>(request);
                Organizasyon guncellenenOrganizasyon = await _organizasyonlarRepository.UpdateAsync(guncellenecekOrganizasyon);

                return new UpdatedOrganizasyonResponse()
                {
                    Id = guncellenecekOrganizasyon.Id,
                    Aciklama_TR = guncellenenOrganizasyon.Aciklama_TR,
                    Aktif = guncellenenOrganizasyon.Aktif
                };
            }
        }
    }
}
