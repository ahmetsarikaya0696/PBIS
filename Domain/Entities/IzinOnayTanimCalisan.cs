namespace Domain.Entities;

public class IzinOnayTanimCalisan
{
    public int IzinOnayTanimId { get; set; }

    public int CalisanId { get; set; }

    public virtual Calisan Calisan { get; set; }

    public virtual IzinOnayTanim IzinOnayTanim { get; set; }
}
