using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Tatiller.Commands.Create
{
    public class CreateTatilCommand : IRequest<CreatedTatilResponse>
    {
        public DateTime Tarih { get; set; }
        public string Aciklama { get; set; }

        public class CreateTatilCommandHandler : IRequestHandler<CreateTatilCommand, CreatedTatilResponse>
        {
            private readonly IMapper _mapper;
            private readonly ITatillerRepository _tatillerRepository;

            public CreateTatilCommandHandler(IMapper mapper, ITatillerRepository tatillerRepository)
            {
                _mapper = mapper;
                _tatillerRepository = tatillerRepository;
            }

            public async Task<CreatedTatilResponse> Handle(CreateTatilCommand request, CancellationToken cancellationToken)
            {
                bool exist = await _tatillerRepository.GetAsync(predicate: x => x.Tarih == request.Tarih, cancellationToken: cancellationToken) != null;
                if (exist) throw new ClientsideException($"\"{request.Tarih:dd.MM.yyyy}\" tarihiyle eklenmiş mevcut bir tatil günü var!");

                var tatil = _mapper.Map<Tatil>(request);

                var eklenenTatil = await _tatillerRepository.AddAsync(tatil);

                return _mapper.Map<CreatedTatilResponse>(eklenenTatil);
            }
        }
    }
}
