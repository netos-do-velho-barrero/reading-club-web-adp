namespace ClubeDaLeituraWeb.WebApp.ModuloMulta.Apresentacao;

public record ListarMultasViewModel(
    string Id,
    string NomeAmigo,
    string TituloRevista,
    DateTime DataEmprestimo,
    DateTime DataDevolucaoPrevista,
    int DiasAtraso,
    decimal Valor,
    string Status
);

public record QuitarMultaViewModel(
    string Id,
    string NomeAmigo,
    string TituloRevista,
    int DiasAtraso,
    decimal Valor
);

public record FiltrarMultasPorAmigoViewModel(
    string AmigoId
);
