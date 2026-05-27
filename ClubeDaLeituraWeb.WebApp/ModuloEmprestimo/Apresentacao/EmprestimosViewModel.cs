using System.ComponentModel.DataAnnotations;

namespace ClubeDaLeituraWeb.WebApp.ModuloEmprestimo.Apresentacao;

public record ListarEmprestimosViewModel(
    string Id,
    string NomeAmigo,
    string TituloRevista,
    DateTime Abertura,
    DateTime ConclusaoPrevista,
    string Status,
    bool EstaAtrasado
);

public record CadastrarEmprestimoViewModel(
    [Required(ErrorMessage = "O campo \"Amigo\" deve ser preenchido.")]
    string AmigoId,

    [Required(ErrorMessage = "O campo \"Revista\" deve ser preenchido.")]
    string RevistaId
);

public record DevolverEmprestimoViewModel(
    string Id,
    string NomeAmigo,
    string TituloRevista,
    DateTime Abertura,
    DateTime ConclusaoPrevista
);
