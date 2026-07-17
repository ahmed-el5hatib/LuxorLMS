import React, { useState } from 'react';
import { Award, FileText, Download, AlertTriangle, CheckCircle2, ShieldCheck, X } from 'lucide-react';

export default function GradingView({ grades, user }) {
  const [showTranscriptModal, setShowTranscriptModal] = useState(false);
  const [appealCourse, setAppealCourse] = useState(null);
  const [appealReason, setAppealReason] = useState('');
  const [submittedAppeal, setSubmittedAppeal] = useState(false);

  const handleAppealSubmit = (e) => {
    e.preventDefault();
    setSubmittedAppeal(true);
    setTimeout(() => {
      setAppealCourse(null);
      setSubmittedAppeal(false);
      setAppealReason('');
    }, 2000);
  };

  return (
    <div className="animate-fade-in" style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      
      {/* Header */}
      <div className="glass-panel" style={{ padding: 28, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 6 }}>
            <span className="badge badge-amber">Milestone 4</span>
            <span className="badge badge-emerald">Grading & Official Transcripts</span>
          </div>
          <h2 style={{ fontSize: '1.8rem' }}>Student Academic Performance & Transcripts</h2>
          <p style={{ color: 'var(--text-muted)', fontSize: '0.9rem' }}>
            Official Cumulative Grade Point Average (GPA) & Grade Appeals
          </p>
        </div>

        <button className="btn-primary" onClick={() => setShowTranscriptModal(true)}>
          <FileText size={18} /> View Digital Transcript
        </button>
      </div>

      {/* GPA Summary Cards */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))', gap: 20 }}>
        
        <div className="glass-panel" style={{ padding: 24, background: 'linear-gradient(135deg, rgba(245, 158, 11, 0.1), rgba(15, 23, 42, 0.8))' }}>
          <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)', fontWeight: 600 }}>Cumulative GPA (CGPA)</p>
          <h1 style={{ fontSize: '2.8rem', fontWeight: 800, margin: '8px 0' }} className="gold-gradient-text">{user.gpa} / 4.00</h1>
          <p style={{ fontSize: '0.82rem', color: 'var(--accent-emerald)', display: 'flex', alignItems: 'center', gap: 4 }}>
            <ShieldCheck size={16} /> Verified by University Registrar
          </p>
        </div>

        <div className="glass-panel" style={{ padding: 24 }}>
          <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)', fontWeight: 600 }}>Total Earned Credits</p>
          <h1 style={{ fontSize: '2.8rem', fontWeight: 800, margin: '8px 0' }}>{user.completedCredits} <span style={{ fontSize: '1.2rem', color: 'var(--text-muted)' }}>Cr</span></h1>
          <p style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>Required for Graduation: 136 Cr</p>
        </div>

      </div>

      {/* Semester Grades Table */}
      <div className="glass-panel" style={{ padding: 24 }}>
        <h3 style={{ fontSize: '1.2rem', marginBottom: 16 }}>Spring 2026 Grade Records</h3>
        
        <div style={{ overflowX: 'auto' }}>
          <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
            <thead>
              <tr style={{ borderBottom: '1px solid var(--border-glass)', color: 'var(--text-muted)', fontSize: '0.82rem' }}>
                <th style={{ padding: 12 }}>COURSE CODE</th>
                <th style={{ padding: 12 }}>TITLE</th>
                <th style={{ padding: 12 }}>CREDIT HOURS</th>
                <th style={{ padding: 12 }}>SCORE (%)</th>
                <th style={{ padding: 12 }}>LETTER GRADE</th>
                <th style={{ padding: 12 }}>ACTION</th>
              </tr>
            </thead>
            <tbody>
              {grades.map((g, idx) => (
                <tr key={idx} style={{ borderBottom: '1px solid var(--border-glass)', fontSize: '0.9rem' }}>
                  <td style={{ padding: 14 }}><span className="badge badge-cyan">{g.courseCode}</span></td>
                  <td style={{ padding: 14, fontWeight: 600 }}>{g.title}</td>
                  <td style={{ padding: 14 }}>{g.creditHours} Cr</td>
                  <td style={{ padding: 14, fontWeight: 700, color: 'var(--accent-cyan)' }}>{g.score}%</td>
                  <td style={{ padding: 14 }}>
                    <span className="badge badge-emerald" style={{ fontSize: '0.85rem' }}>{g.grade}</span>
                  </td>
                  <td style={{ padding: 14 }}>
                    <button className="btn-secondary" style={{ padding: '4px 10px', fontSize: '0.75rem' }} onClick={() => setAppealCourse(g.courseCode)}>
                      <AlertTriangle size={12} color="var(--accent-amber)" /> Submit Appeal
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Grade Appeal Modal */}
      {appealCourse && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
          <div className="glass-panel" style={{ padding: 28, width: 440, maxWidth: '90%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
              <h3 style={{ fontSize: '1.2rem' }}>Submit Grade Appeal for {appealCourse}</h3>
              <X size={20} style={{ cursor: 'pointer' }} onClick={() => setAppealCourse(null)} />
            </div>

            {submittedAppeal ? (
              <div style={{ textAlign: 'center', padding: 20 }}>
                <CheckCircle2 size={48} color="var(--accent-emerald)" style={{ margin: '0 auto 12px' }} />
                <h4>Appeal Submitted Successfully</h4>
                <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)', marginTop: 4 }}>The academic committee will review your appeal within 5 business days.</p>
              </div>
            ) : (
              <form onSubmit={handleAppealSubmit} style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
                <label style={{ fontSize: '0.85rem', color: 'var(--text-muted)' }}>Justification / Reason for Appeal:</label>
                <textarea 
                  rows={4} 
                  required
                  className="form-input" 
                  placeholder="Explain why you are requesting a grade regrade or review..." 
                  value={appealReason}
                  onChange={e => setAppealReason(e.target.value)}
                />
                <div style={{ display: 'flex', gap: 10, justifyContent: 'flex-end', marginTop: 10 }}>
                  <button type="button" className="btn-secondary" onClick={() => setAppealCourse(null)}>Cancel</button>
                  <button type="submit" className="btn-primary">Submit Appeal Request</button>
                </div>
              </form>
            )}
          </div>
        </div>
      )}

      {/* Digital Transcript Modal */}
      {showTranscriptModal && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(10px)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
          <div className="glass-panel" style={{ padding: 36, width: 680, maxWidth: '95%', maxHeight: '90vh', overflowY: 'auto' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 20, pb: 16, borderBottom: '1px solid var(--border-glass)' }}>
              <div>
                <span className="badge badge-emerald">OFFICIAL TRANSCRIPT</span>
                <h2 style={{ fontSize: '1.6rem', marginTop: 4 }}>LUXOR UNIVERSITY REGISTRAR</h2>
                <p style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>Student: {user.fullName} ({user.username})</p>
              </div>
              <X size={24} style={{ cursor: 'pointer' }} onClick={() => setShowTranscriptModal(false)} />
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: 16, marginBottom: 24 }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', background: 'rgba(15, 23, 42, 0.6)', padding: 14, borderRadius: 10 }}>
                <span>Major: Computer Science</span>
                <span>CGPA: <strong>{user.gpa} / 4.00</strong></span>
                <span>Total Credits: <strong>{user.completedCredits} Cr</strong></span>
              </div>

              <h4 style={{ fontSize: '1rem', marginTop: 10 }}>Academic History Records</h4>
              {grades.map((g, idx) => (
                <div key={idx} style={{ display: 'flex', justifyContent: 'space-between', padding: '10px 14px', borderBottom: '1px dotted var(--border-glass)', fontSize: '0.88rem' }}>
                  <span>{g.courseCode} — {g.title} ({g.creditHours} Cr)</span>
                  <span style={{ fontWeight: 700, color: 'var(--accent-cyan)' }}>Grade: {g.grade} ({g.score}%)</span>
                </div>
              ))}
            </div>

            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', pt: 16, borderTop: '1px solid var(--border-glass)' }}>
              <span style={{ fontSize: '0.75rem', color: 'var(--text-dim)' }}>Verification Hash: 8F91A-LUXOR-REG-2026</span>
              <button className="btn-primary" onClick={() => alert("Downloading Official PDF Transcript...")}>
                <Download size={16} /> Export PDF Certificate
              </button>
            </div>
          </div>
        </div>
      )}

    </div>
  );
}
