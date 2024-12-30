namespace Domain.Entities
{
    public class Yetkili
    {
        public int Id { get; set; }
        public int CalisanId { get; set; }
        public virtual Calisan Calisan { get; set; }
    }
}
