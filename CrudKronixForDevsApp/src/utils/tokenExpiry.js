const EXPIRY_PATTERN = /^(\d{2})\/(\d{2})\/(\d{4}) (\d{2}):(\d{2}):(\d{2}) UTC$/

// Converte a data de expiração da API para "dd/MM/yyyy HH:mm:ss UTC".
// O sufixo UTC é literal: a formatação sempre usa o fuso universal, não o local.
export function formatExpiresAtUtc(value) {
  if (typeof value !== 'string' || !value.trim()) {
    throw new Error('A resposta da API não incluiu expiresAtUtc.')
  }

  const date = parseExpiresAtUtc(value)
  if (!date) {
    throw new Error('A data expiresAtUtc retornada pela API é inválida.')
  }

  const parts = new Intl.DateTimeFormat('en-GB', {
    timeZone: 'UTC',
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
    hourCycle: 'h23',
  }).formatToParts(date)
  const values = Object.fromEntries(parts.map(({ type, value: part }) => [type, part]))

  return `${values.day}/${values.month}/${values.year} ${values.hour}:${values.minute}:${values.second} UTC`
}

// Lê o formato persistido e monta uma data em UTC para comparação segura,
// validando os componentes para rejeitar datas impossíveis, como 31/02.
export function parseExpiresAtUtc(value) {
  if (typeof value !== 'string') return null

  const formattedMatch = value.match(EXPIRY_PATTERN)
  if (formattedMatch) {
    const [, day, month, year, hour, minute, second] = formattedMatch
    const date = new Date(
      Date.UTC(
        Number(year),
        Number(month) - 1,
        Number(day),
        Number(hour),
        Number(minute),
        Number(second),
      ),
    )

    if (
      date.getUTCFullYear() !== Number(year) ||
      date.getUTCMonth() !== Number(month) - 1 ||
      date.getUTCDate() !== Number(day) ||
      date.getUTCHours() !== Number(hour) ||
      date.getUTCMinutes() !== Number(minute) ||
      date.getUTCSeconds() !== Number(second)
    ) {
      return null
    }
    return date
  }

  // ISO 8601 é o formato padrão de serialização de DateTime no .NET.
  const isoUtcPattern =
    /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d+)?(?:Z|[+-]\d{2}:\d{2})$/
  if (!isoUtcPattern.test(value)) return null

  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? null : date
}

// Um valor ausente ou inválido é tratado como expirado: é mais seguro pedir
// autenticação novamente do que manter uma sessão sem expiração verificável.
export function isTokenExpired(value, now = new Date()) {
  const expiresAt = parseExpiresAtUtc(value)
  return !expiresAt || expiresAt.getTime() <= now.getTime()
}
