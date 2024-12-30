using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Isyerleri.Queries.GetIsyerleriSelectList
{
    public class GetIsyerleriSelectListQuery : IRequest<List<GetIsyerleriSelectListResponse>>
    {
        public string Search { get; set; }

        public class GetIsyerleriSelectListQueryHandler : IRequestHandler<GetIsyerleriSelectListQuery, List<GetIsyerleriSelectListResponse>>
        {
            private readonly IIsyerleriRepository _isyerleriRepository;

            public GetIsyerleriSelectListQueryHandler(IIsyerleriRepository isyerleriRepository)
            {
                _isyerleriRepository = isyerleriRepository;
            }

            public async Task<List<GetIsyerleriSelectListResponse>> Handle(GetIsyerleriSelectListQuery request, CancellationToken cancellationToken)
            {
                if (request?.Search == null)
                    return new List<GetIsyerleriSelectListResponse>();

                List<GetIsyerleriSelectListResponse> response = (await _isyerleriRepository.GetAllAsync(cancellationToken: cancellationToken))
                                                        .AsQueryable()
                                                        .Where(x => x.Aciklama.ToLower().Contains(request.Search.ToLower()))
                                                        .Select(x => new GetIsyerleriSelectListResponse()
                                                        {
                                                            Id = x.Id.ToString(),
                                                            Text = x.Aciklama
                                                        })
                                                        .Take(5)
                                                        .ToList();

                return response;
            }
        }
    }
}
