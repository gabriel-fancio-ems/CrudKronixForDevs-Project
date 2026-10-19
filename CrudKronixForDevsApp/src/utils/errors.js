// Escolhe a mensagem mais útil disponível em erros HTTP ou JavaScript comuns.
// Mantém uma mensagem genérica como último recurso, evitando texto vazio na UI.
export function getErrorMessage(error) {
  return (
    error.response?.data?.message ||
    error.response?.data?.title ||
    error.message ||
    'Ocorreu um erro inesperado.'
  )
}
