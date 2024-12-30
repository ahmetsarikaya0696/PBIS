using Application.Features.IzinGruplari.Commands.Create;
using Application.Features.IzinGruplari.Commands.Update;
using Application.Features.Izinler.Commands.Create;
using Application.Features.Izinler.Commands.Update;
using Application.Features.Izinler.Queries.GetIzinDetayById;
using Application.Features.Izinler.Queries.GetSuankiIzinByCalisanId;
using Application.Features.IzinOnayTanimlari.Commands.Create;
using Application.Features.IzinOnayTanimlari.Commands.Update;
using Application.Features.IzinTurleri.Commands.Create;
using Application.Features.IzinTurleri.Commands.Update;
using Application.Features.Organizasyonlar.Commands.UpdateOrganizasyon;
using Application.Features.OrganizasyonSemasi.Commands.CreateOrganizasyon;
using Application.Features.RetSebepleri.Commands.Create;
using Application.Features.RetSebepleri.Commands.Update;
using Application.Features.Tatiller.Commands.Create;
using Application.Features.Tatiller.Commands.Update;
using Application.Features.Tatiller.Queries.GetTatillerByTarih;
using AutoMapper;
using System.Globalization;
using Web.Models;

namespace Web.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<GetSuankiIzinByCalisanIdResponse, HomeVM>()
                .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => src.BaslangicTarihi))
                .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => src.BitisTarihi))
                .ForMember(dest => dest.IseBaslamaTarihi, opt => opt.MapFrom(src => src.IseBaslamaTarihi));

            CreateMap<FormTipIzinCreateVM, CreateIzinCommand>()
                .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.BaslangicTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.BitisTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.IseBaslamaTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.IseBaslamaTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)));

            CreateMap<TextTipIzinCreateVM, CreateIzinCommand>()
                .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.BaslangicTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.BitisTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.IseBaslamaTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.IseBaslamaTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)));

            CreateMap<FormTipIzinUpdateVM, UpdateIzinCommand>()
                .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.BaslangicTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.BitisTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.IseBaslamaTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.IseBaslamaTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)));

            CreateMap<TextTipIzinUpdateVM, UpdateIzinCommand>()
                .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.BaslangicTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.BitisTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.IseBaslamaTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.IseBaslamaTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)));

            CreateMap<CreateTatilVM, CreateTatilCommand>()
                .ForMember(dest => dest.Tarih, opt => opt.MapFrom(src => DateTime.ParseExact(src.Tarih, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
;
            CreateMap<GetTatillerByTarihResponse, CreateTatilVM>();

            CreateMap<CreatedTatilResponse, CreateTatilVM>();
            ;

            CreateMap<UpdateTatilVM, UpdateTatilCommand>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UpdateId))
                .ForMember(dest => dest.Aciklama, opt => opt.MapFrom(src => src.UpdateAciklama))
                .ForMember(dest => dest.Tarih, opt => opt.MapFrom(src => DateTime.ParseExact(src.UpdateTarih, "dd.MM.yyyy", CultureInfo.InvariantCulture)));

            CreateMap<GetIzinDetayByIdResponse, IzinDetayVM>();

            CreateMap<CreateRetSebepVM, CreateRetSebepCommand>();
            CreateMap<CreatedRetSebepResponse, CreateRetSebepVM>();

            CreateMap<UpdateRetSebepVM, UpdateRetSebepCommand>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UpdateId))
                .ForMember(dest => dest.Aciklama, opt => opt.MapFrom(src => src.UpdateAciklama))
                .ForMember(dest => dest.Duzenlenebilir, opt => opt.MapFrom(src => src.UpdateDuzenlenebilir))
                .ForMember(dest => dest.Aktif, opt => opt.MapFrom(src => src.UpdateAktif));

            CreateMap<IzinOnayTanimIdWithSira, IzinOnayTanimIdWithSira>();

            CreateMap<CreateIzinGrupVM, CreateIzinGrupWithIzinOnayTanimlariCommand>()
                .ForMember(dest => dest.IzinOnayTanimIdVeSiralari, opt => opt.MapFrom(src => src.IzinOnayTanimVeSiralari))
                .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.BaslangicTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.BitisTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)));

            CreateMap<CreateIzinOnayTanimVM, CreateIzinOnayTanimWithCalisanlarCommand>();

            CreateMap<CreateIzinTurVM, CreateIzinTurCommand>();

            CreateMap<UpdateIzinGrupVM, UpdateIzinGrupCommand>()
                .ForMember(dest => dest.BaslangicTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.BaslangicTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.BitisTarihi, opt => opt.MapFrom(src => DateTime.ParseExact(src.BitisTarihi, "dd.MM.yyyy", CultureInfo.InvariantCulture)));

            CreateMap<UpdateIzinOnayTanimVM, UpdateIzinOnayTanimWithCalisanlarCommand>();

            CreateMap<UpdateIzinTurVM, UpdateIzinTurCommand>();

            CreateMap<CreateOrganizasyonVM, CreateOrganizasyonCommand>();

            CreateMap<UpdateOrganizasyonVM, UpdateOrganizasyonCommand>();

        }
    }
}
