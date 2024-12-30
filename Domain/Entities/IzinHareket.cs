namespace Domain.Entities;

public class IzinHareket
{
    public int Id { get; set; }

    public string Ip { get; set; }

    public int CalisanId { get; set; }

    public int IzinId { get; set; }

    public int? Sira { get; set; }

    public DateTime IslemTarihi { get; set; }

    public int? IzinOnayTanimId { get; set; }

    public int IzinDurumId { get; set; }

    public virtual IzinDurum IzinDurum { get; set; }

    public virtual Izin Izin { get; set; }

    public virtual IzinOnayTanim IzinOnayTanim { get; set; }

    public virtual Calisan Calisan { get; set; }

    public virtual RetDetay RetDetay { get; set; }
}
