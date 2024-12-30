using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.RetSebepleri.Queries.GetRetSebepleriByAciklama
{
    public class GetRetSebepleriByAciklamaQuery : IRequest<List<string>>
    {
        public List<string> Aciklamalar { get; set; }

        public class GetRetSebepleriByAciklamaQueryHandler : IRequestHandler<GetRetSebepleriByAciklamaQuery, List<string>>
        {
            private readonly IRetSebepleriRepository _retSebepleriRepository;

            public GetRetSebepleriByAciklamaQueryHandler(IRetSebepleriRepository retSebepleriRepository)
            {
                _retSebepleriRepository = retSebepleriRepository;
            }

            public async Task<List<string>> Handle(GetRetSebepleriByAciklamaQuery request, CancellationToken cancellationToken)
            {
                List<string> result = new();

                foreach (var aciklama in request.Aciklamalar)
                {
                    var retSebebi = await _retSebepleriRepository.GetAsync(predicate: x => x.Aciklama.ToLower() == aciklama.ToLower().Trim(), cancellationToken: cancellationToken);

                    if (retSebebi != null)
                        result.Add(aciklama.Trim());
                }

                return result;
            }
        }
    }
}
