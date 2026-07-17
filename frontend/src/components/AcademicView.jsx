import React, { useEffect, useState } from 'react';
import { BookOpen, UserCheck, Plus, Check, Search, Filter, ShieldCheck, Info } from 'lucide-react';
import { apiRequest } from '../services/apiClient';

export default function AcademicView({ courses, setCourses, user }) {
  const [search, setSearch] = useState('');
  const [deptFilter, setDeptFilter] = useState('All');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;
    async function loadCourses() {
      setLoading(true);
      const res = await apiRequest('/academic/courses');
      if (!cancelled && res.success) {
        setCourses(res.data || []);
      }
      if (!cancelled) setLoading(false);
    }
    loadCourses();
    return () => { cancelled = true; };
  }, [setCourses]);

  const toggleEnrollment = async (id) => {
    const course = courses.find(c => c.id === id);
    if (!course) return;

    const action = course.isEnrolled ? 'drop' : 'enroll';
    const res = await apiRequest(`/registration/enrollments/${id}`, {
      method: action === 'enroll' ? 'POST' : 'DELETE',
    });

    if (res.success) {
      setCourses(courses.map(c => {
        if (c.id === id) {
          const isEnrolled = !c.isEnrolled;
          return {
            ...c,
            isEnrolled,
            enrolledCount: isEnrolled ? c.enrolledCount + 1 : Math.max(0, c.enrolledCount - 1)
          };
        }
        return c;
      }));
    }
  };

  const departments = ['All', ...new Set(courses.map(c => c.department).filter(Boolean))];

  const filteredCourses = courses.filter(c => {
    const matchesSearch = (c.nameEn || '').toLowerCase().includes(search.toLowerCase()) || 
                         (c.courseCode || '').toLowerCase().includes(search.toLowerCase());
    const matchesDept = deptFilter === 'All' || c.department === deptFilter;
    return matchesSearch && matchesDept;
  });

  if (loading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '40vh' }}>
        <div className="glass-panel" style={{ padding: 24 }}>Loading courses...</div>
      </div>
    );
  }

  return (
    <div className="animate-fade-in" style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      
      {/* Header */}
      <div className="glass-panel" style={{ padding: 28, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 6 }}>
            <span className="badge badge-cyan">Milestone 2 & 3</span>
            <span className="badge badge-emerald">Academic Hierarchy & Enrollment</span>
          </div>
          <h2 style={{ fontSize: '1.8rem' }}>Course Offering Catalog & Registration</h2>
          <p style={{ color: 'var(--text-muted)', fontSize: '0.9rem' }}>
            Faculty of Computers & Artificial Intelligence • Departmental Offerings
          </p>
        </div>

        <div style={{ display: 'flex', gap: 12 }}>
          <div style={{ background: 'var(--bg-glass)', padding: '10px 18px', borderRadius: 12, border: '1px solid var(--border-glass)', textAlign: 'center' }}>
            <p style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>Registration Status</p>
            <p style={{ fontSize: '0.95rem', fontWeight: 700, color: 'var(--accent-emerald)' }}>OPEN (Spring 2026)</p>
          </div>
        </div>
      </div>

      {/* Filters & Search */}
      <div className="glass-panel" style={{ padding: 18, display: 'flex', gap: 16, alignItems: 'center', flexWrap: 'wrap' }}>
        <div style={{ flex: 1, minWidth: 260, display: 'flex', alignItems: 'center', gap: 10, background: 'rgba(15, 23, 42, 0.6)', padding: '8px 14px', borderRadius: 8, border: '1px solid var(--border-glass)' }}>
          <Search size={18} color="var(--text-muted)" />
          <input 
            type="text" 
            placeholder="Search by course code, title, or instructor..." 
            value={search}
            onChange={e => setSearch(e.target.value)}
            style={{ background: 'transparent', border: 'none', color: '#fff', fontSize: '0.9rem', outline: 'none', width: '100%' }}
          />
        </div>

        <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
          <Filter size={16} color="var(--text-muted)" />
          <span style={{ fontSize: '0.85rem', color: 'var(--text-muted)' }}>Department:</span>
          {departments.map(dept => (
            <button
              key={dept}
              onClick={() => setDeptFilter(dept)}
              style={{
                padding: '6px 14px',
                borderRadius: 8,
                fontSize: '0.82rem',
                fontWeight: 600,
                border: 'none',
                cursor: 'pointer',
                background: deptFilter === dept ? 'linear-gradient(135deg, var(--accent-cyan), var(--accent-blue))' : 'var(--bg-glass)',
                color: deptFilter === dept ? '#fff' : 'var(--text-muted)',
                transition: 'all 0.2s ease'
              }}
            >
              {dept}
            </button>
          ))}
        </div>
      </div>

      {/* Courses Grid */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(340px, 1fr))', gap: 20 }}>
        {filteredCourses.map(course => (
          <div key={course.id} className="glass-panel glass-panel-hover" style={{ padding: 24, display: 'flex', flexDirection: 'column', justifyContent: 'space-between' }}>
            <div>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
                <span className="badge badge-cyan" style={{ fontSize: '0.82rem' }}>{course.courseCode}</span>
                <span className="badge badge-purple">{course.creditHours} Credit Hours</span>
              </div>

              <h3 style={{ fontSize: '1.2rem', marginBottom: 6 }}>{course.nameEn}</h3>
              <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)', marginBottom: 12, fontFamily: 'sans-serif' }}>{course.nameAr}</p>

              <div style={{ display: 'flex', flexDirection: 'column', gap: 8, background: 'rgba(15, 23, 42, 0.4)', padding: 12, borderRadius: 10, marginBottom: 16 }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.82rem' }}>
                  <span style={{ color: 'var(--text-muted)' }}>Primary Instructor:</span>
                  <span style={{ fontWeight: 600 }}>{course.primaryTeacher}</span>
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.82rem' }}>
                  <span style={{ color: 'var(--text-muted)' }}>Department:</span>
                  <span>{course.department}</span>
                </div>
                <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.82rem' }}>
                  <span style={{ color: 'var(--text-muted)' }}>Capacity Limit:</span>
                  <span style={{ color: course.enrolledCount >= course.capacity ? 'var(--accent-rose)' : 'var(--accent-emerald)', fontWeight: 700 }}>
                    {course.enrolledCount} / {course.capacity} Students
                  </span>
                </div>
              </div>
            </div>

            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', pt: 12, borderTop: '1px solid var(--border-glass)' }}>
              <span className="badge badge-amber">{course.sectionType}</span>
              <button 
                onClick={() => toggleEnrollment(course.id)}
                className={course.isEnrolled ? "btn-danger" : "btn-primary"}
                style={{ padding: '8px 16px', fontSize: '0.85rem' }}
              >
                {course.isEnrolled ? (
                  <> <Check size={16} /> Enrolled (Drop) </>
                ) : (
                  <> <Plus size={16} /> Enroll Now </>
                )}
              </button>
            </div>
          </div>
        ))}
      </div>

    </div>
  );
}
