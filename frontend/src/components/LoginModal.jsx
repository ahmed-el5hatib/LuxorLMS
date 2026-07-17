import React, { useState } from 'react';
import { ShieldCheck, Lock, User, Key, X, CheckCircle2, AlertCircle, Eye, EyeOff, Sparkles, Server } from 'lucide-react';
import { authService } from '../services/authService';

export default function LoginModal({ isOpen, onClose, onLoginSuccess }) {
  const [usernameOrEmail, setUsernameOrEmail] = useState('ahmed.elkhatib');
  const [password, setPassword] = useState('LuxorPass123!');
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [errorMsg, setErrorMsg] = useState('');
  const [isSimulated, setIsSimulated] = useState(false);

  const [mfaRequired, setMfaRequired] = useState(false);
  const [mfaCode, setMfaCode] = useState('');

  if (!isOpen) return null;

  const handleLogin = async (e) => {
    e.preventDefault();
    setErrorMsg('');
    setIsLoading(true);

    const result = await authService.login(usernameOrEmail, password);
    setIsLoading(false);

    if (result.mfaRequired) {
      setMfaRequired(true);
      return;
    }

    if (result.success) {
      setIsSimulated(!!result.isSimulated);
      onLoginSuccess(result.user, result.accessToken);
      onClose();
    } else {
      setErrorMsg(result.description || 'Authentication failed. Please check credentials.');
    }
  };

  const handleMfaVerify = async (e) => {
    e.preventDefault();
    if (mfaCode.length !== 6) {
      setErrorMsg('MFA Code must be 6 digits.');
      return;
    }
    const result = await authService.login(usernameOrEmail, password);
    onLoginSuccess(result.user, result.accessToken);
    onClose();
  };

  return (
    <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.8)', backdropFilter: 'blur(12px)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 2000 }}>
      <div className="glass-panel" style={{ padding: 36, width: 460, maxWidth: '92%', boxShadow: '0 0 40px rgba(56, 189, 248, 0.2)' }}>
        
        {/* Header */}
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 20 }}>
          <div>
            <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 6 }}>
              <span className="badge badge-cyan">Identity & RBAC M1</span>
              <span className="badge badge-purple"><Server size={10} /> ASP.NET Core API</span>
            </div>
            <h2 style={{ fontSize: '1.6rem' }} className="gradient-text">JWT Authentication</h2>
          </div>
          <X size={20} style={{ cursor: 'pointer' }} onClick={onClose} />
        </div>

        {errorMsg && (
          <div style={{ background: 'rgba(244, 63, 94, 0.15)', border: '1px solid rgba(244, 63, 94, 0.3)', padding: 12, borderRadius: 10, color: 'var(--accent-rose)', fontSize: '0.85rem', marginBottom: 16, display: 'flex', alignItems: 'center', gap: 8 }}>
            <AlertCircle size={16} /> {errorMsg}
          </div>
        )}

        {!mfaRequired ? (
          <form onSubmit={handleLogin} style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
            <div>
              <label style={{ fontSize: '0.82rem', color: 'var(--text-muted)', marginBottom: 4, display: 'block' }}>Username or Academic Email:</label>
              <div style={{ position: 'relative' }}>
                <User size={16} color="var(--text-muted)" style={{ position: 'absolute', left: 12, top: 12 }} />
                <input 
                  type="text" 
                  required 
                  className="form-input" 
                  style={{ paddingLeft: 38 }}
                  value={usernameOrEmail}
                  onChange={e => setUsernameOrEmail(e.target.value)}
                />
              </div>
            </div>

            <div>
              <label style={{ fontSize: '0.82rem', color: 'var(--text-muted)', marginBottom: 4, display: 'block' }}>Account Password:</label>
              <div style={{ position: 'relative' }}>
                <Lock size={16} color="var(--text-muted)" style={{ position: 'absolute', left: 12, top: 12 }} />
                <input 
                  type={showPassword ? "text" : "password"} 
                  required 
                  className="form-input" 
                  style={{ paddingLeft: 38, paddingRight: 38 }}
                  value={password}
                  onChange={e => setPassword(e.target.value)}
                />
                <button type="button" onClick={() => setShowPassword(!showPassword)} style={{ position: 'absolute', right: 12, top: 12, background: 'none', border: 'none', cursor: 'pointer', color: 'var(--text-muted)' }}>
                  {showPassword ? <EyeOff size={16} /> : <Eye size={16} />}
                </button>
              </div>
            </div>

            {/* Role Helper Presets */}
            <div>
              <p style={{ fontSize: '0.75rem', color: 'var(--text-muted)', marginBottom: 6 }}>Quick Fill Credentials:</p>
              <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
                <button type="button" className="btn-secondary" style={{ padding: '3px 8px', fontSize: '0.72rem' }} onClick={() => setUsernameOrEmail('student.ahmed')}>Student</button>
                <button type="button" className="btn-secondary" style={{ padding: '3px 8px', fontSize: '0.72rem' }} onClick={() => setUsernameOrEmail('doctor.hassan')}>Doctor</button>
                <button type="button" className="btn-secondary" style={{ padding: '3px 8px', fontSize: '0.72rem' }} onClick={() => setUsernameOrEmail('ta.omar')}>TA</button>
                <button type="button" className="btn-secondary" style={{ padding: '3px 8px', fontSize: '0.72rem' }} onClick={() => setUsernameOrEmail('admin.luxor')}>Admin</button>
              </div>
            </div>

            <button type="submit" className="btn-primary" style={{ justifyContent: 'center', marginTop: 10 }} disabled={isLoading}>
              <ShieldCheck size={18} /> {isLoading ? 'Authenticating with JWT...' : 'Sign In & Issue Access Token'}
            </button>
          </form>
        ) : (
          /* MFA Step */
          <form onSubmit={handleMfaVerify} style={{ display: 'flex', flexDirection: 'column', gap: 16, textAlign: 'center' }}>
            <Key size={40} color="var(--accent-amber)" style={{ margin: '0 auto' }} />
            <h3>Two-Factor Authentication Required</h3>
            <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)' }}>Enter 6-digit MFA passcode from your Authenticator app:</p>

            <input 
              type="text" 
              maxLength={6} 
              required
              placeholder="000000" 
              className="form-input" 
              style={{ textAlign: 'center', fontSize: '1.5rem', letterSpacing: '0.25em', fontWeight: 800 }}
              value={mfaCode}
              onChange={e => setMfaCode(e.target.value)}
            />

            <button type="submit" className="btn-primary" style={{ justifyContent: 'center' }}>
              <CheckCircle2 size={18} /> Verify MFA Code
            </button>
          </form>
        )}

      </div>
    </div>
  );
}
