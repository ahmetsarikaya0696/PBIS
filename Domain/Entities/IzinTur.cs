namespace Domain.Entities;

public class IzinTur
{
    public int Id { get; set; }

    public string Aciklama { get; set; }

    public char IzinFormTipi { get; set; }

    public int? SabitGunSayisi { get; set; }

    public bool TatilGunleriSayilir { get; set; }

    public bool SenelikIzinMi { get; set; }

    public bool Aktif { get; set; }

    public virtual List<Izin> Izinler { get; set; } = new();
}
