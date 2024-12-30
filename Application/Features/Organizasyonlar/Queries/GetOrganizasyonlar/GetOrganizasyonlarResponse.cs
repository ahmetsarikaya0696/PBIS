﻿namespace Application.Features.OrganizasyonSemasi.Queries.GetOrganizasyonlar
{
    public class GetOrganizasyonlarResponse
    {
        public int Id { get; set; }
        public string Kod { get; set; }
        public string Aciklama_TR { get; set; }
        public string Aciklama_EN { get; set; }
        public int? OrganizasyonKodu { get; set; }
        public bool AnaBirim { get; set; }
        public int? UstBirimId { get; set; }
        public string UstBirimAciklama { get; set; }
        public bool Aktif { get; set; }
    }
}