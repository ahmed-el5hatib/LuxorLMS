import React, { useState } from 'react';
import { MessagesSquare, Pin, Lock, Shield, User, Send, Flag, Trash2, Plus, CornerDownRight, CheckCircle2 } from 'lucide-react';

export default function ForumsView({ forums, setForums, user }) {
  const [selectedCourseFilter, setSelectedCourseFilter] = useState('All');
  const [activeTopic, setActiveTopic] = useState(null);
  const [newTopicTitle, setNewTopicTitle] = useState('');
  const [newTopicContent, setNewTopicContent] = useState('');
  const [newTopicCourse, setNewTopicCourse] = useState('CS304');
  const [showCreateModal, setShowCreateModal] = useState(false);

  const [replyText, setReplyText] = useState('');

  const createTopic = (e) => {
    e.preventDefault();
    if (!newTopicTitle || !newTopicContent) return;

    const topic = {
      id: `topic-${Date.now()}`,
      courseOfferingId: 'offering-101',
      courseCode: newTopicCourse,
      title: newTopicTitle,
      authorName: user.fullName,
      isDoctor: user.role === 'Doctor',
      isPinned: false,
      isLocked: false,
      createdAt: new Date().toISOString(),
      repliesCount: 1,
      posts: [
        {
          id: `post-${Date.now()}`,
          authorName: user.fullName,
          isDoctor: user.role === 'Doctor',
          body: newTopicContent,
          createdAt: new Date().toISOString(),
          moderationStatus: 'None'
        }
      ]
    };

    setForums([topic, ...forums]);
    setNewTopicTitle('');
    setNewTopicContent('');
    setShowCreateModal(false);
  };

  const addReply = (topicId) => {
    if (!replyText) return;

    const newPost = {
      id: `post-${Date.now()}`,
      authorName: user.fullName,
      isDoctor: user.role === 'Doctor' || user.role === 'TA',
      body: replyText,
      createdAt: new Date().toISOString(),
      moderationStatus: 'None'
    };

    setForums(forums.map(t => {
      if (t.id === topicId) {
        return {
          ...t,
          repliesCount: t.repliesCount + 1,
          posts: [...t.posts, newPost]
        };
      }
      return t;
    }));

    if (activeTopic && activeTopic.id === topicId) {
      setActiveTopic({
        ...activeTopic,
        repliesCount: activeTopic.repliesCount + 1,
        posts: [...activeTopic.posts, newPost]
      });
    }

    setReplyText('');
  };

  const togglePin = (topicId) => {
    setForums(forums.map(t => t.id === topicId ? { ...t, isPinned: !t.isPinned } : t));
    if (activeTopic?.id === topicId) setActiveTopic({ ...activeTopic, isPinned: !activeTopic.isPinned });
  };

  const toggleLock = (topicId) => {
    setForums(forums.map(t => t.id === topicId ? { ...t, isLocked: !t.isLocked } : t));
    if (activeTopic?.id === topicId) setActiveTopic({ ...activeTopic, isLocked: !activeTopic.isLocked });
  };

  const moderatePost = (topicId, postId) => {
    setForums(forums.map(t => {
      if (t.id === topicId) {
        return {
          ...t,
          posts: t.posts.map(p => p.id === postId ? { ...p, moderationStatus: 'Removed', body: '[This post has been removed by moderator]' } : p)
        };
      }
      return t;
    }));

    if (activeTopic?.id === topicId) {
      setActiveTopic({
        ...activeTopic,
        posts: activeTopic.posts.map(p => p.id === postId ? { ...p, moderationStatus: 'Removed', body: '[This post has been removed by moderator]' } : p)
      });
    }
  };

  const filteredTopics = forums.filter(t => selectedCourseFilter === 'All' || t.courseCode === selectedCourseFilter);

  return (
    <div className="animate-fade-in" style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      
      {/* Header */}
      <div className="glass-panel" style={{ padding: 28, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 6 }}>
            <span className="badge badge-amber">Milestone 6.3</span>
            <span className="badge badge-cyan">Course-Scoped Discussion Boards</span>
          </div>
          <h2 style={{ fontSize: '1.8rem' }}>Course Forums & Threaded Discussion Board</h2>
          <p style={{ color: 'var(--text-muted)', fontSize: '0.9rem' }}>
            Doctor & TA Moderation Rights • Student Social Participation
          </p>
        </div>

        <button className="btn-primary" onClick={() => setShowCreateModal(true)}>
          <Plus size={18} /> Start New Topic
        </button>
      </div>

      {/* Main Forum View */}
      {!activeTopic ? (
        <div style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>
          
          {/* Course Filter */}
          <div style={{ display: 'flex', gap: 10 }}>
            {['All', 'CS304', 'CS401', 'SE205'].map(c => (
              <button
                key={c}
                onClick={() => setSelectedCourseFilter(c)}
                className={`btn-secondary ${selectedCourseFilter === c ? 'badge-cyan' : ''}`}
                style={{ padding: '6px 14px', fontSize: '0.85rem' }}
              >
                {c === 'All' ? 'All Course Forums' : `Course ${c}`}
              </button>
            ))}
          </div>

          {/* Topics List */}
          <div className="glass-panel" style={{ padding: 24 }}>
            <h3 style={{ fontSize: '1.2rem', marginBottom: 16 }}>Discussion Threads</h3>
            
            <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
              {filteredTopics.map(topic => (
                <div 
                  key={topic.id} 
                  className="glass-panel glass-panel-hover" 
                  style={{ padding: 20, display: 'flex', justifyContent: 'space-between', alignItems: 'center', cursor: 'pointer', background: 'rgba(15, 23, 42, 0.5)' }}
                  onClick={() => setActiveTopic(topic)}
                >
                  <div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 6 }}>
                      <span className="badge badge-cyan">{topic.courseCode}</span>
                      {topic.isPinned && <span className="badge badge-amber"><Pin size={12} /> Pinned Thread</span>}
                      {topic.isLocked && <span className="badge badge-purple"><Lock size={12} /> Locked</span>}
                    </div>

                    <h4 style={{ fontSize: '1.1rem', marginBottom: 4 }}>{topic.title}</h4>
                    <p style={{ fontSize: '0.8rem', color: 'var(--text-muted)' }}>
                      Author: <strong>{topic.authorName}</strong> {topic.isDoctor && <span style={{ color: 'var(--accent-amber)', fontWeight: 700 }}>(Doctor)</span>} • {new Date(topic.createdAt).toLocaleDateString()}
                    </p>
                  </div>

                  <div style={{ display: 'flex', alignItems: 'center', gap: 14 }}>
                    <div style={{ textAlign: 'right' }}>
                      <span style={{ fontSize: '1.1rem', fontWeight: 800, color: 'var(--accent-cyan)' }}>{topic.repliesCount}</span>
                      <p style={{ fontSize: '0.72rem', color: 'var(--text-muted)' }}>Replies</p>
                    </div>

                    {(user.role === 'Doctor' || user.role === 'TA' || user.role === 'Admin') && (
                      <div style={{ display: 'flex', gap: 6 }} onClick={e => e.stopPropagation()}>
                        <button className="btn-secondary" style={{ padding: 6 }} onClick={() => togglePin(topic.id)} title="Toggle Pin">
                          <Pin size={14} color={topic.isPinned ? 'var(--accent-amber)' : 'var(--text-muted)'} />
                        </button>
                        <button className="btn-secondary" style={{ padding: 6 }} onClick={() => toggleLock(topic.id)} title="Toggle Lock">
                          <Lock size={14} color={topic.isLocked ? 'var(--accent-purple)' : 'var(--text-muted)'} />
                        </button>
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>

        </div>
      ) : (
        /* Active Topic Discussion Workspace */
        <div className="glass-panel" style={{ padding: 32 }}>
          
          <button className="btn-secondary" style={{ marginBottom: 20 }} onClick={() => setActiveTopic(null)}>
            ← Back to Threads
          </button>

          <div style={{ pb: 16, borderBottom: '1px solid var(--border-glass)', marginBottom: 24 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 8 }}>
              <span className="badge badge-cyan">{activeTopic.courseCode}</span>
              {activeTopic.isPinned && <span className="badge badge-amber"><Pin size={12} /> Pinned</span>}
              {activeTopic.isLocked && <span className="badge badge-purple"><Lock size={12} /> Locked</span>}
            </div>
            <h2 style={{ fontSize: '1.6rem' }}>{activeTopic.title}</h2>
          </div>

          {/* Posts List */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: 18, marginBottom: 28 }}>
            {activeTopic.posts.map((post, idx) => (
              <div 
                key={post.id} 
                style={{ 
                  padding: 20, 
                  background: post.isDoctor ? 'rgba(245, 158, 11, 0.05)' : 'rgba(15, 23, 42, 0.6)', 
                  borderRadius: 14, 
                  border: post.isDoctor ? '1px solid rgba(245, 158, 11, 0.3)' : '1px solid var(--border-glass)',
                  marginLeft: idx === 0 ? 0 : 24
                }}
              >
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 10 }}>
                  <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                    {idx > 0 && <CornerDownRight size={16} color="var(--text-muted)" />}
                    <User size={18} color={post.isDoctor ? 'var(--accent-amber)' : 'var(--accent-cyan)'} />
                    <span style={{ fontWeight: 700, fontSize: '0.92rem' }}>{post.authorName}</span>
                    {post.isDoctor && <span className="badge badge-amber" style={{ fontSize: '0.65rem' }}>Instructor / TA</span>}
                  </div>

                  <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                    <span style={{ fontSize: '0.75rem', color: 'var(--text-dim)' }}>{new Date(post.createdAt).toLocaleTimeString()}</span>
                    
                    {(user.role === 'Doctor' || user.role === 'TA' || user.role === 'Admin') && post.moderationStatus !== 'Removed' && (
                      <button className="btn-danger" style={{ padding: '2px 8px', fontSize: '0.7rem' }} onClick={() => moderatePost(activeTopic.id, post.id)}>
                        <Flag size={12} /> Moderate
                      </button>
                    )}
                  </div>
                </div>

                <p style={{ fontSize: '0.95rem', color: post.moderationStatus === 'Removed' ? 'var(--accent-rose)' : 'var(--text-main)', fontStyle: post.moderationStatus === 'Removed' ? 'italic' : 'normal' }}>
                  {post.body}
                </p>
              </div>
            ))}
          </div>

          {/* Reply Form */}
          {!activeTopic.isLocked ? (
            <div style={{ display: 'flex', gap: 12 }}>
              <input 
                type="text" 
                placeholder="Write a reply to this thread..." 
                className="form-input" 
                value={replyText} 
                onChange={e => setReplyText(e.target.value)}
              />
              <button className="btn-primary" onClick={() => addReply(activeTopic.id)}>
                <Send size={16} /> Reply
              </button>
            </div>
          ) : (
            <div style={{ background: 'rgba(168, 85, 247, 0.15)', padding: 12, borderRadius: 10, textAlign: 'center', color: 'var(--accent-purple)', fontWeight: 600 }}>
              This topic has been locked by Doctor/TA. Further replies are disabled.
            </div>
          )}

        </div>
      )}

      {/* Start Topic Modal */}
      {showCreateModal && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
          <div className="glass-panel" style={{ padding: 28, width: 520, maxWidth: '90%' }}>
            <h3 style={{ fontSize: '1.25rem', marginBottom: 16 }}>Start New Forum Discussion Thread</h3>

            <form onSubmit={createTopic} style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
              <div>
                <label style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>Target Course Offering:</label>
                <select className="form-input" value={newTopicCourse} onChange={e => setNewTopicCourse(e.target.value)} style={{ marginTop: 4 }}>
                  <option value="CS304">CS304 — Distributed Systems</option>
                  <option value="CS401">CS401 — Machine Learning</option>
                  <option value="SE205">SE205 — Software Engineering</option>
                </select>
              </div>

              <div>
                <label style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>Thread Title:</label>
                <input 
                  type="text" 
                  required 
                  placeholder="e.g. Question regarding Raft consensus leader election..." 
                  className="form-input" 
                  value={newTopicTitle} 
                  onChange={e => setNewTopicTitle(e.target.value)}
                  style={{ marginTop: 4 }}
                />
              </div>

              <div>
                <label style={{ fontSize: '0.82rem', color: 'var(--text-muted)' }}>Initial Post Description:</label>
                <textarea 
                  rows={4} 
                  required 
                  placeholder="Provide context for your discussion topic..." 
                  className="form-input" 
                  value={newTopicContent} 
                  onChange={e => setNewTopicContent(e.target.value)}
                  style={{ marginTop: 4 }}
                />
              </div>

              <div style={{ display: 'flex', gap: 10, justifyContent: 'flex-end', marginTop: 10 }}>
                <button type="button" className="btn-secondary" onClick={() => setShowCreateModal(false)}>Cancel</button>
                <button type="submit" className="btn-primary">Publish Discussion Thread</button>
              </div>
            </form>
          </div>
        </div>
      )}

    </div>
  );
}
