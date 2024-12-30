using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Notifications.OrganizasyonHareketleri
{
    public class OrganizasyonHareketNotificationHandler : INotificationHandler<OrganizasyonHareketNotification>
    {
        private readonly IOrganizasyonHareketleriRepository _organizasyonHareketleriRepository;

        public OrganizasyonHareketNotificationHandler(IOrganizasyonHareketleriRepository organizasyonHareketleriRepository)
        {
            _organizasyonHareketleriRepository = organizasyonHareketleriRepository;
        }

        public async Task Handle(OrganizasyonHareketNotification notification, CancellationToken cancellationToken)
        {
            OrganizasyonHareket organizasyonHareket = new()
            {
                CalisanId = notification.CalisanId,
                Ip = notification.Ip,
                OrganizasyonId = notification.OrganizasyonId,
                Islem = (int)notification.IslemEnum,
                IslemTarihi = DateTime.Now
            };

            await _organizasyonHareketleriRepository.AddAsync(organizasyonHareket);
        }
    }
}
