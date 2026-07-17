import React, { useState } from 'react';
import { BellRing, Mail, Smartphone, Send, ShieldCheck, CheckCircle2, RefreshCw } from 'lucide-react';

export default function NotificationsView({ notifications, setNotifications, user }) {
  const [recipient, setRecipient] = useState(user.email);
  const [channel, setChannel] = useState('InApp');
  const [title, setTitle] = useState('');
  const [body, setBody] = useState('');
  const [isSending, setIsSending] = useState(false);

  const [preferences, setPreferences] = useState({
    InApp: true,
    Email: true,
    Sms: true,
    Push: false
  });

  const togglePref = (ch) => {
    setPreferences({ ...preferences, [ch]: !preferences[ch] });
  };

  const handleSendNotification = (e) => {
    e.preventDefault();
    if (!title || !body) return;

    setIsSending(true);
    setTimeout(() => {
      const newNotif = {
        id: `n-${Date.now()}`,
        title,
        body,
        channel,
        status: 'Sent',
        createdAt: 'Just now'
      };

      setNotifications([newNotif, ...notifications]);
      setTitle('');
      setBody('');
      setIsSending(false);
    }, 1000);
  };

  return (
    <div className="animate-fade-in" style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      
      {/* Header */}
      <div className="glass-panel" style={{ padding: 28, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 6 }}>
            <span className="badge badge-purple">Milestone 6.2</span>
            <span className="badge badge-cyan">Hangfire Multi-Channel Dispatcher</span>
          </div>
          <h2 style={{ fontSize: '1.8rem' }}>Multi-Channel Notifications & Preferences</h2>
          <p style={{ color: 'var(--text-muted)', fontSize: '0.9rem' }}>
            In-App, SMTP Email, Twilio SMS & Firebase Mobile Push with Push $\rightarrow$ Email fallback
          </p>
        </div>

        <div style={{ display: 'flex', gap: 8 }}>
          <span className="badge badge-emerald"><CheckCircle2 size={12} /> Hangfire Worker Active</span>
        </div>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 24 }}>
        
        {/* Send Test Notification Form */}
        <div className="glass-panel" style={{ padding: 24 }}>
          <h3 style={{ fontSize: '1.2rem', marginBottom: 16 }}>Dispatch New Notification</h3>

          <form onSubmit={handleSendNotification} style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
            <div>
              <label style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>Target Channel:</label>
              <select className="form-input" value={channel} onChange={e => setChannel(e.target.value)} style={{ marginTop: 4 }}>
                <option value="InApp">In-App Notification</option>
                <option value="Email">Email (SMTP / SendGrid)</option>
                <option value="Sms">SMS (Twilio API)</option>
                <option value="Push">Mobile Push (Firebase)</option>
              </select>
            </div>

            <div>
              <label style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>Notification Title:</label>
              <input 
                type="text" 
                required 
                placeholder="e.g. Attendance Warning Alert" 
                className="form-input" 
                value={title} 
                onChange={e => setTitle(e.target.value)}
                style={{ marginTop: 4 }}
              />
            </div>

            <div>
              <label style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>Notification Message Body:</label>
              <textarea 
                rows={3} 
                required 
                placeholder="Enter detailed message text..." 
                className="form-input" 
                value={body} 
                onChange={e => setBody(e.target.value)}
                style={{ marginTop: 4 }}
              />
            </div>

            <button type="submit" className="btn-primary" style={{ justifyContent: 'center' }} disabled={isSending}>
              <Send size={16} /> {isSending ? 'Enqueueing in Hangfire...' : 'Enqueue Dispatch Job'}
            </button>
          </form>
        </div>

        {/* Channel Preferences Matrix */}
        <div className="glass-panel" style={{ padding: 24 }}>
          <h3 style={{ fontSize: '1.2rem', marginBottom: 16 }}>Channel Notification Preferences</h3>

          <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
            {[
              { key: 'InApp', name: 'In-App Bell Alerts', desc: 'Real-time UI toasts and popover badges' },
              { key: 'Email', name: 'Email Messages', desc: 'Direct emails to user mailbox' },
              { key: 'Sms', name: 'SMS Text Warnings', desc: 'Twilio SMS for urgent attendance alerts' },
              { key: 'Push', name: 'Mobile Push Notifications', desc: 'Mobile push with automatic Email fallback' }
            ].map(item => (
              <div key={item.key} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: 14, background: 'rgba(15, 23, 42, 0.5)', borderRadius: 10, border: '1px solid var(--border-glass)' }}>
                <div>
                  <h4 style={{ fontSize: '0.95rem' }}>{item.name}</h4>
                  <p style={{ fontSize: '0.78rem', color: 'var(--text-muted)' }}>{item.desc}</p>
                </div>
                <input 
                  type="checkbox" 
                  checked={preferences[item.key]} 
                  onChange={() => togglePref(item.key)} 
                  style={{ width: 20, height: 20, cursor: 'pointer', accentColor: 'var(--accent-cyan)' }}
                />
              </div>
            ))}
          </div>
        </div>

      </div>

      {/* Notifications History */}
      <div className="glass-panel" style={{ padding: 24 }}>
        <h3 style={{ fontSize: '1.2rem', marginBottom: 16 }}>Recent Dispatch History</h3>
        
        <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
          {notifications.map(n => (
            <div key={n.id} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: 14, background: 'rgba(15, 23, 42, 0.4)', borderRadius: 10, border: '1px solid var(--border-glass)' }}>
              <div style={{ display: 'flex', gap: 14, alignItems: 'center' }}>
                <div style={{ padding: 10, borderRadius: 10, background: 'rgba(56, 189, 248, 0.15)', color: 'var(--accent-cyan)' }}>
                  <BellRing size={20} />
                </div>
                <div>
                  <h4 style={{ fontSize: '0.98rem' }}>{n.title}</h4>
                  <p style={{ fontSize: '0.82rem', color: 'var(--text-muted)', marginTop: 2 }}>{n.body}</p>
                </div>
              </div>

              <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                <span className="badge badge-cyan">{n.channel}</span>
                <span className="badge badge-emerald">{n.status}</span>
                <span style={{ fontSize: '0.75rem', color: 'var(--text-dim)' }}>{n.createdAt}</span>
              </div>
            </div>
          ))}
        </div>
      </div>

    </div>
  );
}
