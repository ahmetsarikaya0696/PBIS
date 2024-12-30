namespace Domain.Entities;

public class IzinGrupIzinOnayTanim
{
    public int IzinOnayTanimId { get; set; }

    public int IzinGrupId { get; set; }

    public int OnayTanimSirasi { get; set; }

    public virtual IzinGrup IzinGrup { get; set; }

    public virtual IzinOnayTanim IzinOnayTanim { get; set; }
}
