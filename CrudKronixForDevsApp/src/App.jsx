import { useState } from 'react'
import AttachmentsPage from './pages/AttachmentsPage'
import FileGenerationPage from './pages/FileGenerationPage'
import LoginScreen from './components/LoginScreen'
import Sidebar from './components/Sidebar'
import MedicinesPage from './pages/MedicinesPage'
import { isTokenExpired } from './utils/tokenExpiry'
import './App.css'

const AUTH_TOKEN_KEY = 'ems-crud-token'
const AUTH_EMAIL_KEY = 'ems-crud-email'
const AUTH_EXPIRES_AT_UTC = 'ems-crud-token-expires-at-utc'

// Componente raiz: mantém o estado de autenticação e decide qual área da
// aplicação deve ser exibida. Também compartilha dados entre as páginas.
function App() {
  const [token, setToken] = useState(
    () => localStorage.getItem(AUTH_TOKEN_KEY) || '',
  )
  const [userEmail, setUserEmail] = useState(
    () => localStorage.getItem(AUTH_EMAIL_KEY) || '',
  )
  const [expiresDateToken, setExpiresDateToken] = useState(
    () => localStorage.getItem(AUTH_EXPIRES_AT_UTC) || '',
  )
  const [activePage, setActivePage] = useState('medicamentos')

  // Persiste a sessão depois do login e atualiza o estado React para trocar
  // imediatamente da tela de acesso para a área autenticada.
  function handleLogin(accessToken, email, expiresAtUtc) {
    localStorage.setItem(AUTH_TOKEN_KEY, accessToken)
    localStorage.setItem(AUTH_EMAIL_KEY, email)
    localStorage.setItem(AUTH_EXPIRES_AT_UTC, expiresAtUtc)
    setToken(accessToken)
    setUserEmail(email)
    setExpiresDateToken(expiresAtUtc)
  }

  // Encerra a sessão tanto no estado atual quanto no armazenamento local,
  // evitando que a próxima atualização restaure uma autenticação já encerrada.
  function handleLogout() {
    localStorage.removeItem(AUTH_TOKEN_KEY)
    localStorage.removeItem(AUTH_EMAIL_KEY)
    localStorage.removeItem(AUTH_EXPIRES_AT_UTC)
    setToken('')
    setUserEmail('')
    setExpiresDateToken('')
  }

  if (!token || isTokenExpired(expiresDateToken)) {
    return <LoginScreen onLogin={handleLogin} />
  }

  return (
    <div className="app-shell">
      <Sidebar
        activePage={activePage}
        userEmail={userEmail}
        onNavigate={setActivePage}
        onLogout={handleLogout}
      />
      <main className="main-content">
        {activePage === 'medicamentos' && <MedicinesPage token={token} />}
        {activePage === 'anexos' && <AttachmentsPage token={token} />}
        {activePage === 'arquivos' && <FileGenerationPage />}
      </main>
    </div>
  )
}

export default App
