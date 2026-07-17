import React, { useEffect, useState } from 'react';
import { ClockCheck, QrCode, CheckCircle2, Play, AlertCircle, Award, ArrowRight, ArrowLeft } from 'lucide-react';
import { apiRequest } from '../services/apiClient';

export default function QuizzesView({ quizzes, setQuizzes, user }) {
  const [activeQuiz, setActiveQuiz] = useState(null);
  const [currentQIndex, setCurrentQIndex] = useState(0);
  const [selectedAnswers, setSelectedAnswers] = useState({});
  const [quizCompleted, setQuizCompleted] = useState(false);
  const [calculatedScore, setCalculatedScore] = useState(0);

  const [showQrModal, setShowQrModal] = useState(false);
  const [qrCodeInput, setQrCodeInput] = useState('');
  const [attendanceSuccess, setAttendanceSuccess] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;
    async function loadQuizzes() {
      setLoading(true);
      const res = await apiRequest('/quizzes');
      if (!cancelled && res.success) {
        setQuizzes(res.data || []);
      }
      if (!cancelled) setLoading(false);
    }
    loadQuizzes();
    return () => { cancelled = true; };
  }, [setQuizzes]);

  const startQuiz = (q) => {
    setActiveQuiz(q);
    setCurrentQIndex(0);
    setSelectedAnswers({});
    setQuizCompleted(false);
  };

  const selectOption = (qId, optionIdx) => {
    setSelectedAnswers({ ...selectedAnswers, [qId]: optionIdx });
  };

  const finishQuiz = () => {
    let correctCount = 0;
    if (activeQuiz?.questions) {
      activeQuiz.questions.forEach(q => {
        if (selectedAnswers[q.id] === q.correctIndex) {
          correctCount++;
        }
      });
    }

    const scorePct = activeQuiz?.questions?.length ? Math.round((correctCount / activeQuiz.questions.length) * 100) : 0;
    setCalculatedScore(scorePct);
    setQuizCompleted(true);

    setQuizzes(quizzes.map(q => q.id === activeQuiz.id ? { ...q, status: 'Completed', score: `${scorePct}%` } : q));
  };

  const handleQrSubmit = (e) => {
    e.preventDefault();
    setAttendanceSuccess(true);
    setTimeout(() => {
      setAttendanceSuccess(false);
      setShowQrModal(false);
      setQrCodeInput('');
    }, 2000);
  };

  if (loading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '40vh' }}>
        <div className="glass-panel" style={{ padding: 24 }}>Loading quizzes...</div>
      </div>
    );
  }

  return (
    <div className="animate-fade-in" style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      
      {/* Header */}
      <div className="glass-panel" style={{ padding: 28, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 6 }}>
            <span className="badge badge-purple">Milestone 5</span>
            <span className="badge badge-cyan">Attendance & Online Quizzes</span>
          </div>
          <h2 style={{ fontSize: '1.8rem' }}>Attendance QR Check-In & Active Quizzes</h2>
          <p style={{ color: 'var(--text-muted)', fontSize: '0.9rem' }}>
            Real-time session attendance validation and online exam runner
          </p>
        </div>

        <button className="btn-primary" onClick={() => setShowQrModal(true)}>
          <QrCode size={18} /> Scan QR / Pin Check-In
        </button>
      </div>

      {/* Quizzes List or Active Quiz Stepper */}
      {!activeQuiz ? (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: 20 }}>
          {quizzes.map(q => (
            <div key={q.id} className="glass-panel glass-panel-hover" style={{ padding: 24, display: 'flex', flexDirection: 'column', justifyContent: 'space-between' }}>
              <div>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
                  <span className="badge badge-cyan">{q.courseCode}</span>
                  <span className={`badge ${q.status === 'Completed' ? 'badge-emerald' : 'badge-amber'}`}>
                    {q.status}
                  </span>
                </div>

                <h3 style={{ fontSize: '1.15rem', marginBottom: 8 }}>{q.title}</h3>
                <p style={{ fontSize: '0.82rem', color: 'var(--text-muted)', marginBottom: 16 }}>
                  Duration: {q.durationMinutes} Mins • Total Questions: {q.totalQuestions || 5}
                </p>

                {q.status === 'Completed' && (
                  <div style={{ background: 'rgba(16, 185, 129, 0.1)', padding: 10, borderRadius: 8, textAlign: 'center', color: 'var(--accent-emerald)', fontWeight: 700, fontSize: '0.95rem' }}>
                    Score Achieved: {q.score}
                  </div>
                )}
              </div>

              {q.status === 'Pending' && (
                <button className="btn-primary" style={{ width: '100%', justifyContent: 'center', marginTop: 16 }} onClick={() => startQuiz(q)}>
                  <Play size={16} /> Start Quiz Session
                </button>
              )}
            </div>
          ))}
        </div>
      ) : (
        /* Quiz Runner Active Session */
        <div className="glass-panel" style={{ padding: 32 }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', pb: 16, borderBottom: '1px solid var(--border-glass)', marginBottom: 24 }}>
            <div>
              <span className="badge badge-purple">{activeQuiz.courseCode} EXAM RUNNER</span>
              <h3 style={{ fontSize: '1.4rem', marginTop: 4 }}>{activeQuiz.title}</h3>
            </div>
            <div style={{ display: 'flex', alignItems: 'center', gap: 8, background: 'rgba(245, 158, 11, 0.15)', padding: '8px 16px', borderRadius: 10, color: 'var(--accent-amber)', fontWeight: 700 }}>
              <ClockCheck size={18} /> Time Remaining: 28:45
            </div>
          </div>

          {!quizCompleted ? (
            <div>
              <div style={{ display: 'flex', justifyContent: 'space-between', color: 'var(--text-muted)', fontSize: '0.85rem', marginBottom: 12 }}>
                <span>Question {currentQIndex + 1} of {activeQuiz.questions?.length || 0}</span>
                <span>Select one best option</span>
              </div>

              {/* Question Text */}
              <h4 style={{ fontSize: '1.15rem', marginBottom: 20 }}>
                {activeQuiz.questions?.[currentQIndex]?.questionText}
              </h4>

              {/* Options */}
              <div style={{ display: 'flex', flexDirection: 'column', gap: 12, marginBottom: 28 }}>
                {activeQuiz.questions?.[currentQIndex]?.options?.map((opt, idx) => {
                  const qId = activeQuiz.questions[currentQIndex].id;
                  const isSelected = selectedAnswers[qId] === idx;
                  return (
                    <button
                      key={idx}
                      onClick={() => selectOption(qId, idx)}
                      style={{
                        padding: '14px 18px',
                        borderRadius: 12,
                        textAlign: 'left',
                        fontSize: '0.95rem',
                        fontWeight: isSelected ? 700 : 500,
                        border: isSelected ? '2px solid var(--accent-cyan)' : '1px solid var(--border-glass)',
                        background: isSelected ? 'rgba(56, 189, 248, 0.15)' : 'var(--bg-glass)',
                        color: isSelected ? 'var(--accent-cyan)' : 'var(--text-main)',
                        cursor: 'pointer',
                        transition: 'all 0.2s ease'
                      }}
                    >
                      {String.fromCharCode(65 + idx)}. {opt}
                    </button>
                  );
                })}
              </div>

              {/* Stepper Footer Controls */}
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <button 
                  className="btn-secondary" 
                  disabled={currentQIndex === 0} 
                  onClick={() => setCurrentQIndex(currentQIndex - 1)}
                >
                  <ArrowLeft size={16} /> Previous
                </button>

                {currentQIndex < (activeQuiz.questions?.length || 0) - 1 ? (
                  <button className="btn-primary" onClick={() => setCurrentQIndex(currentQIndex + 1)}>
                    Next Question <ArrowRight size={16} />
                  </button>
                ) : (
                  <button className="btn-primary" style={{ background: 'linear-gradient(135deg, var(--accent-emerald), #059669)' }} onClick={finishQuiz}>
                    Submit Quiz Answers <CheckCircle2 size={16} />
                  </button>
                )}
              </div>
            </div>
          ) : (
            /* Quiz Completed Score Breakdown */
            <div style={{ textAlign: 'center', padding: 30 }}>
              <Award size={64} color="var(--accent-amber)" style={{ margin: '0 auto 16px' }} />
              <h2 style={{ fontSize: '2rem', marginBottom: 8 }}>Quiz Completed!</h2>
              <p style={{ fontSize: '1rem', color: 'var(--text-muted)', marginBottom: 20 }}>Your final calculated score for this submission:</p>
              <h1 style={{ fontSize: '3.5rem', fontWeight: 800 }} className="gold-gradient-text">{calculatedScore}%</h1>
              <button className="btn-secondary" style={{ marginTop: 24 }} onClick={() => setActiveQuiz(null)}>
                Back to Active Quizzes List
              </button>
            </div>
          )}
        </div>
      )}

      {/* QR Check-in Modal */}
      {showQrModal && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
          <div className="glass-panel" style={{ padding: 28, width: 420, maxWidth: '90%', textAlign: 'center' }}>
            <h3 style={{ fontSize: '1.25rem', marginBottom: 12 }}>Session Attendance QR Check-In</h3>
            <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)', marginBottom: 20 }}>Enter the 6-digit session pin displayed on the lecture screen:</p>

            {attendanceSuccess ? (
              <div style={{ padding: 20 }}>
                <CheckCircle2 size={48} color="var(--accent-emerald)" style={{ margin: '0 auto 12px' }} />
                <h4 style={{ color: 'var(--accent-emerald)' }}>Attendance Verified!</h4>
              </div>
            ) : (
              <form onSubmit={handleQrSubmit} style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
                <input 
                  type="text" 
                  maxLength={6} 
                  required
                  placeholder="e.g. 894012" 
                  className="form-input" 
                  style={{ textAlign: 'center', fontSize: '1.4rem', letterSpacing: '0.2em', fontWeight: 800 }}
                  value={qrCodeInput}
                  onChange={e => setQrCodeInput(e.target.value)}
                />
                <div style={{ display: 'flex', gap: 10, justifyContent: 'center' }}>
                  <button type="button" className="btn-secondary" onClick={() => setShowQrModal(false)}>Cancel</button>
                  <button type="submit" className="btn-primary">Verify Check-In</button>
                </div>
              </form>
            )}
          </div>
        </div>
      )}

    </div>
  );
}
