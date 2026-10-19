import { Pill } from 'lucide-react'

const menuItems = [
  { id: 'medicamentos', label: 'Medicamentos', icon: Pill },
]

// Renderiza a navegação lateral. O estado de página e as ações pertencem ao
// componente pai; este componente apenas apresenta os itens e encaminha cliques.
function Sidebar({ activePage, userEmail, onNavigate, onLogout }) {
  return (
    <aside className="sidebar">
      <div className="brand">
        <span className="brand-mark">GRUPO EMS</span>
        <span className="brand-caption">FARMACÊUTICA</span>
      </div>
      <div className="sidebar-label">MENU PRINCIPAL</div>
      <nav aria-label="Menu principal">
        {menuItems.map(({ id, label, icon: MenuIcon }) => (
          <button
            key={id}
            className={`nav-item ${activePage === id ? 'active' : ''}`}
            onClick={() => onNavigate(id)}
            aria-current={activePage === id ? 'page' : undefined}
          >
            <MenuIcon size={20} aria-hidden="true" />
            <span>{label}</span>
          </button>
        ))}
      </nav>
      <div className="sidebar-bottom">
        <div className="user-avatar">{userEmail.slice(0, 1).toUpperCase()}</div>
        <div className="user-info">
          <span className="user-email">{userEmail}</span>
          <button className="logout-button" onClick={onLogout}>
            Sair
          </button>
        </div>
      </div>
    </aside>
  )
}

export default Sidebar
