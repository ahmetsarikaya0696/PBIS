namespace Application.Features.Izinler.Queries.GetByCalisanId
{
    public class GetByCalisanIdIzinResponse
    {
        public int Id { get; set; }

        public string IstekTarihi { get; set; }

        public string BaslangicTarihi { get; set; }

        public string BitisTarihi { get; set; }

        public string IseBaslamaTarihi { get; set; }

        public string IzinDurumu { get; set; }

        public string IzinTur { get; set; }

        public bool IsForm { get; set; }

        public int? SabitGunSayisi { get; set; }

        public string Adim { get; set; }

        public string DogrulamaYontemi { get; set; }
    }
}
