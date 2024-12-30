namespace Domain.Entities;

public class IzinGrup
{
    public int Id { get; set; }

    public string Aciklama { get; set; }

    public int? CalisanId { get; set; }

    public int? UnvanId { get; set; }

    public int? BirimId { get; set; }

    public int? IsyeriId { get; set; }

    public DateTime BaslangicTarihi { get; set; }

    public DateTime BitisTarihi { get; set; }

    public virtual List<IzinGrupIzinOnayTanim> IzinGrupIzinOnayTanimlari { get; set; } = new();

    public virtual Calisan Calisan { get; set; }

    public virtual Unvan Unvan { get; set; }

    public virtual Birim Birim { get; set; }

    public virtual Isyeri Isyeri { get; set; }
}
