namespace Application.Features.IzinOnayTanimlari.Queries.GetAllIzinOnayTanim
{
    public class GetAllIzinOnayTanimResponse
    {
        public int Id { get; set; }

        public string Aciklama { get; set; }

        public string Aktif { get; set; }

        public string MerkezMuduruYetkisi { get; set; }

        public string PersonelSubeYetkisi { get; set; }
    }
}
