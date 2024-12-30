using Application.Features.RetSebepleri.Commands.Create;
using Application.Features.RetSebepleri.Commands.Update;
using Application.Features.RetSebepleri.Queries.GetAllRetSebepExtended;
using Application.Features.RetSebepleri.Queries.GetRetDetayByIzinHareketId;
using Application.Features.RetSebepleri.Queries.GetRetDetayByIzinId;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.RetSebepleri.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<IzinHareket, GetRetDetayByIzinIdResponse>()
                .ForMember(dest => dest.RetSebep, opt => opt.MapFrom(src => src.RetDetay.RetSebep.Aciklama))
                .ForMember(dest => dest.RetDetay, opt => opt.MapFrom(src => src.RetDetay.Detay))
                .ForMember(dest => dest.RetTarih, opt => opt.MapFrom(src => src.IslemTarihi.ToString("dd.MM.yyyy HH:mm:ss")));

            CreateMap<RetSebep, GetAllRetSebepExtendedResponse>();

            CreateMap<CreateRetSebepCommand, RetSebep>()
                .ForMember(dest => dest.Aciklama, opt => opt.MapFrom(src => src.Aciklama.Trim()));

            CreateMap<RetSebep, CreatedRetSebepResponse>();

            CreateMap<UpdateRetSebepCommand, RetSebep>();
            CreateMap<UpdateRetSebepCommand, UpdatedRetSebepResponse>();

            CreateMap<RetDetay, GetRetDetayByIzinHareketIdResponse>()
                .ForMember(dest => dest.Aciklama, opt => opt.MapFrom(src => src.RetSebep.Aciklama))
                .ForMember(dest => dest.Detay, opt => opt.MapFrom(src => src.Detay));
        }
    }
}
