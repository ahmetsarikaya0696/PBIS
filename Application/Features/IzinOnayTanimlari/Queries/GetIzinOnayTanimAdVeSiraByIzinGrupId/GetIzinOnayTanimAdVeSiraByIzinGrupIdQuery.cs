using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.IzinOnayTanimlari.Queries.GetIzinOnayTanimAdVeSiraByIzinGrupId
{
    public class GetIzinOnayTanimAdVeSiraByIzinGrupIdQuery : IRequest<List<GetIzinOnayTanimAdVeSiraByIzinGrupIdResponse>>
    {
        public int IzinGrupId { get; set; }

        public class GetIzinOnayTanimAdVeSiraByIzinGrupIdQueryHandler : IRequestHandler<GetIzinOnayTanimAdVeSiraByIzinGrupIdQuery, List<GetIzinOnayTanimAdVeSiraByIzinGrupIdResponse>>
        {
            private readonly IIzinGrupIzinOnayTanimRepository _izinGrupIzinOnayTanimRepository;

            public GetIzinOnayTanimAdVeSiraByIzinGrupIdQueryHandler(IIzinGrupIzinOnayTanimRepository izinGrupIzinOnayTanimRepository)
            {
                _izinGrupIzinOnayTanimRepository = izinGrupIzinOnayTanimRepository;
            }

            public async Task<List<GetIzinOnayTanimAdVeSiraByIzinGrupIdResponse>> Handle(GetIzinOnayTanimAdVeSiraByIzinGrupIdQuery request, CancellationToken cancellationToken)
            {
                List<IzinGrupIzinOnayTanim> izinGrupIzinOnayTanim = await _izinGrupIzinOnayTanimRepository.GetAllAsync(predicate: x => x.IzinGrupId == request.IzinGrupId,
                                                                                                                       include: source => source.Include(x => x.IzinOnayTanim),
                                                                                                                       orderBy: source => source.OrderBy(x => x.OnayTanimSirasi),
                                                                                                                       cancellationToken: cancellationToken);

                return izinGrupIzinOnayTanim.Select(x => new GetIzinOnayTanimAdVeSiraByIzinGrupIdResponse() { IzinOnayTanim = x.IzinOnayTanim.Aciklama, Sira = x.OnayTanimSirasi })
                                            .ToList();
            }
        }
    }
}
