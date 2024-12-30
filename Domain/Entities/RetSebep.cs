namespace Domain.Entities
{
    public class RetSebep
    {
        public int Id { get; set; }
        public string Aciklama { get; set; }
        public bool Duzenlenebilir { get; set; }
        public bool PersonelSubeyeOzgu { get; set; }
        public bool Aktif { get; set; }

        public virtual List<RetDetay> RetDetaylari { get; set; }
    }
}
