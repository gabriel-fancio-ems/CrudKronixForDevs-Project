import { useState } from 'react'
import { login } from '../services/api'
import { getErrorMessage } from '../utils/errors'
import { formatExpiresAtUtc } from '../utils/tokenExpiry'

// Normaliza os formatos de token mais comuns retornados pela API de login.
// Algumas APIs retornam o token diretamente; outras o envolvem em um objeto.
function getToken(session) {
  if (typeof session === 'string') return session
  return (
    session?.token ||
    session?.accessToken ||
    session?.jwt ||
    session?.data?.token ||
    session?.data?.accessToken ||
    ''
  )
}

// Tela responsável por coletar as credenciais, chamar a API e comunicar ao
// componente raiz quando a autenticação for concluída com sucesso.
function LoginScreen({ onLogin }) {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  // Interrompe o envio HTML padrão, solicita autenticação e atualiza o estado
  // visual de carregamento/erro durante a chamada assíncrona.
  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setLoading(true)
    try {
      const session = await login(email, password)
      const token = getToken(session)
      if (!token) {
        throw new Error('A resposta da API não incluiu um token de acesso.')
      }
      const expiresAtUtc =
        session?.expiresAtUtc || session?.data?.expiresAtUtc || ''
      onLogin(token, email, formatExpiresAtUtc(expiresAtUtc))
    } catch (loginError) {
      setError(getErrorMessage(loginError))
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="login-page">
      <form className="login-card" onSubmit={handleSubmit}>
        <div className="brand brand-login">
          <span className="brand-mark">GRUPO EMS</span>
          <span className="brand-caption">FARMACÊUTICA</span>
        </div>
        <p className="eyebrow">ÁREA RESTRITA</p>
        <h1>Acesse sua conta</h1>
        <p className="login-description">
          Entre para gerenciar o catálogo de medicamentos.
        </p>
        <label className="field">
          <span>Email</span>
          <input
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            autoComplete="username"
            required
          />
        </label>
        <label className="field">
          <span>Senha</span>
          <input
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            autoComplete="current-password"
            required
          />
        </label>
        {error && <p className="message message-error">{error}</p>}
        <button className="button button-primary login-button" disabled={loading}>
          {loading ? 'Entrando...' : 'Entrar'}
        </button>
      </form>
    </main>
  )
}

export default LoginScreen
