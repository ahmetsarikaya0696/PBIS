namespace Application.Features.IzinHareketleri.Queries.GetIzinHareketleriByIzinId
{
    public class GetIzinHareketleriByIzinIdResponse
    {
        public int Id { get; set; }
        public string IslemYapanCalisanUnvan { get; set; }
        public string IslemYapanCalisanAdSoyad { get; set; }
        public string IsleminIzinOnayTanimi { get; set; }
        public string IslemTarihi { get; set; }
        public string IzinDurum { get; set; }
    }
}
