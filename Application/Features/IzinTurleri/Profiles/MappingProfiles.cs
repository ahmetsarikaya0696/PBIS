using Application.Features.IzinTurleri.Commands.Create;
using Application.Features.IzinTurleri.Commands.Update;
using Application.Features.IzinTurleri.Queries.GetAllIzinTur;
using Application.Features.IzinTurleri.Queries.GetAllIzinTurSelectList;
using Application.Features.IzinTurleri.Queries.GetIzinTurById;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.IzinTurleri.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<IzinTur, GetAllIzinTurSelectListResponse>();
            CreateMap<IzinTur, GetIzinTurByIdResponse>();

            CreateMap<IzinTur, GetAllIzinTurResponse>()
                .ForMember(dest => dest.IzinFormTipi, opt => opt.MapFrom(src => src.IzinFormTipi == 'T' ? "Text" : "Form"))
                .ForMember(dest => dest.Aktif, opt => opt.MapFrom(src => src.Aktif ? "Evet" : "Hayır"))
                .ForMember(dest => dest.SabitGunSayisi, opt => opt.MapFrom(src => src.SabitGunSayisi == null ? "-" : src.SabitGunSayisi.ToString()))
                .ForMember(dest => dest.TatilGunleriSayilir, opt => opt.MapFrom(src => src.TatilGunleriSayilir ? "Evet" : "Hayır"));

            CreateMap<CreateIzinTurCommand, IzinTur>();
            CreateMap<IzinTur, CreatedIzinTurResponse>();

            CreateMap<UpdateIzinTurCommand, IzinTur>();
            CreateMap<IzinTur, UpdatedIzinTurResponse>()
                .ForMember(dest => dest.Aktif, opt => opt.MapFrom(src => src.Aktif ? "aktif" : "pasif"));
        }
    }
}
