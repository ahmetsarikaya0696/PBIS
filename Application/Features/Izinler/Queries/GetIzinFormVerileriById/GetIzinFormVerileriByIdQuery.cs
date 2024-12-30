using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Izinler.Queries.GetIzinFormVerileriById
{
    public class GetIzinFormVerileriByIdQuery : IRequest<GetIzinFormVerileriByIdResponse>
    {
        public int Id { get; set; }

        public class GetIzinFormVerileriByIdQueryHandler : IRequestHandler<GetIzinFormVerileriByIdQuery, GetIzinFormVerileriByIdResponse>
        {
            private readonly IIzinlerRepository _izinlerRepository;

            private readonly IMapper _mapper;

            public GetIzinFormVerileriByIdQueryHandler(IIzinlerRepository izinlerRepository, IMapper mapper)
            {
                _izinlerRepository = izinlerRepository;
                _mapper = mapper;
            }

            public async Task<GetIzinFormVerileriByIdResponse> Handle(GetIzinFormVerileriByIdQuery request, CancellationToken cancellationToken)
            {
                Izin izin = await _izinlerRepository.GetAsync(predicate: x => x.Id == request.Id,
                                                              cancellationToken: cancellationToken);

                GetIzinFormVerileriByIdResponse response = _mapper.Map<GetIzinFormVerileriByIdResponse>(izin);

                return response;
            }
        }
    }
}
