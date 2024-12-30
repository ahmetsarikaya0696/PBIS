using Domain.Enums;
using MediatR;

namespace Application.Notifications.OrganizasyonHareketleri
{
    public class OrganizasyonHareketNotification : INotification
    {
        public int CalisanId { get; set; }
        public string Ip { get; set; }
        public int OrganizasyonId { get; set; }
        public IslemEnum IslemEnum { get; set; }
    }
}
