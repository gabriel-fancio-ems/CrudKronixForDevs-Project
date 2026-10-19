// Converte uma data ISO (ou timestamp que comece por YYYY-MM-DD) para o padrão
// brasileiro. Cria a data usando componentes locais para evitar deslocamento
// de dia causado pela interpretação UTC de strings sem horário.
export function formatDate(value) {
  if (!value) return '—'
  const [year, month, day] = value.slice(0, 10).split('-').map(Number)
  if (!year || !month || !day) return value
  return new Intl.DateTimeFormat('pt-BR').format(
    new Date(year, month - 1, day),
  )
}
