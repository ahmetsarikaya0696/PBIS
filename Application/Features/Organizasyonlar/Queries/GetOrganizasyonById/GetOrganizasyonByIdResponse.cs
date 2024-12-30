namespace Application.Features.Organizasyonlar.Queries.GetOrganizasyonById
{
    public class GetOrganizasyonByIdResponse
    {
        public int Id { get; set; }
        public string Kod { get; set; }
        public int? UstBirimId { get; set; }
        public string UstBirim { get; set; }
        public string Aciklama_TR { get; set; }
        public string Aciklama_EN { get; set; }
        public int? OrganizasyonKodu { get; set; }
        public bool AnaBirim { get; set; }
        public bool Aktif { get; set; }
        public int? BirimId { get; set; }
    }
}
