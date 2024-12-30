using Application.Interfaces.Services;
using MediatR;

namespace Application.Features.Dogrulamalar.Commands.Create
{
    public class CreateDogrulamaCommand : IRequest<CreatedDogrulamaResponse>
    {
        public int CalisanId { get; set; }
        public string Yontem { get; set; }

        public class CreateDogrulamaCommandHandler : IRequestHandler<CreateDogrulamaCommand, CreatedDogrulamaResponse>
        {
            private readonly IDogrulamaService _dogrulamaService;

            public CreateDogrulamaCommandHandler(IDogrulamaService dogrulamaService)
            {
                _dogrulamaService = dogrulamaService;
            }

            public async Task<CreatedDogrulamaResponse> Handle(CreateDogrulamaCommand request, CancellationToken cancellationToken)
            {

                var response = await _dogrulamaService.Gonder(request.CalisanId, request.Yontem, cancellationToken);
                return response;
            }
        }
    }
}
