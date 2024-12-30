using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Features.RetSebepleri.Commands.Delete
{
    public class DeleteRetSebepByIdCommand : IRequest<string>
    {
        public int Id { get; set; }

        public class DeleteRetSebepByIdCommandHandler : IRequestHandler<DeleteRetSebepByIdCommand, string>
        {
            private readonly IRetDetaylariRepository _retDetaylariRepository;
            private readonly IRetSebepleriRepository _retSebepleriRepository;

            public DeleteRetSebepByIdCommandHandler(IRetDetaylariRepository retDetaylariRepository, IRetSebepleriRepository retSebepleriRepository)
            {
                _retDetaylariRepository = retDetaylariRepository;
                _retSebepleriRepository = retSebepleriRepository;
            }

            public async Task<string> Handle(DeleteRetSebepByIdCommand request, CancellationToken cancellationToken)
            {
                // Belirtilen Id ' ye ait veri var mı
                RetSebep retSebebi = await _retSebepleriRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken)
                                            ?? throw new ClientsideException("Belirtilen ID ' ye sahip veri bulunamadı!");

                // Ret Detaylari tablosunda ilişkili veri var mı
                bool iliskiliVeriVar = await _retDetaylariRepository.AnyAsync(predicate: x => x.RetSebepId == request.Id, cancellationToken: cancellationToken);

                if (iliskiliVeriVar)
                    throw new ClientsideException("İlişkili veri bulundu!");

                RetSebep silinecekRetSebebi = await _retSebepleriRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken);

                RetSebep silinmisRetSebebi = await _retSebepleriRepository.DeleteAsync(silinecekRetSebebi);

                return silinmisRetSebebi.Aciklama;
            }
        }
    }
}
