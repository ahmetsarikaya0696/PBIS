namespace Domain.Entities;

public class IzinDurum
{
    public int Id { get; set; }

    public string Aciklama { get; set; }

    public virtual List<Izin> Izinler { get; set; } = new();

    public virtual List<IzinHareket> IzinHareketleri { get; set; } = new();
}
