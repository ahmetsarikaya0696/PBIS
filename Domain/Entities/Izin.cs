namespace Domain.Entities;

public class Izin
{
    public int Id { get; set; }

    public int CalisanId { get; set; }

    public string Ip { get; set; }

    public DateTime IstekTarihi { get; set; }

    public DateTime BaslangicTarihi { get; set; }

    public DateTime BitisTarihi { get; set; }

    public DateTime? MahsubenBaslangicTarihi { get; set; }

    public DateTime IseBaslamaTarihi { get; set; }

    public int IzinDurumId { get; set; }

    public int IsyeriId { get; set; }

    public int BirimId { get; set; }

    public int UnvanId { get; set; }

    public string YerineBakacakKisi { get; set; }

    public string Adres { get; set; }

    public string Telefon { get; set; }

    public string Aciklama { get; set; }

    public bool YillikIzinUcretiIstegi { get; set; } = false;

    public int IzinTurId { get; set; }

    public virtual Calisan Calisan { get; set; }

    public virtual IzinDurum IzinDurum { get; set; }

    public virtual IzinTur IzinTur { get; set; }

    public virtual Isyeri Isyeri { get; set; }

    public virtual Birim Birim { get; set; }

    public virtual Unvan Unvan { get; set; }

    public virtual List<IzinHareket> IzinHareketleri { get; set; } = new();

    public virtual List<Dogrulama> Dogrulamalar { get; set; }
}
