import React, { useState, useEffect, useCallback } from 'react';
import Navbar from './components/Navbar';
import Sidebar from './components/Sidebar';
import DashboardView from './components/DashboardView';
import AcademicView from './components/AcademicView';
import GradingView from './components/GradingView';
import QuizzesView from './components/QuizzesView';
import StorageView from './components/StorageView';
import NotificationsView from './components/NotificationsView';
import ForumsView from './components/ForumsView';
import LoginModal from './components/LoginModal';
import { authService } from './services/authService';
import { Bell, X } from 'lucide-react';

export default function App() {
  const [user, setUser] = useState(null);
  const [courses, setCourses] = useState([]);
  const [grades, setGrades] = useState([]);
  const [quizzes, setQuizzes] = useState([]);
  const [files, setFiles] = useState([]);
  const [notifications, setNotifications] = useState([]);
  const [forums, setForums] = useState([]);

  const [activeView, setActiveView] = useState('dashboard');
  const [showNotifDrawer, setShowNotifDrawer] = useState(false);
  const [isLoginModalOpen, setIsLoginModalOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const storedUser = authService.getStoredUser();
    if (storedUser) {
      setUser(storedUser);
    }

    const handleAuthChanged = () => {
      const u = authService.getStoredUser();
      setUser(u);
    };

    window.addEventListener('luxorlms_auth_changed', handleAuthChanged);
    return () => window.removeEventListener('luxorlms_auth_changed', handleAuthChanged);
  }, []);

  const handleLoginSuccess = (authUser, token) => {
    setUser(authUser);
    setIsLoginModalOpen(false);
  };

  const handleLogout = async () => {
    await authService.logout();
    setUser(null);
    setActiveView('dashboard');
  };

  const renderActiveView = () => {
    if (!user) {
      return (
        <div className="animate-fade-in" style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '60vh' }}>
          <div className="glass-panel" style={{ padding: 40, textAlign: 'center', maxWidth: 480 }}>
            <h2 style={{ fontSize: '1.6rem', marginBottom: 10 }}>Welcome to LuxorLMS</h2>
            <p style={{ color: 'var(--text-muted)', marginBottom: 20 }}>Please sign in to access your dashboard, courses, and academic tools.</p>
            <button className="btn-primary" onClick={() => setIsLoginModalOpen(true)}>Sign In</button>
          </div>
        </div>
      );
    }

    switch (activeView) {
      case 'dashboard':
        return <DashboardView user={user} courses={courses} quizzes={quizzes} setActiveView={setActiveView} />;
      case 'academic':
      case 'registration':
        return <AcademicView courses={courses} setCourses={setCourses} user={user} />;
      case 'grading':
        return <GradingView grades={grades} user={user} />;
      case 'quizzes':
        return <QuizzesView quizzes={quizzes} setQuizzes={setQuizzes} user={user} />;
      case 'storage':
        return <StorageView files={files} setFiles={setFiles} user={user} />;
      case 'notifications':
        return <NotificationsView notifications={notifications} setNotifications={setNotifications} user={user} />;
      case 'forums':
        return <ForumsView forums={forums} setForums={setForums} user={user} />;
      default:
        return <DashboardView user={user} courses={courses} quizzes={quizzes} setActiveView={setActiveView} />;
    }
  };

  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <Navbar 
        user={user} 
        onLogout={handleLogout}
        activeView={activeView} 
        setActiveView={setActiveView} 
        notifications={notifications}
        showNotifDrawer={showNotifDrawer}
        setShowNotifDrawer={setShowNotifDrawer}
        openLoginModal={() => setIsLoginModalOpen(true)}
      />

      <div className="app-container">
        <Sidebar activeView={activeView} setActiveView={setActiveView} />

        <main className="main-content">
          {isLoading && (
            <div style={{ position: 'fixed', inset: 0, display: 'flex', alignItems: 'center', justifyContent: 'center', background: 'rgba(0,0,0,0.4)', zIndex: 2000 }}>
              <div className="glass-panel" style={{ padding: 24 }}>Loading...</div>
            </div>
          )}
          {renderActiveView()}
        </main>
      </div>

      <LoginModal 
        isOpen={isLoginModalOpen} 
        onClose={() => setIsLoginModalOpen(false)}
        onLoginSuccess={handleLoginSuccess}
      />

      {showNotifDrawer && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.5)', zIndex: 900 }} onClick={() => setShowNotifDrawer(false)}>
          <div 
            className="glass-panel" 
            style={{ 
              position: 'fixed', top: 78, right: 20, width: 380, maxHeight: '80vh', 
              padding: 20, overflowY: 'auto', borderRadius: 16, zIndex: 1000 
            }}
            onClick={e => e.stopPropagation()}
          >
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 14, pb: 10, borderBottom: '1px solid var(--border-glass)' }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                <Bell size={18} color="var(--accent-cyan)" />
                <h3 style={{ fontSize: '1.05rem' }}>System Notifications</h3>
              </div>
              <X size={18} style={{ cursor: 'pointer' }} onClick={() => setShowNotifDrawer(false)} />
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
              {notifications.map(n => (
                <div key={n.id} style={{ padding: 12, background: 'rgba(15, 23, 42, 0.6)', borderRadius: 10, border: '1px solid var(--border-glass)' }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 4 }}>
                    <span style={{ fontSize: '0.88rem', fontWeight: 700 }}>{n.title}</span>
                    <span className="badge badge-cyan" style={{ fontSize: '0.62rem' }}>{n.channel}</span>
                  </div>
                  <p style={{ fontSize: '0.8rem', color: 'var(--text-muted)' }}>{n.body}</p>
                  <span style={{ fontSize: '0.7rem', color: 'var(--text-dim)', marginTop: 4, display: 'inline-block' }}>{n.createdAt}</span>
                </div>
              ))}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
