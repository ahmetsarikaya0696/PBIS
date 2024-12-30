using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinOnayTanimlari.Queries.GetIzinOnayTanimSelectListItems
{
    public class GetIzinOnayTanimSelectListItemsQuery : IRequest<List<GetIzinOnayTanimSelectListItemsResponse>>
    {
        public class GetIzinOnayTanimSelectListItemsQueryHandler : IRequestHandler<GetIzinOnayTanimSelectListItemsQuery, List<GetIzinOnayTanimSelectListItemsResponse>>
        {
            private readonly IIzinOnayTanimlariRepository _izinOnayTanimlariRepository;

            public GetIzinOnayTanimSelectListItemsQueryHandler(IIzinOnayTanimlariRepository izinOnayTanimlariRepository)
            {
                _izinOnayTanimlariRepository = izinOnayTanimlariRepository;
            }

            public async Task<List<GetIzinOnayTanimSelectListItemsResponse>> Handle(GetIzinOnayTanimSelectListItemsQuery request, CancellationToken cancellationToken)
            {
                List<IzinOnayTanim> izinOnayTanimlari = await _izinOnayTanimlariRepository.GetAllAsync(predicate: x => x.Aktif, cancellationToken: cancellationToken);

                List<GetIzinOnayTanimSelectListItemsResponse> response = izinOnayTanimlari.Select(x => new GetIzinOnayTanimSelectListItemsResponse()
                {
                    Id = x.Id,
                    Aciklama = x.Aciklama,
                }).ToList();

                return response;
            }
        }
    }
}
