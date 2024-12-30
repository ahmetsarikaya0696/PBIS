using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tatiller.Commands.Delete
{
    public class DeleteTatilByIdCommand : IRequest<DeletedTatilByIdResponse>
    {
        public int Id { get; set; }

        public class DeleteTatilByIdCommandHanler : IRequestHandler<DeleteTatilByIdCommand, DeletedTatilByIdResponse>
        {
            private readonly ITatillerRepository _tatillerRepository;
            private readonly IIzinlerRepository _izinlerRepository;
            private readonly IMapper _mapper;

            public DeleteTatilByIdCommandHanler(ITatillerRepository tatillerRepository, IIzinlerRepository izinlerRepository, IMapper mapper)
            {
                _tatillerRepository = tatillerRepository;
                _izinlerRepository = izinlerRepository;
                _mapper = mapper;
            }

            public async Task<DeletedTatilByIdResponse> Handle(DeleteTatilByIdCommand request, CancellationToken cancellationToken)
            {
                Tatil tatil = await _tatillerRepository.GetAsync(predicate: x => x.Id == request.Id, cancellationToken: cancellationToken)
                                    ?? throw new ClientsideException("Belirtilen ID ' ye sahip veri bulunamadı!");


                var iliskiliIzinler = await _izinlerRepository.GetAllAsync(predicate: x => x.BaslangicTarihi <= tatil.Tarih && x.BitisTarihi >= tatil.Tarih && !x.IzinTur.TatilGunleriSayilir,
                                                                           include: source => source.Include(x => x.IzinTur),
                                                                           cancellationToken: cancellationToken);

                bool iliskiliKayitVar = iliskiliIzinler?.Count > 0;

                if (iliskiliKayitVar) throw new ClientsideException("İlişkili kayıt bulundu!");

                Tatil deletedTatil = await _tatillerRepository.DeleteAsync(tatil);

                return _mapper.Map<DeletedTatilByIdResponse>(deletedTatil);
            }
        }
    }
}
