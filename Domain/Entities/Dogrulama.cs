namespace Domain.Entities
{
    public class Dogrulama
    {
        private string GenerateRandomCode()
        {
            Random random = new Random();
            int codeLength = 6;
            string code = string.Empty;

            for (int i = 0; i < codeLength; i++)
            {
                int randomNumber = random.Next(10);
                code += randomNumber.ToString();
            }

            return code;
        }

        public Dogrulama()
        {
            Kod = GenerateRandomCode();
            SonKullanimTarihi = DateTime.Now.AddMinutes(3.45);
        }

        public int Id { get; set; }
        public string Kod { get; private set; }
        public DateTime SonKullanimTarihi { get; private set; }
        public string Yontem { get; set; }
        public bool Gecerli { get; set; } = true;

        // Navigation Properties
        public int CalisanId { get; set; }
        public virtual Calisan Calisan { get; set; }

        public int? IzinId { get; set; }
        public virtual Izin Izin { get; set; }
    }
}
