namespace Web.Models
{
    public class UpdateIzinOnayTanimVM
    {
        public int Id { get; set; }

        public string Aciklama { get; set; }

        public bool PersonelSubeYetkisi { get; set; }

        public bool MerkezMuduruYetkisi { get; set; }

        public bool Aktif { get; set; }

        public List<int> CalisanIdleri { get; set; }
    }
}
