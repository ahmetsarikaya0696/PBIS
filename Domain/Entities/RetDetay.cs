namespace Domain.Entities
{
    public class RetDetay
    {
        public int Id { get; set; }
        public string Detay { get; set; }
        public int IzinHareketId { get; set; }
        public int RetSebepId { get; set; }

        public virtual IzinHareket IzinHareket { get; set; }
        public virtual RetSebep RetSebep { get; set; }
    }
}
