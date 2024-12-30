using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.RetSebepleri.Commands.Update
{
    public class UpdateRetSebepCommand : IRequest<UpdatedRetSebepResponse>
    {
        public int Id { get; set; }
        public string Aciklama { get; set; }
        public bool Duzenlenebilir { get; set; }
        public bool Aktif { get; set; }

        public class UpdateRetSebepCommandHandler : IRequestHandler<UpdateRetSebepCommand, UpdatedRetSebepResponse>
        {
            private readonly IRetDetaylariRepository _retDetaylariRepository;
            private readonly IRetSebepleriRepository _retSebepleriRepository;
            private readonly IMapper _mapper;

            public UpdateRetSebepCommandHandler(IRetDetaylariRepository retDetaylariRepository, IRetSebepleriRepository retSebepleriRepository, IMapper mapper)
            {
                _retDetaylariRepository = retDetaylariRepository;
                _retSebepleriRepository = retSebepleriRepository;
                _mapper = mapper;
            }

            public async Task<UpdatedRetSebepResponse> Handle(UpdateRetSebepCommand request, CancellationToken cancellationToken)
            {
                var retSebebi = await _retSebepleriRepository.GetAsync(predicate: x => x.Id == request.Id,
                                                                       cancellationToken: cancellationToken)
                                                        ?? throw new ClientsideException("Belirtilen ID ' ye sahip ret sebebi bulunamadı!");


                // İlişkili veri varsa ve önceki açıklama ile yeni açıklama farklıysa error fırlat
                bool iliskiliVeriVar = await _retDetaylariRepository.AnyAsync(predicate: x => x.RetSebepId == request.Id, cancellationToken: cancellationToken);

                string oncekiAciklama = (await _retSebepleriRepository.GetAsync(predicate: x => x.Id == request.Id,
                                                                                cancellationToken: cancellationToken)).Aciklama;

                bool aciklamaDegisti = oncekiAciklama != request.Aciklama;

                if (aciklamaDegisti && iliskiliVeriVar)
                    throw new ClientsideException("İlişkili verisi bulunan ret sebebinin \"Açıklama\" alanı değiştirilemez!");

                var mappedRetSebep = _mapper.Map<RetSebep>(request);

                var updatedRetSebep = await _retSebepleriRepository.UpdateAsync(mappedRetSebep);

                var response = _mapper.Map<UpdatedRetSebepResponse>(request);

                return response;
            }
        }
    }
}
