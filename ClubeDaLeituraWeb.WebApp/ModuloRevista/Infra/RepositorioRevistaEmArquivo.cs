using ClubeDaLeituraWeb.WebApp.Compartilhado.Infra.Arquivos;
using ClubeDaLeituraWeb.WebApp.ModuloAmigo.Dominio;

namespace ClubeDaLeituraWeb.WebApp.ModuloRevista.Infra;

public class RepositorioRevistaEmArquivo : RepositorioBaseEmArquivo<Revista>, IRepositorioRevista
{
    public RepositorioRevistaEmArquivo(ContextoJson contexto) : base(contexto) { }

    protected override List<Revista> CarregarRegistros()
    {
        return contexto.Revistas;
    }
}
