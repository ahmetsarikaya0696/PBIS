using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Persistence.Contexts;

public class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public virtual DbSet<Birim> Birimler { get; set; }

    public virtual DbSet<Calisan> Calisanlar { get; set; }

    public virtual DbSet<Isyeri> Isyerleri { get; set; }

    public virtual DbSet<Izin> Izinler { get; set; }

    public virtual DbSet<IzinDurum> IzinDurumlari { get; set; }

    public virtual DbSet<IzinGrup> IzinGruplari { get; set; }

    public virtual DbSet<IzinGrupIzinOnayTanim> IzinGrupIzinOnayTanimlar { get; set; }

    public virtual DbSet<IzinHareket> IzinHareketler { get; set; }

    public virtual DbSet<IzinOnayTanim> IzinOnayTanimlar { get; set; }

    public virtual DbSet<IzinTur> IzinTurleri { get; set; }

    public virtual DbSet<IzinOnayTanimCalisan> OnayTanimCalisanlar { get; set; }

    public virtual DbSet<Unvan> Unvanlar { get; set; }

    public virtual DbSet<BaskentResimBlob> BaskentResimBlobs { get; set; }

    public virtual DbSet<Tatil> Tatiller { get; set; }

    public virtual DbSet<RetSebep> RetSebepleri { get; set; }

    public virtual DbSet<RetDetay> RetDetaylari { get; set; }

    public virtual DbSet<Yetkili> Yetkililer { get; set; }

    public virtual DbSet<Organizasyon> OrganizasyonSemasi { get; set; }
}
