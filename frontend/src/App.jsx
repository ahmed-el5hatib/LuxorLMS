import React, { useState, useEffect } from 'react';
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

import { 
  INITIAL_USER, INITIAL_COURSES, INITIAL_GRADES, 
  INITIAL_QUIZZES, INITIAL_FILES, INITIAL_NOTIFICATIONS, INITIAL_FORUMS 
} from './services/apiMock';
import { authService } from './services/authService';
import { Bell, X } from 'lucide-react';

export default function App() {
  const [user, setUser] = useState(INITIAL_USER);
  const [courses, setCourses] = useState(INITIAL_COURSES);
  const [grades, setGrades] = useState(INITIAL_GRADES);
  const [quizzes, setQuizzes] = useState(INITIAL_QUIZZES);
  const [files, setFiles] = useState(INITIAL_FILES);
  const [notifications, setNotifications] = useState(INITIAL_NOTIFICATIONS);
  const [forums, setForums] = useState(INITIAL_FORUMS);

  const [activeView, setActiveView] = useState('dashboard');
  const [showNotifDrawer, setShowNotifDrawer] = useState(false);
  const [isLoginModalOpen, setIsLoginModalOpen] = useState(false);

  useEffect(() => {
    const storedUser = authService.getStoredUser();
    if (storedUser) {
      setUser(prev => ({
        ...prev,
        username: storedUser.username || prev.username,
        email: storedUser.email || prev.email,
        role: storedUser.role || prev.role,
      }));
    }

    const handleAuthChanged = () => {
      const u = authService.getStoredUser();
      if (u) {
        setUser(prev => ({
          ...prev,
          username: u.username || prev.username,
          email: u.email || prev.email,
          role: u.role || prev.role,
        }));
      }
    };

    window.addEventListener('luxorlms_auth_changed', handleAuthChanged);
    return () => window.removeEventListener('luxorlms_auth_changed', handleAuthChanged);
  }, []);

  const handleLoginSuccess = (authUser, token) => {
    setUser(prev => ({
      ...prev,
      username: authUser.username || prev.username,
      email: authUser.email || prev.email,
      role: authUser.role || prev.role,
      fullName: authUser.username ? authUser.username.replace('.', ' ').toUpperCase() : prev.fullName,
    }));
  };

  const renderActiveView = () => {
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
      
      {/* Top Navbar */}
      <Navbar 
        user={user} 
        setUser={setUser} 
        activeView={activeView} 
        setActiveView={setActiveView} 
        notifications={notifications}
        showNotifDrawer={showNotifDrawer}
        setShowNotifDrawer={setShowNotifDrawer}
        openLoginModal={() => setIsLoginModalOpen(true)}
      />

      <div className="app-container">
        
        {/* Left Sidebar */}
        <Sidebar activeView={activeView} setActiveView={setActiveView} />

        {/* Main Content Area */}
        <main className="main-content">
          {renderActiveView()}
        </main>

      </div>

      {/* Login / JWT Auth Modal */}
      <LoginModal 
        isOpen={isLoginModalOpen} 
        onClose={() => setIsLoginModalOpen(false)}
        onLoginSuccess={handleLoginSuccess}
      />

      {/* Slide-over Notifications Drawer */}
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
