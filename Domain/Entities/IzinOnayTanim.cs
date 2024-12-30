namespace Domain.Entities;

public class IzinOnayTanim
{
    public int Id { get; set; }

    public string Aciklama { get; set; }

    public bool PersonelSubeYetkisi { get; set; }

    public bool MerkezMuduruYetkisi { get; set; }

    public bool Aktif { get; set; }

    public virtual List<IzinGrupIzinOnayTanim> IzinGrupIzinOnayTanimlari { get; set; } = new();

    public virtual List<IzinHareket> IzinHareketleri { get; set; } = new();

    public virtual List<IzinOnayTanimCalisan> OnayTanimCalisanlar { get; set; } = new();
}
