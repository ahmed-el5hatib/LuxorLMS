// LuxorLMS Real API Client & Middleware

const API_BASE_URL = 'http://localhost:5000/api/v1';

export async function apiRequest(endpoint, options = {}) {
  const token = localStorage.getItem('luxorlms_access_token');

  const headers = {
    'Content-Type': 'application/json',
    ...(token ? { 'Authorization': `Bearer ${token}` } : {}),
    ...options.headers,
  };

  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      ...options,
      headers,
    });

    if (response.status === 401) {
      // Token expired or invalid
      localStorage.removeItem('luxorlms_access_token');
      localStorage.removeItem('luxorlms_user');
      window.dispatchEvent(new Event('luxorlms_auth_changed'));
    }

    const data = await response.json().catch(() => null);

    if (!response.ok) {
      return {
        success: false,
        status: response.status,
        error: data?.error || data?.title || 'API_ERROR',
        description: data?.description || data?.detail || 'An error occurred during request execution.',
      };
    }

    return {
      success: true,
      status: response.status,
      data,
    };
  } catch (err) {
    return {
      success: false,
      status: 0,
      error: 'NETWORK_DISCONNECTED',
      description: 'Unable to connect to LuxorLMS ASP.NET Core API server at http://localhost:5000.',
    };
  }
}
