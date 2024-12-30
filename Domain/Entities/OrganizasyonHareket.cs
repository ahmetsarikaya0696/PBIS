namespace Domain.Entities
{
    public class OrganizasyonHareket
    {
        public int Id { get; set; }
        public string Ip { get; set; }
        public int CalisanId { get; set; }
        public int OrganizasyonId { get; set; }
        public DateTime IslemTarihi { get; set; }
        public int Islem { get; set; }

        public Calisan Calisan { get; set; }
        public Organizasyon Organizasyon { get; set; }
    }
}
