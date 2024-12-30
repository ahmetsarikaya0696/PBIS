namespace Web.Models
{
    public class CreateIzinOnayTanimVM
    {
        public string Aciklama { get; set; }

        public bool PersonelSubeYetkisi { get; set; } = false;

        public bool MerkezMuduruYetkisi { get; set; } = false;

        public bool Aktif { get; set; } = true;

        public List<int> CalisanIdleri { get; set; }
    }
}
