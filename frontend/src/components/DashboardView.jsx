import React, { useEffect, useState } from 'react';
import { 
  Award, BookOpen, Clock, AlertCircle, FileText, CheckCircle2, 
  TrendingUp, ArrowRight, Zap, FolderUp, MessagesSquare
} from 'lucide-react';
import { apiRequest } from '../services/apiClient';

export default function DashboardView({ user, quizzes, setActiveView }) {
  const [courses, setCourses] = useState([]);
  const [stats, setStats] = useState({ gpa: 0, credits: 0, attendance: 0, pendingQuizzes: 0 });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;

    async function loadDashboard() {
      setLoading(true);
      try {
        const [coursesRes, gradesRes, attendanceRes] = await Promise.all([
          apiRequest('/academic/courses'),
          apiRequest('/grading/my-grades'),
          apiRequest('/attendance/my-attendance'),
        ]);

        if (!cancelled) {
          const coursesData = coursesRes.success ? coursesRes.data : [];
          const gradesData = gradesRes.success ? gradesRes.data : [];
          const attendanceData = attendanceRes.success ? attendanceRes.data : {};

          setCourses(coursesData);

          const gpa = gradesData.length ? (gradesData.reduce((sum, g) => sum + (g.score || 0), 0) / gradesData.length / 20).toFixed(2) : '0.00';
          const credits = coursesData.filter(c => c.isEnrolled).reduce((sum, c) => sum + (c.creditHours || 0), 0);
          const pendingQuizzes = quizzes.filter(q => q.status === 'Pending').length;

          setStats({
            gpa: parseFloat(gpa),
            credits,
            attendance: attendanceData.rate || 0,
            pendingQuizzes,
          });
        }
      } catch (err) {
        console.error('Failed to load dashboard', err);
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    loadDashboard();
    return () => { cancelled = true; };
  }, [quizzes]);

  if (loading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '40vh' }}>
        <div className="glass-panel" style={{ padding: 24 }}>Loading dashboard...</div>
      </div>
    );
  }

  return (
    <div className="animate-fade-in" style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      
      {/* Welcome Banner */}
      <div className="glass-panel glass-panel-hover" style={{ 
        padding: 32, 
        background: 'linear-gradient(135deg, rgba(15, 23, 42, 0.9), rgba(30, 41, 59, 0.7))',
        backgroundSize: 'cover',
        backgroundPosition: 'center',
        position: 'relative',
        overflow: 'hidden'
      }}>
        <div style={{ position: 'relative', zIndex: 2, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 8 }}>
              <span className="badge badge-emerald">Spring Semester 2026</span>
              <span className="badge badge-purple">Academic Honor Roll</span>
            </div>
            <h1 style={{ fontSize: '2.2rem', marginBottom: 6 }}>Welcome back, {user?.username || 'User'}! 👋</h1>
            <p style={{ color: 'var(--text-muted)', fontSize: '0.98rem', maxWidth: 620 }}>
              You are currently enrolled in <strong>{courses.filter(c => c.isEnrolled).length} active courses</strong> with a cumulative GPA of <span style={{ color: 'var(--accent-amber)', fontWeight: 800 }}>{stats.gpa}</span>.
            </p>
          </div>

          <div style={{ display: 'flex', gap: 12 }}>
            <button className="btn-primary" onClick={() => setActiveView('registration')}>
              <Zap size={18} /> Course Registration
            </button>
            <button className="btn-secondary" onClick={() => setActiveView('grading')}>
              <Award size={18} /> View Transcript
            </button>
          </div>
        </div>
      </div>

      {/* Metric Stat Cards */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', gap: 20 }}>
        
        <div className="glass-panel glass-panel-hover" style={{ padding: 20 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
            <span style={{ fontSize: '0.85rem', color: 'var(--text-muted)', fontWeight: 600 }}>Cumulative GPA</span>
            <div style={{ padding: 8, borderRadius: 10, background: 'rgba(245, 158, 11, 0.15)', color: 'var(--accent-amber)' }}>
              <Award size={20} />
            </div>
          </div>
          <p style={{ fontSize: '2.1rem', fontWeight: 800 }} className="gold-gradient-text">{stats.gpa} / 4.00</p>
          <p style={{ fontSize: '0.78rem', color: 'var(--accent-emerald)', marginTop: 4, display: 'flex', alignItems: 'center', gap: 4 }}>
            <TrendingUp size={14} /> Top 5% of Faculty Class
          </p>
        </div>

        <div className="glass-panel glass-panel-hover" style={{ padding: 20 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
            <span style={{ fontSize: '0.85rem', color: 'var(--text-muted)', fontWeight: 600 }}>Completed Credits</span>
            <div style={{ padding: 8, borderRadius: 10, background: 'rgba(56, 189, 248, 0.15)', color: 'var(--accent-cyan)' }}>
              <BookOpen size={20} />
            </div>
          </div>
          <p style={{ fontSize: '2.1rem', fontWeight: 800 }}>{stats.credits} <span style={{ fontSize: '1rem', color: 'var(--text-muted)' }}>/ 136 Cr</span></p>
          <div style={{ width: '100%', height: 6, background: 'var(--bg-glass)', borderRadius: 99, marginTop: 8, overflow: 'hidden' }}>
            <div style={{ width: `${(stats.credits / 136) * 100}%`, height: '100%', background: 'linear-gradient(90deg, #38bdf8, #6366f1)' }}></div>
          </div>
        </div>

        <div className="glass-panel glass-panel-hover" style={{ padding: 20 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
            <span style={{ fontSize: '0.85rem', color: 'var(--text-muted)', fontWeight: 600 }}>Attendance Rate</span>
            <div style={{ padding: 8, borderRadius: 10, background: 'rgba(16, 185, 129, 0.15)', color: 'var(--accent-emerald)' }}>
              <Clock size={20} />
            </div>
          </div>
          <p style={{ fontSize: '2.1rem', fontWeight: 800, color: 'var(--accent-emerald)' }}>{stats.attendance}%</p>
          <p style={{ fontSize: '0.78rem', color: 'var(--text-muted)', marginTop: 4 }}>QR Check-in Verified</p>
        </div>

        <div className="glass-panel glass-panel-hover" style={{ padding: 20 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
            <span style={{ fontSize: '0.85rem', color: 'var(--text-muted)', fontWeight: 600 }}>Active Quizzes</span>
            <div style={{ padding: 8, borderRadius: 10, background: 'rgba(168, 85, 247, 0.15)', color: 'var(--accent-purple)' }}>
              <AlertCircle size={20} />
            </div>
          </div>
          <p style={{ fontSize: '2.1rem', fontWeight: 800 }}>{stats.pendingQuizzes} <span style={{ fontSize: '1rem', color: 'var(--text-muted)' }}>Pending</span></p>
          <p style={{ fontSize: '0.78rem', color: 'var(--accent-amber)', marginTop: 4 }}>Due in 3 days</p>
        </div>

      </div>

      {/* Main Grid Content */}
      <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: 24 }}>
        
        {/* Enrolled Courses Quick View */}
        <div className="glass-panel" style={{ padding: 24 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
            <div>
              <h3 style={{ fontSize: '1.2rem' }}>Current Course Offerings</h3>
              <p style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>Spring 2026 Active Schedule</p>
            </div>
            <button className="btn-secondary" style={{ fontSize: '0.82rem' }} onClick={() => setActiveView('academic')}>
              Explore All Catalog <ArrowRight size={14} />
            </button>
          </div>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))', gap: 16 }}>
            {courses.filter(c => c.isEnrolled).map(course => (
              <div key={course.id} className="glass-panel glass-panel-hover" style={{ padding: 18, background: 'rgba(15, 23, 42, 0.5)' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 8 }}>
                  <span className="badge badge-cyan">{course.courseCode}</span>
                  <span style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>{course.creditHours} Cr</span>
                </div>
                <h4 style={{ fontSize: '1.02rem', marginBottom: 6 }}>{course.nameEn}</h4>
                <p style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: 12 }}>{course.primaryTeacher}</p>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', pt: 8, borderTop: '1px solid var(--border-glass)' }}>
                  <span style={{ fontSize: '0.75rem', color: 'var(--text-dim)' }}>{course.enrolledCount}/{course.capacity} Enrolled</span>
                  <button className="btn-secondary" style={{ padding: '4px 10px', fontSize: '0.75rem' }} onClick={() => setActiveView('forums')}>
                    <MessagesSquare size={12} /> Forum
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Action Shortcuts & Deadlines */}
        <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
          
          <div className="glass-panel" style={{ padding: 20 }}>
            <h3 style={{ fontSize: '1.1rem', marginBottom: 16 }}>Quick Action Hub</h3>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 10 }}>
              <button className="btn-secondary" style={{ justifyContent: 'flex-start' }} onClick={() => setActiveView('storage')}>
                <FolderUp size={18} color="var(--accent-cyan)" /> Cloud File Manager
              </button>
              <button className="btn-secondary" style={{ justifyContent: 'flex-start' }} onClick={() => setActiveView('quizzes')}>
                <Clock size={18} color="var(--accent-purple)" /> Start Active Quiz
              </button>
              <button className="btn-secondary" style={{ justifyContent: 'flex-start' }} onClick={() => setActiveView('forums')}>
                <MessagesSquare size={18} color="var(--accent-amber)" /> Discussion Boards
              </button>
            </div>
          </div>

          <div className="glass-panel" style={{ padding: 20 }}>
            <h3 style={{ fontSize: '1.1rem', marginBottom: 14 }}>Upcoming Deadlines</h3>
            <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
              {quizzes.map(q => (
                <div key={q.id} style={{ display: 'flex', alignItems: 'flex-start', gap: 10, paddingBottom: 10, borderBottom: '1px solid var(--border-glass)' }}>
                  <CheckCircle2 size={16} color={q.status === 'Completed' ? 'var(--accent-emerald)' : 'var(--accent-amber)'} style={{ marginTop: 2 }} />
                  <div>
                    <p style={{ fontSize: '0.88rem', fontWeight: 600 }}>{q.title}</p>
                    <p style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>Due: {new Date(q.dueDate).toLocaleDateString()}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>

        </div>

      </div>

    </div>
  );
}
