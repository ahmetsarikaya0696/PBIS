namespace Application.Features.Izinler.Queries.GetIzinFormVerileriById
{
    public class GetIzinFormVerileriByIdResponse
    {
        public int Id { get; set; }

        public int IzinTurId { get; set; }

        public string BaslangicTarihi { get; set; }

        public string BitisTarihi { get; set; }

        public string IseBaslamaTarihi { get; set; }

        public string MahsubenBaslangicTarihi { get; set; }

        public string YerineBakacakKisi { get; set; }

        public string Aciklama { get; set; }

        public bool YillikIzinUcretiIstegi { get; set; }

        public string Telefon { get; set; }

        public string Adres { get; set; }
    }
}
