namespace Application.Features.IzinTurleri.Queries.GetAllIzinTur
{
    public class GetAllIzinTurResponse
    {
        public int Id { get; set; }

        public string Aciklama { get; set; }

        public string IzinFormTipi { get; set; }

        public string SabitGunSayisi { get; set; }

        public string TatilGunleriSayilir { get; set; }

        public string Aktif { get; set; }
    }
}
