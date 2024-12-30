using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Tatiller.Queries.GetTatillerByTarih
{
    public class GetTatillerByTarihQuery : IRequest<List<GetTatillerByTarihResponse>>
    {
        public List<DateTime> Tarihler { get; set; }

        public class GetTatillerByTarihQueryHandler : IRequestHandler<GetTatillerByTarihQuery, List<GetTatillerByTarihResponse>>
        {
            private readonly ITatillerRepository _tatillerRepository;
            private readonly IMapper _mapper;

            public GetTatillerByTarihQueryHandler(ITatillerRepository tatillerRepository, IMapper mapper)
            {
                _tatillerRepository = tatillerRepository;
                _mapper = mapper;
            }

            public async Task<List<GetTatillerByTarihResponse>> Handle(GetTatillerByTarihQuery request, CancellationToken cancellationToken)
            {
                List<Tatil> tatiller = new();

                foreach (var tarih in request.Tarihler)
                {
                    var tatil = await _tatillerRepository.GetAsync(predicate: x => x.Tarih == tarih, cancellationToken: cancellationToken);

                    if (tatil != null)
                        tatiller.Add(tatil);
                }

                var response = _mapper.Map<List<GetTatillerByTarihResponse>>(tatiller);
                return response;
            }
        }
    }
}
