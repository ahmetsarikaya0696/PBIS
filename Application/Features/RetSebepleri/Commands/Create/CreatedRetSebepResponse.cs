namespace Application.Features.RetSebepleri.Commands.Create
{
    public class CreatedRetSebepResponse
    {
        public int Id { get; set; }
        public string Aciklama { get; set; }
        public bool Duzenlenebilir { get; set; }
        public bool Aktif { get; set; }
    }
}
