using Application.Features.Tatiller.Commands.Create;
using Application.Features.Tatiller.Commands.Delete;
using Application.Features.Tatiller.Commands.Update;
using Application.Features.Tatiller.Queries.GetTatiller;
using Application.Features.Tatiller.Queries.GetTatillerByTarih;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.Tatiller.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreateTatilCommand, Tatil>()
                .ForMember(dest => dest.Aciklama, opt => opt.MapFrom(src => src.Aciklama.Trim()));

            CreateMap<Tatil, CreatedTatilResponse>()
                .ForMember(dest => dest.Tarih, opt => opt.MapFrom(src => src.Tarih.ToString("dd.MM.yyyy")));

            CreateMap<Tatil, GetTatillerByTarihResponse>()
                .ForMember(dest => dest.Tarih, opt => opt.MapFrom(src => src.Tarih.ToString("dd.MM.yyyy"))); ;

            CreateMap<Tatil, GetTatillerResponse>()
                .ForMember(dest => dest.Tarih, opt => opt.MapFrom(src => src.Tarih.ToString("dd.MM.yyyy")));

            CreateMap<UpdateTatilCommand, Tatil>();
            CreateMap<Tatil, UpdatedTatilResponse>()
                .ForMember(dest => dest.Tarih, opt => opt.MapFrom(src => src.Tarih.ToString("dd.MM.yyyy")));

            CreateMap<Tatil, DeletedTatilByIdResponse>()
               .ForMember(dest => dest.Tarih, opt => opt.MapFrom(src => src.Tarih.ToString("dd.MM.yyyy")));
        }
    }
}
