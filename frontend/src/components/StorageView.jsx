import React, { useEffect, useState } from 'react';
import { FolderLock, UploadCloud, Link, History, Shield, FileText, Download, Trash2, X, CheckCircle2 } from 'lucide-react';
import { apiRequest } from '../services/apiClient';

export default function StorageView({ files, setFiles, user }) {
  const [selectedFileForUrl, setSelectedFileForUrl] = useState(null);
  const [generatedSignedUrl, setGeneratedSignedUrl] = useState(null);
  const [selectedFileForVersions, setSelectedFileForVersions] = useState(null);

  const [uploadFileName, setUploadFileName] = useState('');
  const [uploadProvider, setUploadProvider] = useState('S3');
  const [isUploading, setIsUploading] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;
    async function loadFiles() {
      setLoading(true);
      const res = await apiRequest('/storage/files');
      if (!cancelled && res.success) {
        setFiles(res.data || []);
      }
      if (!cancelled) setLoading(false);
    }
    loadFiles();
    return () => { cancelled = true; };
  }, [setFiles]);

  const handleUpload = async (e) => {
    e.preventDefault();
    if (!uploadFileName) return;

    setIsUploading(true);
    const res = await apiRequest('/storage/files', {
      method: 'POST',
      body: JSON.stringify({ fileName: uploadFileName, provider: uploadProvider }),
    });

    if (res.success) {
      setFiles([res.data, ...files]);
      setUploadFileName('');
    }
    setIsUploading(false);
  };

  const generateSignedUrl = async (file) => {
    setSelectedFileForUrl(file);
    const res = await apiRequest(`/storage/files/${file.id}/url`);
    if (res.success) {
      setGeneratedSignedUrl(res.data?.url || '');
    }
  };

  const deleteFile = async (id) => {
    const res = await apiRequest(`/storage/files/${id}`, { method: 'DELETE' });
    if (res.success) {
      setFiles(files.filter(f => f.id !== id));
    }
  };

  if (loading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '40vh' }}>
        <div className="glass-panel" style={{ padding: 24 }}>Loading storage...</div>
      </div>
    );
  }

  return (
    <div className="animate-fade-in" style={{ display: 'flex', flexDirection: 'column', gap: 24 }}>
      
      {/* Header */}
      <div className="glass-panel" style={{ padding: 28, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <div style={{ display: 'flex', alignItems: 'center', gap: 10, marginBottom: 6 }}>
            <span className="badge badge-cyan">Milestone 6.1</span>
            <span className="badge badge-emerald">Cloud-Agnostic Storage & Versioning</span>
          </div>
          <h2 style={{ fontSize: '1.8rem' }}>Object Storage Manager & Expiring Signed URLs</h2>
          <p style={{ color: 'var(--text-muted)', fontSize: '0.9rem' }}>
            Abstract provider integration for AWS S3, Azure Blob Storage, MinIO & Local Disk
          </p>
        </div>

        <div style={{ display: 'flex', gap: 10 }}>
          <span className="badge badge-cyan" style={{ padding: '8px 14px' }}>AWS S3</span>
          <span className="badge badge-purple" style={{ padding: '8px 14px' }}>Azure Blob</span>
          <span className="badge badge-amber" style={{ padding: '8px 14px' }}>MinIO</span>
        </div>
      </div>

      {/* Upload Box */}
      <div className="glass-panel" style={{ padding: 24 }}>
        <h3 style={{ fontSize: '1.15rem', marginBottom: 14 }}>Upload New Storage Artifact</h3>
        <form onSubmit={handleUpload} style={{ display: 'flex', gap: 14, alignItems: 'center', flexWrap: 'wrap' }}>
          <input 
            type="text" 
            required
            placeholder="File name (e.g. CS304_Assignment_Final.pdf)..." 
            className="form-input" 
            style={{ flex: 1, minWidth: 280 }}
            value={uploadFileName}
            onChange={e => setUploadFileName(e.target.value)}
          />

          <select 
            className="form-input" 
            style={{ width: 160 }}
            value={uploadProvider}
            onChange={e => setUploadProvider(e.target.value)}
          >
            <option value="S3">AWS S3</option>
            <option value="AzureBlob">Azure Blob</option>
            <option value="MinIO">MinIO Server</option>
            <option value="LocalStorage">Local Disk</option>
          </select>

          <button type="submit" className="btn-primary" disabled={isUploading}>
            <UploadCloud size={18} /> {isUploading ? 'Uploading Blob...' : 'Upload & Compute Hash'}
          </button>
        </form>
      </div>

      {/* Stored Files Table */}
      <div className="glass-panel" style={{ padding: 24 }}>
        <h3 style={{ fontSize: '1.2rem', marginBottom: 16 }}>Stored Files Catalog</h3>
        
        <div style={{ overflowX: 'auto' }}>
          <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
            <thead>
              <tr style={{ borderBottom: '1px solid var(--border-glass)', color: 'var(--text-muted)', fontSize: '0.82rem' }}>
                <th style={{ padding: 12 }}>FILE NAME</th>
                <th style={{ padding: 12 }}>PROVIDER</th>
                <th style={{ padding: 12 }}>CONTAINER</th>
                <th style={{ padding: 12 }}>SIZE</th>
                <th style={{ padding: 12 }}>VERSION</th>
                <th style={{ padding: 12 }}>CREATED</th>
                <th style={{ padding: 12 }}>ACTIONS</th>
              </tr>
            </thead>
            <tbody>
              {files.map(f => (
                <tr key={f.id} style={{ borderBottom: '1px solid var(--border-glass)', fontSize: '0.9rem' }}>
                  <td style={{ padding: 14, fontWeight: 600, display: 'flex', alignItems: 'center', gap: 8 }}>
                    <FileText size={18} color="var(--accent-cyan)" /> {f.fileName}
                  </td>
                  <td style={{ padding: 14 }}>
                    <span className={`badge ${f.provider === 'S3' ? 'badge-cyan' : f.provider === 'MinIO' ? 'badge-amber' : 'badge-purple'}`}>
                      {f.provider}
                    </span>
                  </td>
                  <td style={{ padding: 14, fontFamily: 'monospace', fontSize: '0.82rem', color: 'var(--text-muted)' }}>{f.container}</td>
                  <td style={{ padding: 14 }}>{(f.sizeBytes / 1000000).toFixed(2)} MB</td>
                  <td style={{ padding: 14 }}><span className="badge badge-emerald">v{f.version}</span></td>
                  <td style={{ padding: 14, fontSize: '0.82rem', color: 'var(--text-muted)' }}>{new Date(f.createdAt).toLocaleDateString()}</td>
                  <td style={{ padding: 14 }}>
                    <div style={{ display: 'flex', gap: 8 }}>
                      <button className="btn-secondary" style={{ padding: '4px 8px', fontSize: '0.75rem' }} onClick={() => generateSignedUrl(f)}>
                        <Link size={12} color="var(--accent-cyan)" /> Signed URL
                      </button>
                      <button className="btn-secondary" style={{ padding: '4px 8px', fontSize: '0.75rem' }} onClick={() => setSelectedFileForVersions(f)}>
                        <History size={12} color="var(--accent-purple)" /> History
                      </button>
                      <button className="btn-danger" style={{ padding: '4px 8px', fontSize: '0.75rem' }} onClick={() => deleteFile(f.id)}>
                        <Trash2 size={12} />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Expiring Signed URL Modal */}
      {selectedFileForUrl && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
          <div className="glass-panel" style={{ padding: 28, width: 540, maxWidth: '90%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
              <h3 style={{ fontSize: '1.2rem' }}>Expiring Signed Download URL</h3>
              <X size={20} style={{ cursor: 'pointer' }} onClick={() => setSelectedFileForUrl(null)} />
            </div>

            <p style={{ fontSize: '0.85rem', color: 'var(--text-muted)', marginBottom: 14 }}>
              Pre-signed URL generated for <strong>{selectedFileForUrl.fileName}</strong>. Valid for <strong>15 minutes</strong>.
            </p>

            <input 
              type="text" 
              readOnly 
              className="form-input" 
              value={generatedSignedUrl}
              style={{ fontSize: '0.82rem', fontFamily: 'monospace', marginBottom: 20 }}
            />

            <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 10 }}>
              <button className="btn-secondary" onClick={() => setSelectedFileForUrl(null)}>Close</button>
              <button className="btn-primary" onClick={() => { navigator.clipboard.writeText(generatedSignedUrl); alert("Signed URL copied to clipboard!"); }}>
                Copy Signed Link
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Version History Modal */}
      {selectedFileForVersions && (
        <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.75)', backdropFilter: 'blur(8px)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
          <div className="glass-panel" style={{ padding: 28, width: 520, maxWidth: '90%' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 }}>
              <h3 style={{ fontSize: '1.2rem' }}>Version History for {selectedFileForVersions.fileName}</h3>
              <X size={20} style={{ cursor: 'pointer' }} onClick={() => setSelectedFileForVersions(null)} />
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: 12, marginBottom: 20 }}>
              {selectedFileForVersions.versions?.map(v => (
                <div key={v.version} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: 12, background: 'rgba(15, 23, 42, 0.6)', borderRadius: 10, border: '1px solid var(--border-glass)' }}>
                  <div>
                    <span className="badge badge-emerald">Version v{v.version}</span>
                    <p style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginTop: 4 }}>Key: {v.objectKey}</p>
                  </div>
                  <span style={{ fontSize: '0.78rem', color: 'var(--text-dim)' }}>{new Date(v.createdAt).toLocaleDateString()}</span>
                </div>
              ))}
            </div>

            <button className="btn-secondary" style={{ width: '100%', justifyContent: 'center' }} onClick={() => setSelectedFileForVersions(null)}>
              Close Version Timeline
            </button>
          </div>
        </div>
      )}

    </div>
  );
}
