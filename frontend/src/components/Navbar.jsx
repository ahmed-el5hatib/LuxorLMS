import React, { useState, useEffect } from 'react';
import { 
  GraduationCap, Bell, Search, UserCheck, Shield, Key, 
  LogOut, Lock, CheckCircle2, Copy
} from 'lucide-react';
import { authService } from '../services/authService';

export default function Navbar({ user, setUser, activeView, setActiveView, notifications, showNotifDrawer, setShowNotifDrawer, openLoginModal }) {
  const [time, setTime] = useState(new Date().toLocaleTimeString());
  const [showTokenPopup, setShowTokenPopup] = useState(false);
  const currentToken = authService.getAccessToken();

  useEffect(() => {
    const timer = setInterval(() => setTime(new Date().toLocaleTimeString()), 1000);
    return () => clearInterval(timer);
  }, []);

  const roles = ["Student", "Doctor", "TA", "Admin"];

  const handleLogout = () => {
    authService.logout();
    setUser({ ...user, role: "Student" });
  };

  return (
    <header className="glass-panel" style={{ borderRadius: 0, borderTop: 0, borderLeft: 0, borderRight: 0, padding: '16px 32px', position: 'sticky', top: 0, zIndex: 100 }}>
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        
        {/* Brand Logo */}
        <div style={{ display: 'flex', alignItems: 'center', gap: 14, cursor: 'pointer' }} onClick={() => setActiveView('dashboard')}>
          <div style={{ 
            width: 44, height: 44, borderRadius: 12, 
            background: 'linear-gradient(135deg, #38bdf8, #6366f1)',
            display: 'flex', alignItems: 'center', justifyContent: 'center',
            boxShadow: '0 0 20px rgba(56, 189, 248, 0.4)'
          }}>
            <GraduationCap size={26} color="#fff" />
          </div>
          <div>
            <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
              <span style={{ fontSize: '1.4rem', fontWeight: 800 }} className="gradient-text">LUXOR LMS</span>
              <span className="badge badge-cyan" style={{ fontSize: '0.65rem' }}>JWT ENTERPRISE</span>
            </div>
            <p style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>Enterprise Academic Suite</p>
          </div>
        </div>

        {/* Global Search Bar */}
        <div style={{ display: 'flex', alignItems: 'center', gap: 10, background: 'rgba(15, 23, 42, 0.6)', padding: '8px 16px', borderRadius: 12, border: '1px solid var(--border-glass)', width: 340 }}>
          <Search size={18} color="var(--text-muted)" />
          <input 
            type="text" 
            placeholder="Search courses, files, forums, grades..." 
            style={{ background: 'transparent', border: 'none', color: '#fff', fontSize: '0.88rem', outline: 'none', width: '100%' }}
          />
          <kbd style={{ background: 'var(--bg-glass)', padding: '2px 6px', borderRadius: 4, fontSize: '0.7rem', color: 'var(--text-muted)' }}>Ctrl K</kbd>
        </div>

        {/* User Controls & Role Switcher */}
        <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
          
          <div style={{ fontSize: '0.82rem', color: 'var(--text-muted)', fontFamily: 'monospace' }}>
            {time}
          </div>

          {/* Role Claims Selector */}
          <div style={{ display: 'flex', alignItems: 'center', gap: 4, background: 'var(--bg-glass)', padding: 4, borderRadius: 10, border: '1px solid var(--border-glass)' }}>
            <Shield size={14} color="var(--accent-amber)" style={{ marginLeft: 6 }} />
            {roles.map(role => (
              <button
                key={role}
                onClick={() => setUser({ ...user, role })}
                style={{
                  padding: '4px 8px',
                  borderRadius: 6,
                  fontSize: '0.75rem',
                  fontWeight: 600,
                  border: 'none',
                  cursor: 'pointer',
                  background: user.role === role ? 'linear-gradient(135deg, var(--accent-cyan), var(--accent-blue))' : 'transparent',
                  color: user.role === role ? '#fff' : 'var(--text-muted)',
                  transition: 'all 0.2s ease'
                }}
              >
                {role}
              </button>
            ))}
          </div>

          {/* JWT Auth Action Button */}
          <button className="btn-secondary" style={{ padding: '6px 12px', fontSize: '0.78rem' }} onClick={openLoginModal}>
            <Key size={14} color="var(--accent-cyan)" /> JWT Login
          </button>

          {/* Notifications Bell */}
          <div style={{ position: 'relative' }}>
            <button 
              onClick={() => setShowNotifDrawer(!showNotifDrawer)}
              className="btn-secondary" 
              style={{ padding: 10, borderRadius: 12, position: 'relative' }}
            >
              <Bell size={20} color={showNotifDrawer ? 'var(--accent-cyan)' : 'var(--text-main)'} />
              {notifications.length > 0 && (
                <span style={{
                  position: 'absolute', top: -4, right: -4, width: 18, height: 18,
                  borderRadius: 999, background: 'var(--accent-rose)', color: '#fff',
                  fontSize: '0.7rem', fontWeight: 800, display: 'flex', alignItems: 'center', justifyContent: 'center'
                }}>
                  {notifications.length}
                </span>
              )}
            </button>
          </div>

          {/* User Profile */}
          <div style={{ display: 'flex', alignItems: 'center', gap: 10, paddingLeft: 10, borderLeft: '1px solid var(--border-glass)' }}>
            <img src={user.avatar} alt="Avatar" style={{ width: 38, height: 38, borderRadius: 10, objectFit: 'cover', border: '2px solid var(--accent-cyan)' }} />
            <div>
              <p style={{ fontSize: '0.88rem', fontWeight: 700 }}>{user.fullName}</p>
              <p style={{ fontSize: '0.72rem', color: 'var(--text-muted)' }}>{user.role} • RBAC Claim</p>
            </div>
          </div>

        </div>

      </div>
    </header>
  );
}
