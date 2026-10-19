namespace CrudKronixForDevs.DTOs;

public class MedicamentoFornecedorDto
{
    public Guid MedicamentoId { get; set; }

    public string NomeMedicamento { get; set; } = string.Empty;

    public Guid FornecedorId { get; set; }

    public string NomeFornecedor { get; set; } = string.Empty;
}
