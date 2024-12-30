using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.RetSebepleri.Queries.GetRetDetayByIzinHareketId
{
    public class GetRetDetayByIzinHareketIdQuery : IRequest<GetRetDetayByIzinHareketIdResponse>
    {
        public int Id { get; set; }

        public class GetRetDetayByIzinHareketIdQueryHandler : IRequestHandler<GetRetDetayByIzinHareketIdQuery, GetRetDetayByIzinHareketIdResponse>
        {
            private readonly IMapper _mapper;
            private readonly IRetDetaylariRepository _retDetaylariRepository;

            public GetRetDetayByIzinHareketIdQueryHandler(IMapper mapper, IRetDetaylariRepository retDetaylariRepository)
            {
                _mapper = mapper;
                _retDetaylariRepository = retDetaylariRepository;
            }

            public async Task<GetRetDetayByIzinHareketIdResponse> Handle(GetRetDetayByIzinHareketIdQuery request, CancellationToken cancellationToken)
            {
                RetDetay retDetay = await _retDetaylariRepository.GetAsync(predicate: x => x.IzinHareketId == request.Id,
                                                                           include: source => source.Include(x => x.RetSebep),
                                                                           cancellationToken: cancellationToken); ;

                var response = _mapper.Map<GetRetDetayByIzinHareketIdResponse>(retDetay);

                return response;
            }
        }
    }
}
