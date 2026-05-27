using ClubeDaLeituraWeb.WebApp.Compartilhado.Infra.Arquivos;
using ClubeDaLeituraWeb.WebApp.ModuloReserva.Dominio;

namespace ClubeDaLeituraWeb.WebApp.ModuloReserva.Infra;

public class RepositorioReservaEmArquivo : RepositorioBaseEmArquivo<Reserva>, IRepositorioReserva
{
    public RepositorioReservaEmArquivo(ContextoJson contexto) : base(contexto) { }

    protected override List<Reserva> CarregarRegistros()
    {
        return contexto.Reservas;
    }
}
