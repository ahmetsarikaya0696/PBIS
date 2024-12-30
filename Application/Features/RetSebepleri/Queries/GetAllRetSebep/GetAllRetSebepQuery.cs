using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.RetSebepleri.Queries.GetAllRetSebep
{
    public class GetAllRetSebepQuery : IRequest<List<GetAllRetSebepResponse>>
    {
        public bool PersonelSubeyeOzgu { get; set; }

        public class GetAllRetSebepQueryHandler : IRequestHandler<GetAllRetSebepQuery, List<GetAllRetSebepResponse>>
        {
            private readonly IRetSebepleriRepository _retSebepleriRepository;

            public GetAllRetSebepQueryHandler(IRetSebepleriRepository retSebepleriRepository)
            {
                _retSebepleriRepository = retSebepleriRepository;
            }

            public async Task<List<GetAllRetSebepResponse>> Handle(GetAllRetSebepQuery request, CancellationToken cancellationToken)
            {
                List<RetSebep> retSebepleri = await _retSebepleriRepository.GetAllAsync(x => x.Aktif, cancellationToken: cancellationToken);

                if (!request.PersonelSubeyeOzgu)
                {
                    retSebepleri = retSebepleri.Where(x => x.PersonelSubeyeOzgu == request.PersonelSubeyeOzgu).ToList();
                }

                return retSebepleri.Select(x => new GetAllRetSebepResponse()
                {
                    Id = x.Id,
                    Aciklama = x.Aciklama,
                }).ToList();
            }
        }
    }
}
