using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinTurleri.Queries.GetIzinTurById
{
    public class GetIzinTurByIdQuery : IRequest<GetIzinTurByIdResponse>
    {
        public int Id { get; set; }

        public class GetIzinTurByIdQueryHandler : IRequestHandler<GetIzinTurByIdQuery, GetIzinTurByIdResponse>
        {
            private readonly IIzinTurleriRepository _izinTurleriRepository;
            private readonly IMapper _mapper;

            public GetIzinTurByIdQueryHandler(IIzinTurleriRepository izinTurleriRepository, IMapper mapper)
            {
                _izinTurleriRepository = izinTurleriRepository;
                _mapper = mapper;
            }

            public async Task<GetIzinTurByIdResponse> Handle(GetIzinTurByIdQuery request, CancellationToken cancellationToken)
            {
                IzinTur izinTur = await _izinTurleriRepository.GetAsync(predicate: x => x.Id == request.Id && x.Aktif == true, cancellationToken: cancellationToken);
                return _mapper.Map<GetIzinTurByIdResponse>(izinTur);
            }
        }
    }
}
