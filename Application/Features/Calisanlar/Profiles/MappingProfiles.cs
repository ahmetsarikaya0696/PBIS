using Application.Features.Calisanlar.Queries.GetByKullaniciAdi;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.Calisanlar.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Calisan, GetByKullaniciAdiCalisanResponse>()
                .ForMember(dest => dest.Unvan, opt => opt.MapFrom(src => src.Unvan.Aciklama))
                .ForMember(dest => dest.Birim, opt => opt.MapFrom(src => src.Birim.Aciklama))
                .ForMember(dest => dest.Kod, opt => opt.MapFrom(src => src.Birim.OrganizasyonSemasi != null ? src.Birim.OrganizasyonSemasi.Kod : "10"));
        }
    }
}
