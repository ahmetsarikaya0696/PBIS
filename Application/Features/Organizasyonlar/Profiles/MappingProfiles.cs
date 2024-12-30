using Application.Features.Organizasyonlar.Commands.DeleteOrganizasyon;
using Application.Features.Organizasyonlar.Commands.UpdateOrganizasyon;
using Application.Features.Organizasyonlar.Queries.GetOrganizasyonById;
using Application.Features.OrganizasyonSemasi.Commands.CreateOrganizasyon;
using Application.Features.OrganizasyonSemasi.Queries.GetOrganizasyonlar;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.OrganizasyonSemasi.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Organizasyon, GetOrganizasyonlarResponse>();

            CreateMap<CreateOrganizasyonCommand, Organizasyon>();
            CreateMap<Organizasyon, CreateOrganizasyonResponse>();

            CreateMap<Organizasyon, DeleteOrganizasyonByIdResponse>();

            CreateMap<Organizasyon, GetOrganizasyonByIdResponse>();
            CreateMap<UpdateOrganizasyonCommand, Organizasyon>();
        }
    }
}
