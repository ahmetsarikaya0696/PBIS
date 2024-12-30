using Application.Features.OrganizasyonHareketleri.Queries.GetOrganizasyonHareketleriByOrganizasyonId;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.OrganizasyonHareketleri.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrganizasyonHareket, GetOrganizasyonHareketleriByOrganizasyonIdResponse>()
                            .ForMember(dest => dest.Ad, opt => opt.MapFrom(src => src.Calisan.Ad))
                            .ForMember(dest => dest.Soyad, opt => opt.MapFrom(src => src.Calisan.Soyad))
                            .ForMember(dest => dest.Unvan, opt => opt.MapFrom(src => src.Calisan.Unvan.Aciklama))
                            .ForMember(dest => dest.IslemTarihi, opt => opt.MapFrom(src => src.IslemTarihi.ToString("dd.MM.yyyy HH:mm:ss")));
        }
    }
}
