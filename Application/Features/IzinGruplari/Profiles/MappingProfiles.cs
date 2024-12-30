using Application.Features.IzinGruplari.Commands.Create;
using Application.Features.IzinGruplari.Commands.Update;
using Application.Features.IzinGruplari.Queries.GetAllIzinGrup;
using Application.Features.IzinGruplari.Queries.GetAllIzinGrupByCalisanId;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.IzinGruplari.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<IzinGrup, GetAllIzinGrupResponse>()
                    .ForMember(dest => dest.UnvanAdSoyad, opt => opt.MapFrom(src => src.Calisan != null ? $"{src.Calisan.Unvan.Aciklama} {src.Calisan.Ad} {src.Calisan.Soyad}" : "-"))
                    .ForMember(dest => dest.Unvan, opt => opt.MapFrom(src => src.Unvan != null ? src.Unvan.Aciklama : "-"))
                    .ForMember(dest => dest.Birim, opt => opt.MapFrom(src => src.Birim != null ? src.Birim.Aciklama : "-"))
                    .ForMember(dest => dest.Isyeri, opt => opt.MapFrom(src => src.Isyeri.Aciklama))
                    .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => src.BaslangicTarihi.ToString("dd.MM.yyyy")))
                    .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => src.BitisTarihi.ToString("dd.MM.yyyy")));

            CreateMap<CreateIzinGrupWithIzinOnayTanimlariCommand, IzinGrup>();

            CreateMap<UpdateIzinGrupCommand, IzinGrup>();
            CreateMap<IzinGrup, UpdatedIzinGrupResponse>()
                .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => src.BaslangicTarihi.ToString("dd.MM.yyyy")))
                .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => src.BitisTarihi.ToString("dd.MM.yyyy")));

            CreateMap<IzinGrup, GetSelectListIzinGrupByCalisanIdResponse>();
        }
    }
}
