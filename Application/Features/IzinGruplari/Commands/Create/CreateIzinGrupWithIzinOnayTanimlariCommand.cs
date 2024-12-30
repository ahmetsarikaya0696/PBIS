using Application.Exceptions;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.IzinGruplari.Commands.Create
{
    public class IzinOnayTanimIdWithSira
    {
        public int IzinOnayTanimId { get; set; }
        public int Sira { get; set; }
    }
    public class CreateIzinGrupWithIzinOnayTanimlariCommand : IRequest<bool>
    {
        public string Aciklama { get; set; }

        public int? CalisanId { get; set; }

        public int? UnvanId { get; set; }

        public int? BirimId { get; set; }

        public int? IsyeriId { get; set; }

        public DateTime BaslangicTarihi { get; set; }

        public DateTime BitisTarihi { get; set; }

        public List<IzinOnayTanimIdWithSira> IzinOnayTanimIdVeSiralari { get; set; }

        public class CreateIzinGrupWithIzinOnayTanimlariCommandHandler : IRequestHandler<CreateIzinGrupWithIzinOnayTanimlariCommand, bool>
        {
            private readonly IMapper _mapper;
            private readonly IIzinGruplariRepository _izinGruplariRepository;
            private readonly IIzinOnayTanimlariRepository _izinOnayTanimlariRepository;

            public CreateIzinGrupWithIzinOnayTanimlariCommandHandler(IMapper mapper, IIzinGruplariRepository izinGruplariRepository, IIzinOnayTanimlariRepository izinOnayTanimlariRepository)
            {
                _mapper = mapper;
                _izinGruplariRepository = izinGruplariRepository;
                _izinOnayTanimlariRepository = izinOnayTanimlariRepository;
            }

            public async Task<bool> Handle(CreateIzinGrupWithIzinOnayTanimlariCommand request, CancellationToken cancellationToken)
            {
                bool mevcutKayıtBulundu = await _izinGruplariRepository.AnyAsync(predicate: x => x.CalisanId == request.CalisanId &&
                                                                                                 x.UnvanId == request.UnvanId &&
                                                                                                 x.BirimId == request.BirimId &&
                                                                                                 x.IsyeriId == request.IsyeriId &&
                                                                                                (x.BaslangicTarihi <= DateTime.Now && x.BitisTarihi >= DateTime.Now),
                                                                                 cancellationToken: cancellationToken);

                if (mevcutKayıtBulundu) throw new ClientsideException("Mevcut niteliklere sahip aktif kayıt bulundu!");

                bool ayniAciklmayaSahipAktifVeriBulundu = await _izinGruplariRepository.AnyAsync(predicate: x => x.Aciklama.ToLower().Trim() == request.Aciklama.ToLower().Trim() &&
                                                                                                  (x.BaslangicTarihi <= DateTime.Now && x.BitisTarihi >= DateTime.Now),
                                                                                  cancellationToken: cancellationToken);

                if (ayniAciklmayaSahipAktifVeriBulundu) throw new ClientsideException($"\"{request.Aciklama}\" açıklamasıyla oluşturulmuş mevcut aktif izin grubu bulundu!");


                var izinOnayTanimIdVeSiralari = request.IzinOnayTanimIdVeSiralari;

                // Birinci Onay Personel Onayı Olmak Zorundadır
                int birinciIzinOnayTanimId = izinOnayTanimIdVeSiralari.Where(x => x.Sira == 1)
                                                                      .Select(x => x.IzinOnayTanimId)
                                                                      .First();

                bool birinciOnayPersonelOnayi = (await _izinOnayTanimlariRepository.GetAsync(predicate: x => x.Id == birinciIzinOnayTanimId, cancellationToken: cancellationToken)).PersonelSubeYetkisi;
                if (!birinciOnayPersonelOnayi) throw new ClientsideException("İlk onayın personel şube yetkisi olmak zorundadır!");

                // Birinci Onay Dışındaki Onay Tanımlarında Personel Yetkisi Var Mı
                List<int> izinOnayTanimIdleri = izinOnayTanimIdVeSiralari.Where(x => x.Sira > 1)
                                                                         .Select(x => x.IzinOnayTanimId)
                                                                         .ToList();
                foreach (var izinOnayTanimId in izinOnayTanimIdleri)
                {
                    bool personelSubeYetkisi = (await _izinOnayTanimlariRepository.GetAsync(predicate: x => x.Id == izinOnayTanimId, cancellationToken: cancellationToken)).PersonelSubeYetkisi;
                    if (personelSubeYetkisi) throw new ClientsideException("Personel Şube yetkisi sadece ilk izin onay tanımında olmalıdır!");
                }

                // 1 ' den fazla merkez müdürü onayı olamaz
                int merkezMudurYetkisiCount = 0;
                foreach (var izinOnayTanimIdVeSira in izinOnayTanimIdVeSiralari)
                {
                    int izinOnayTanimId = izinOnayTanimIdVeSira.IzinOnayTanimId;
                    int sira = izinOnayTanimIdVeSira.Sira;

                    bool merkezMudurYetkisi = (await _izinOnayTanimlariRepository.GetAsync(predicate: x => x.Id == izinOnayTanimId, cancellationToken: cancellationToken)).MerkezMuduruYetkisi;

                    if (merkezMudurYetkisi)
                    {
                        bool sonOnay = izinOnayTanimIdVeSiralari.OrderByDescending(x => x.Sira).FirstOrDefault().Sira == sira;
                        if (sonOnay) throw new ClientsideException("Merkez müdür yetkisi son onay olamaz!");

                        merkezMudurYetkisiCount++;
                    }
                }

                if (merkezMudurYetkisiCount > 1) throw new ClientsideException("Merkez müdür yetkisi yalnızca bir izin onay tanımında olabilir!");


                if (izinOnayTanimIdVeSiralari?.Count == 0) throw new ClientsideException("İzin onay tanım verisi girilmek zorundadır!");

                foreach (var item in izinOnayTanimIdVeSiralari)
                {
                    int sira = item.Sira;
                    int izinOnayTanimId = item.IzinOnayTanimId;

                    bool siraNotUnique = izinOnayTanimIdVeSiralari.Where(x => x.Sira == sira).Count() > 1;

                    if (siraNotUnique) throw new ClientsideException("Girilen izin onay tanım verilerinin sıraları benzersiz olmak zorundadır!");

                    bool izinOnayTanimIdNotUnique = izinOnayTanimIdVeSiralari.Where(x => x.IzinOnayTanimId == izinOnayTanimId).Count() > 1;

                    if (izinOnayTanimIdNotUnique) throw new ClientsideException("Girilen izin onay tanım verileri benzersiz olmak zorundadır!");
                }

                IzinGrup eklenecekIzinGrubu = _mapper.Map<IzinGrup>(request);

                bool response = await _izinGruplariRepository.UpdateIzinGrupWithIzinOnayTanimlariAsync(izinOnayTanimIdVeSiralari, eklenecekIzinGrubu);

                return response;
            }
        }
    }
}
