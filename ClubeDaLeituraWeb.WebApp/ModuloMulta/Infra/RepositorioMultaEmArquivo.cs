using ClubeDaLeituraWeb.WebApp.Compartilhado.Infra.Arquivos;
using ClubeDaLeituraWeb.WebApp.ModuloMulta.Dominio;

namespace ClubeDaLeituraWeb.WebApp.ModuloMulta.Infra;

public class RepositorioMultaEmArquivo : RepositorioBaseEmArquivo<Multa>, IRepositorioMulta
{
    public RepositorioMultaEmArquivo(ContextoJson contexto) : base(contexto) { }

    protected override List<Multa> CarregarRegistros()
    {
        return contexto.Multas;
    }
}
