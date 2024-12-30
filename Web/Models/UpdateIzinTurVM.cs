namespace Web.Models
{
    public class UpdateIzinTurVM
    {
        public int Id { get; set; }

        public string Aciklama { get; set; }

        public char IzinFormTipi { get; set; }

        public int? SabitGunSayisi { get; set; }

        public bool TatilGunleriSayilir { get; set; }

        public bool Aktif { get; set; }
    }
}
