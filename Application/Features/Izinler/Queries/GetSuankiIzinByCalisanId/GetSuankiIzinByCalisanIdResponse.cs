namespace Application.Features.Izinler.Queries.GetSuankiIzinByCalisanId
{
    public class GetSuankiIzinByCalisanIdResponse
    {
        public string BaslangicTarihi { get; set; }
        public string BitisTarihi { get; set; }
        public string IseBaslamaTarihi { get; set; }
        public int GunSayisi { get; set; }
    }
}
