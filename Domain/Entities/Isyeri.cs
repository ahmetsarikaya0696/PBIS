namespace Domain.Entities;

public class Isyeri
{
    public int Id { get; set; }

    public string Aciklama { get; set; }

    public bool Aktif { get; set; }

    public virtual List<Calisan> Calisanlar { get; set; } = new();

    public virtual List<Izin> Izinler { get; set; } = new();
}
