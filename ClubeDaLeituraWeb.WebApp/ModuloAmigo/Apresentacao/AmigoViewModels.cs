using System.ComponentModel.DataAnnotations;

namespace ClubeDaLeituraWeb.WebApp.ModuloAmigo.Apresentacao;

public record ListarAmigosViewModel(
    string Id,
    string Nome,
    string NomeResponsavel,
    string Telefone
);

public record CadastrarAmigosViewModel(
    [Required(ErrorMessage = "O campo \"Nome\" deve ser preenchido.")]
    [StringLength(100, ErrorMessage = "O campo \"Nome\" deve conter entre 3 e 100 caracteres.")]
    string Nome,


    [Required(ErrorMessage = "O campo \"Nome do Responsavel\" deve ser preenchido.")]
    [StringLength(100, ErrorMessage = "O campo \"Nome do Responsavel\" deve conter entre 3 e 100 caracteres.")]
    string NomeResponsavel,

    [Required(ErrorMessage = "O campo \"Telefone\" deve ser preenchido.")]
    [RegularExpression(@"^\(\d{2}\) \d{4,5}-\d{4}$",
   ErrorMessage = "O campo \"Telefone\" deve seguir o formato: (XX) XXXX-XXXX ou (XX) XXXXX-XXXX.")]
   string Telefone

);

public record EditarAmigosViewModel(
    string Id,

    [Required(ErrorMessage = "O campo \"Nome\" deve ser preenchido.")]
    [StringLength(100, ErrorMessage = "O campo \"Nome\" deve conter entre 3 e 100 caracteres.")]
    string Nome,


    [Required(ErrorMessage = "O campo \"Nome do Responsavel\" deve ser preenchido.")]
    [StringLength(100, ErrorMessage = "O campo \"Nome do Responsavel\" deve conter entre 3 e 100 caracteres.")]
    string NomeResponsavel,

    [Required(ErrorMessage = "O campo \"Telefone\" deve ser preenchido.")]
    [RegularExpression(@"^\(\d{2}\) \d{4,5}-\d{4}$",
    ErrorMessage = "O campo \"Telefone\" deve seguir o formato: (XX) XXXX-XXXX ou (XX) XXXXX-XXXX.")]
    string Telefone
);

public record ExcluirAmigosViewModel(
    string Id,
    string Nome,
    string NomeResponsavel,
    string Telefone
);
