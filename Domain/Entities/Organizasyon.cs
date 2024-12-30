namespace Domain.Entities
{
    public class Organizasyon
    {
        public int Id { get; set; }
        public string Kod { get; set; }
        public string Aciklama_TR { get; set; }
        public string Aciklama_EN { get; set; }
        public int? OrganizasyonKodu { get; set; }
        public bool AnaBirim { get; set; }
        public bool Aktif { get; set; }
        public int UstBirimId { get; set; }

        public int? BirimId { get; set; }
        public virtual Birim Birim { get; set; }

        public List<OrganizasyonHareket> OrganizasyonHareketleri { get; set; }
    }
}
