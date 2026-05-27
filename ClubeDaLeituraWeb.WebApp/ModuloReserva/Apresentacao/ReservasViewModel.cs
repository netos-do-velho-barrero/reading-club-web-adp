using System.ComponentModel.DataAnnotations;

namespace ClubeDaLeituraWeb.WebApp.ModuloReserva.Apresentacao;

public record ListarReservasViewModel(
    string Id,
    string NomeAmigo,
    string TituloRevista,
    DateTime DataReserva,
    string Status
);

public record CadastrarReservaViewModel(
    [Required(ErrorMessage = "O campo \"Amigo\" deve ser preenchido.")]
    string AmigoId,

    [Required(ErrorMessage = "O campo \"Revista\" deve ser preenchido.")]
    string RevistaId
);

public record CancelarReservaViewModel(
    string Id,
    string NomeAmigo,
    string TituloRevista,
    DateTime DataReserva
);

public record ConverterReservaViewModel(
    string Id,
    string NomeAmigo,
    string TituloRevista,
    DateTime DataReserva
);
