using ClubeDaLeituraWeb.WebApp.Compartilhado.Dominio;
using ClubeDaLeituraWeb.WebApp.ModuloAmigo.Dominio;
using ClubeDaLeituraWeb.WebApp.ModuloRevista.Dominio;

namespace ClubeDaLeituraWeb.WebApp.ModuloReserva.Dominio;

public class Reserva : EntidadeBase<Reserva>
{
    public Amigo Amigo { get; set; } = null!;
    public Revista Revista { get; set; } = null!;
    public DateTime DataReserva { get; set; }
    public StatusReserva Status { get; set; }

    public Reserva() { }

    public Reserva(Amigo amigo, Revista revista)
    {
        Amigo = amigo;
        Revista = revista;
        DataReserva = DateTime.Now.Date;
        Status = StatusReserva.Ativa;
    }

    public void Cancelar()
    {
        Status = StatusReserva.Cancelada;
        Revista.CancelarReserva();
    }

    public void Concluir()
    {
        Status = StatusReserva.Concluida;
    }

    public override List<string> Validar()
    {
        List<string> erros = new List<string>();

        if (Amigo == null)
            erros.Add("O campo \"Amigo\" deve ser preenchido.");

        if (Revista == null)
            erros.Add("O campo \"Revista\" deve ser preenchido.");

        if (Revista != null && Status == StatusReserva.Ativa && Revista.Status != StatusRevista.Disponivel)
            erros.Add("Só é possível reservar revistas disponíveis.");

        return erros;
    }

    public override void AtualizarDados(Reserva entidadeAtualizada)
    {
        Amigo = entidadeAtualizada.Amigo;
        Revista = entidadeAtualizada.Revista;
        DataReserva = entidadeAtualizada.DataReserva;
        Status = entidadeAtualizada.Status;
    }
}
