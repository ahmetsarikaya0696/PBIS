using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.Organizasyonlar.Commands.DeleteOrganizasyon
{
    public class DeleteOrganizasyonByIdCommand : IRequest<DeleteOrganizasyonByIdResponse>
    {
        public int Id { get; set; }

        public class DeleteOrganizasyonByIdCommandHandler : IRequestHandler<DeleteOrganizasyonByIdCommand, DeleteOrganizasyonByIdResponse>
        {
            private readonly IOrganizasyonlarRepository _organizasyonlarRepository;
            private readonly IMapper _mapper;

            public DeleteOrganizasyonByIdCommandHandler(IOrganizasyonlarRepository organizasyonlarRepository, IMapper mapper)
            {
                _organizasyonlarRepository = organizasyonlarRepository;
                _mapper = mapper;
            }

            public async Task<DeleteOrganizasyonByIdResponse> Handle(DeleteOrganizasyonByIdCommand request, CancellationToken cancellationToken)
            {
                var silinecekOrganizasyon = await _organizasyonlarRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);

                var silinecekOrganizasyonlar = await _organizasyonlarRepository.AltBirimleriBulAsync(silinecekOrganizasyon.Id, cancellationToken: cancellationToken);
                silinecekOrganizasyonlar.Add(silinecekOrganizasyon);

                foreach (var organizasyon in silinecekOrganizasyonlar)
                {
                    await _organizasyonlarRepository.DeleteAsync(organizasyon);
                }

                var response = _mapper.Map<DeleteOrganizasyonByIdResponse>(silinecekOrganizasyon);

                return response;

            }
        }
    }
}
