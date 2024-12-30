using Application.Features.IzinHareketleri.Queries.GetIzinHareketleriByIzinId;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.IzinHareketleri.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<IzinHareket, GetIzinHareketleriByIzinIdResponse>()
                .ForMember(dest => dest.IslemYapanCalisanUnvan, opt => opt.MapFrom(src => src.Calisan.Unvan.Aciklama))
                .ForMember(dest => dest.IslemYapanCalisanAdSoyad, opt => opt.MapFrom(src => $"{src.Calisan.Ad} {src.Calisan.Soyad}"))
                .ForMember(dest => dest.IslemTarihi, opt => opt.MapFrom(src => src.IslemTarihi.ToString("dd.MM.yyyy HH.mm.ss")))
                .ForMember(dest => dest.IzinDurum, opt => opt.MapFrom(src => src.IzinDurum.Aciklama))
                .ForMember(dest => dest.IsleminIzinOnayTanimi, opt => opt.MapFrom(src => src.IzinOnayTanimId != null ? src.IzinOnayTanim.Aciklama : string.Empty));
        }
    }
}
