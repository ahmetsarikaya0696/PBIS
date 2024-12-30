using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.RetSebepleri.Commands.Create
{
    public class CreateRetSebepListCommand : IRequest<List<CreatedRetSebepResponse>>
    {
        public List<CreateRetSebepCommand> CreateRetSebepCommands { get; set; }

        public class CreateRetSebepListCommandHandler : IRequestHandler<CreateRetSebepListCommand, List<CreatedRetSebepResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IRetSebepleriRepository _retSebepleriRepository;

            public CreateRetSebepListCommandHandler(IMapper mapper, IRetSebepleriRepository retSebepleriRepository)
            {
                _mapper = mapper;
                _retSebepleriRepository = retSebepleriRepository;
            }

            public async Task<List<CreatedRetSebepResponse>> Handle(CreateRetSebepListCommand request, CancellationToken cancellationToken)
            {
                var retSebepleri = _mapper.Map<List<RetSebep>>(request?.CreateRetSebepCommands);

                var eklenenRetSebepleri = await _retSebepleriRepository.AddRangeAsync(retSebepleri);

                var response = _mapper.Map<List<CreatedRetSebepResponse>>(eklenenRetSebepleri);

                return response;
            }
        }
    }
}
