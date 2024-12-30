using Application.Features.IzinOnayTanimlari.Commands.Create;
using Application.Features.IzinOnayTanimlari.Commands.Update;
using Application.Features.IzinOnayTanimlari.Queries.GetAllIzinOnayTanim;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.IzinOnayTanimlari.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<IzinOnayTanim, GetAllIzinOnayTanimResponse>()
                 .ForMember(dest => dest.Aktif, opt => opt.MapFrom(src => src.Aktif ? "Evet" : "Hayır"))
                 .ForMember(dest => dest.PersonelSubeYetkisi, opt => opt.MapFrom(src => src.PersonelSubeYetkisi ? "Evet" : "Hayır"))
                 .ForMember(dest => dest.MerkezMuduruYetkisi, opt => opt.MapFrom(src => src.MerkezMuduruYetkisi ? "Evet" : "Hayır"));

            CreateMap<CreateIzinOnayTanimWithCalisanlarCommand, IzinOnayTanim>();
            CreateMap<IzinOnayTanim, CreatedIzinOnayTanimWithCalisanlarResponse>();

            CreateMap<UpdateIzinOnayTanimWithCalisanlarCommand, IzinOnayTanim>();
            CreateMap<IzinOnayTanim, UpdatedIzinOnayTanimWithCalisanlarResponse>();
        }
    }
}
