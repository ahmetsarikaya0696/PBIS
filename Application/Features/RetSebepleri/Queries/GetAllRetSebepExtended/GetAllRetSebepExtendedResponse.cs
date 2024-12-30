namespace Application.Features.RetSebepleri.Queries.GetAllRetSebepExtended
{
    public class GetAllRetSebepExtendedResponse
    {
        public int Id { get; set; }
        public string Aciklama { get; set; }
        public bool Duzenlenebilir { get; set; }
        public bool Aktif { get; set; }
    }
}
