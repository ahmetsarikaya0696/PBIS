namespace Domain.Entities;

public class Calisan
{
    public int Id { get; set; }

    public string Ad { get; set; }

    public string Soyad { get; set; }

    public string Tc { get; set; }

    public int SicilNo { get; set; }

    public int IsyeriId { get; set; }

    public int BirimId { get; set; }

    public int UnvanId { get; set; }

    public DateTime DogumTarihi { get; set; }

    public DateTime IseBaslamaTarihi { get; set; }

    public DateTime? IstenCikisTarihi { get; set; }

    public string KullaniciAdi { get; set; }

    public bool Aktif { get; set; }

    public int? LREF { get; set; }

    public string Telefon { get; set; }

    public virtual Birim Birim { get; set; }

    public virtual List<Izin> Izinler { get; set; } = new();

    public virtual List<IzinOnayTanimCalisan> IzinOnayTanimCalisanlar { get; set; } = new();

    public virtual Isyeri Isyeri { get; set; }

    public virtual Unvan Unvan { get; set; }

    public virtual List<IzinHareket> IzinHareketleri { get; set; } = new();

    public List<OrganizasyonHareket> OrganizasyonHareketleri { get; set; }
}
