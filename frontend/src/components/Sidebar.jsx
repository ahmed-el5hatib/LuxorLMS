import React from 'react';
import { 
  LayoutDashboard, BookOpen, UserPlus, Award, ClockCheck, 
  FolderLock, BellRing, MessagesSquare, Sparkles, LogOut
} from 'lucide-react';

export default function Sidebar({ activeView, setActiveView }) {
  const menuItems = [
    { id: 'dashboard', label: 'Overview Dashboard', icon: LayoutDashboard, badge: 'Core' },
    { id: 'academic', label: 'Academic & Courses', icon: BookOpen, badge: 'M2' },
    { id: 'registration', label: 'Course Registration', icon: UserPlus, badge: 'M3' },
    { id: 'grading', label: 'Grades & Transcript', icon: Award, badge: 'M4' },
    { id: 'quizzes', label: 'Attendance & Quizzes', icon: ClockCheck, badge: 'M5' },
    { id: 'storage', label: 'Cloud Storage & Files', icon: FolderLock, badge: 'M6.1' },
    { id: 'notifications', label: 'Multi-Channel Notifs', icon: BellRing, badge: 'M6.2' },
    { id: 'forums', label: 'Course Forums', icon: MessagesSquare, badge: 'M6.3' },
  ];

  return (
    <aside className="glass-panel" style={{ width: 270, borderRadius: 0, borderTop: 0, borderBottom: 0, borderLeft: 0, padding: '24px 16px', display: 'flex', flexDirection: 'column', justifyContent: 'space-between', minHeight: 'calc(100vh - 77px)' }}>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 6 }}>
        <p style={{ fontSize: '0.72rem', fontWeight: 800, color: 'var(--text-dim)', paddingLeft: 12, marginBottom: 8, letterSpacing: '0.08em', textTransform: 'uppercase' }}>
          Navigation Hub
        </p>

        {menuItems.map(item => {
          const Icon = item.icon;
          const isActive = activeView === item.id;
          return (
            <button
              key={item.id}
              onClick={() => setActiveView(item.id)}
              style={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
                padding: '12px 14px',
                borderRadius: 12,
                fontSize: '0.9rem',
                fontWeight: isActive ? 700 : 500,
                border: 'none',
                cursor: 'pointer',
                background: isActive ? 'linear-gradient(90deg, rgba(56, 189, 248, 0.15), rgba(99, 102, 241, 0.05))' : 'transparent',
                color: isActive ? 'var(--accent-cyan)' : 'var(--text-muted)',
                boxShadow: isActive ? 'inset 3px 0 0 var(--accent-cyan)' : 'none',
                transition: 'all 0.2s ease'
              }}
            >
              <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                <Icon size={19} color={isActive ? 'var(--accent-cyan)' : 'var(--text-muted)'} />
                <span>{item.label}</span>
              </div>
              <span className={`badge ${isActive ? 'badge-cyan' : ''}`} style={{ fontSize: '0.62rem', opacity: isActive ? 1 : 0.4 }}>
                {item.badge}
              </span>
            </button>
          );
        })}
      </div>

      {/* System Status Panel */}
      <div className="glass-panel" style={{ padding: 14, borderRadius: 12, background: 'rgba(15, 23, 42, 0.6)' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 6 }}>
          <Sparkles size={16} color="var(--accent-amber)" />
          <span style={{ fontSize: '0.8rem', fontWeight: 700 }}>Luxor Cluster Active</span>
        </div>
        <p style={{ fontSize: '0.72rem', color: 'var(--text-dim)' }}>PostgreSQL • Redis • Hangfire • S3 Active</p>
      </div>

    </aside>
  );
}
