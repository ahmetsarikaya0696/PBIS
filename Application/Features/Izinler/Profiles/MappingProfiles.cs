using Application.Features.Izinler.Commands.Create;
using Application.Features.Izinler.Commands.Onayla;
using Application.Features.Izinler.Commands.Reddet;
using Application.Features.Izinler.Commands.Update;
using Application.Features.Izinler.Queries.GetByCalisanId;
using Application.Features.Izinler.Queries.GetIslemYapilanIzinler;
using Application.Features.Izinler.Queries.GetIzinDetayById;
using Application.Features.Izinler.Queries.GetIzinFormTabVerileriById;
using Application.Features.Izinler.Queries.GetIzinFormVerileriById;
using Application.Features.Izinler.Queries.GetOnaylanacakIzinler;
using Application.Features.Izinler.Queries.GetSuankiIzinByCalisanId;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Features.Izinler.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Calisan, Izin>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<CreateIzinCommand, Izin>()
                .ForMember(dest => dest.IzinDurumId, opt => opt.MapFrom(src => IzinDurumEnum.Beklemede))
                .ForMember(dest => dest.IstekTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Telefon, opt => opt.MapFrom(src => src.Telefon.Replace("(", "").Replace(")", "").Replace(" ", "")));

            CreateMap<Izin, CreatedIzinResponse>();

            CreateMap<Izin, GetByCalisanIdIzinResponse>()
                .ForMember(dest => dest.IzinTur, opt => opt.MapFrom(src => src.IzinTur.Aciklama))
                .ForMember(dest => dest.IstekTarihi, opt => opt.MapFrom(src => src.IstekTarihi.ToString("dd.MM.yyyy HH:mm:ss")))
                .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => src.BaslangicTarihi.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => src.BitisTarihi.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.IseBaslamaTarihi, opt => opt.MapFrom(src => src.IseBaslamaTarihi.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.IzinDurumu, opt => opt.MapFrom(src => src.IzinDurum.Aciklama))
                .ForMember(dest => dest.IsForm, opt => opt.MapFrom(src => src.IzinTur.IzinFormTipi == 'F'))
                .ForMember(dest => dest.SabitGunSayisi, opt => opt.MapFrom(src => src.IzinTur.SabitGunSayisi));

            CreateMap<Izin, GetOnaylanacakIzinlerResponse>()
                .ForMember(dest => dest.IstekTarihi, opt => opt.MapFrom(src => src.IstekTarihi.ToString("dd.MM.yyyy HH:mm:ss")))
                .ForMember(dest => dest.Unvan, opt => opt.MapFrom(src => src.Unvan.Aciklama))
                .ForMember(dest => dest.AdSoyad, opt => opt.MapFrom(src => $"{src.Calisan.Ad} {src.Calisan.Soyad}"))
                .ForMember(dest => dest.IzinDurumu, opt => opt.MapFrom(src => src.IzinDurum.Aciklama))
                .ForMember(dest => dest.Birim, opt => opt.MapFrom(src => src.Birim.Aciklama));

            CreateMap<OnaylaIzinCommand, IzinHareket>()
                .ForMember(dest => dest.IslemTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IzinDurumId, opt => opt.MapFrom(src => (int)IzinDurumEnum.Onaylandi));

            CreateMap<IzinHareket, OnaylaIzinResponse>()
                .ForMember(dest => dest.IsyeriAdi, opt => opt.MapFrom(src => src.Calisan.Isyeri.Aciklama))
                .ForMember(dest => dest.BirimAdi, opt => opt.MapFrom(src => src.Calisan.Birim.Aciklama))
                .ForMember(dest => dest.AdSoyad, opt => opt.MapFrom(src => $"{src.Calisan.Ad} {src.Calisan.Soyad}"))
                .ForMember(dest => dest.IzinBaslangic, opt => opt.MapFrom(src => src.Izin.BaslangicTarihi.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.IzinBitis, opt => opt.MapFrom(src => src.Izin.BitisTarihi.ToString("dd.MM.yyyy")));

            CreateMap<Izin, GetIslemYapilanIzinlerResponse>()
                .ForMember(dest => dest.Unvan, opt => opt.MapFrom(src => src.Unvan.Aciklama))
                .ForMember(dest => dest.AdSoyad, opt => opt.MapFrom(src => $"{src.Calisan.Ad} {src.Calisan.Soyad}"))
                .ForMember(dest => dest.Birim, opt => opt.MapFrom(src => src.Calisan.Birim.Aciklama))
                .ForMember(dest => dest.GenelIzinDurumu, opt => opt.MapFrom(src => src.IzinDurum.Aciklama));

            CreateMap<Izin, GetIzinDetayByIdResponse>()
                .ForMember(dest => dest.IzinTur, opt => opt.MapFrom(src => src.IzinTur.Aciklama))
                .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => src.BaslangicTarihi.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => src.BitisTarihi.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.MahsubenBaslangicTarihi, opt => opt.MapFrom(src => src.MahsubenBaslangicTarihi.HasValue ? src.MahsubenBaslangicTarihi.Value.ToString("dd.MM.yyyy") : null))
                .ForMember(dest => dest.IseBaslamaTarihi, opt => opt.MapFrom(src => src.IseBaslamaTarihi.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.Birim, opt => opt.MapFrom(src => src.Birim.Aciklama))
                .ForMember(dest => dest.Unvan, opt => opt.MapFrom(src => src.Unvan.Aciklama))
                .ForMember(dest => dest.AdSoyad, opt => opt.MapFrom(src => $"{src.Calisan.Ad} {src.Calisan.Soyad}"))
                .ForMember(dest => dest.SicilNo, opt => opt.MapFrom(src => src.Calisan.SicilNo.ToString()))
                .ForMember(dest => dest.YillikIzinUcretiIstegi, opt => opt.MapFrom(src => src.YillikIzinUcretiIstegi ? "Evet" : "Hayır"))
                .ForMember(dest => dest.SenelikIzinMi, opt => opt.MapFrom(src => src.IzinTur.SenelikIzinMi));

            CreateMap<Izin, GetIzinFormTabVerileriByIdResponse>()
               .ForMember(dest => dest.IzinTur, opt => opt.MapFrom(src => src.IzinTur.Aciklama))
               .ForMember(dest => dest.IstekTarihi, opt => opt.MapFrom(src => src.IstekTarihi.ToString("dd.MM.yyyy HH:mm:ss")))
               .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => src.BaslangicTarihi.ToString("dd.MM.yyyy")))
               .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => src.BitisTarihi.ToString("dd.MM.yyyy")))
               .ForMember(dest => dest.MahsubenBaslangicTarihi, opt => opt.MapFrom(src => src.MahsubenBaslangicTarihi.HasValue ? src.MahsubenBaslangicTarihi.Value.ToString("dd.MM.yyyy") : null))
               .ForMember(dest => dest.IseBaslamaTarihi, opt => opt.MapFrom(src => src.IseBaslamaTarihi.ToString("dd.MM.yyyy")))
               .ForMember(dest => dest.IzinDurumu, opt => opt.MapFrom(src => src.IzinDurum.Aciklama))
               .ForMember(dest => dest.Isyeri, opt => opt.MapFrom(src => src.Isyeri.Aciklama))
               .ForMember(dest => dest.Birim, opt => opt.MapFrom(src => src.Birim.Aciklama))
               .ForMember(dest => dest.Unvan, opt => opt.MapFrom(src => src.Unvan.Aciklama))
               .ForMember(dest => dest.AdSoyad, opt => opt.MapFrom(src => $"{src.Calisan.Ad} {src.Calisan.Soyad}"))
               .ForMember(dest => dest.SicilNo, opt => opt.MapFrom(src => src.Calisan.SicilNo.ToString()))
               .ForMember(dest => dest.YillikIzinUcretiIstegi, opt => opt.MapFrom(src => src.YillikIzinUcretiIstegi ? "Evet" : "Hayır"))
               .ForMember(dest => dest.FormTip, opt => opt.MapFrom(src => src.IzinTur.IzinFormTipi));

            CreateMap<Izin, GetIzinFormVerileriByIdResponse>()
               .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => src.BaslangicTarihi.ToString("dd.MM.yyyy")))
               .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => src.BitisTarihi.ToString("dd.MM.yyyy")))
               .ForMember(dest => dest.IseBaslamaTarihi, opt => opt.MapFrom(src => src.IseBaslamaTarihi.ToString("dd.MM.yyyy")))
               .ForMember(dest => dest.MahsubenBaslangicTarihi, opt => opt.MapFrom(src => src.MahsubenBaslangicTarihi != null ? src.MahsubenBaslangicTarihi.Value.ToString("dd.MM.yyyy") : null));

            CreateMap<ReddetIzinCommand, IzinHareket>()
                .ForMember(dest => dest.IslemTarihi, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IzinDurumId, opt => opt.MapFrom(src => (int)IzinDurumEnum.Duzeltilmek_Uzere_Geri_Gonderildi));

            CreateMap<IzinHareket, ReddetIzinResponse>()
               .ForMember(dest => dest.IsyeriAdi, opt => opt.MapFrom(src => src.Izin.Isyeri.Aciklama))
               .ForMember(dest => dest.BirimAdi, opt => opt.MapFrom(src => src.Izin.Birim.Aciklama))
               .ForMember(dest => dest.AdSoyad, opt => opt.MapFrom(src => $"{src.Izin.Calisan.Ad} {src.Calisan.Soyad}"))
               .ForMember(dest => dest.IzinBaslangic, opt => opt.MapFrom(src => src.Izin.BaslangicTarihi.ToString("dd.MM.yyyy")))
               .ForMember(dest => dest.IzinBitis, opt => opt.MapFrom(src => src.Izin.BitisTarihi.ToString("dd.MM.yyyy")));

            CreateMap<Izin, KesisenIzin>()
               .ForMember(dest => dest.Unvan, opt => opt.MapFrom(src => src.Unvan.Aciklama))
               .ForMember(dest => dest.AdSoyad, opt => opt.MapFrom(src => $"{src.Calisan.Ad} {src.Calisan.Soyad}"))
               .ForMember(dest => dest.IzinDurumu, opt => opt.MapFrom(src => src.IzinDurum.Aciklama))
               .ForMember(dest => dest.IzinTur, opt => opt.MapFrom(src => src.IzinTur.Aciklama))
               .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => src.BaslangicTarihi.ToString("dd.MM.yyyy")))
               .ForMember(dest => dest.IseBaslamaTarihi, opt => opt.MapFrom(src => src.IseBaslamaTarihi.ToString("dd.MM.yyyy")));

            CreateMap<UpdateIzinCommand, Izin>()
               .ForMember(dest => dest.IzinDurumId, opt => opt.MapFrom(src => (int)IzinDurumEnum.Beklemede));

            CreateMap<Izin, IzinHareket>()
              .ForMember(dest => dest.Id, opt => opt.Ignore())
              .ForMember(dest => dest.IzinId, opt => opt.MapFrom(src => src.Id));

            CreateMap<UpdateIzinCommand, IzinHareket>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.IslemTarihi, opt => opt.MapFrom(src => DateTime.Now))
               .ForMember(dest => dest.IzinDurumId, opt => opt.MapFrom(src => (int)IzinDurumEnum.Duzenlendi));

            CreateMap<Izin, GetSuankiIzinByCalisanIdResponse>()
                .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => src.BaslangicTarihi.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => src.BitisTarihi.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.IseBaslamaTarihi, opt => opt.MapFrom(src => src.IseBaslamaTarihi.ToString("dd.MM.yyyy")));
        }
    }
}
