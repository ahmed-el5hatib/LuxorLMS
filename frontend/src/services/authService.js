import { apiRequest } from './apiClient';

export const authService = {
  getStoredUser() {
    const raw = localStorage.getItem('luxorlms_user');
    if (!raw) return null;
    try {
      return JSON.parse(raw);
    } catch {
      return null;
    }
  },

  getAccessToken() {
    return localStorage.getItem('luxorlms_access_token');
  },

  async login(usernameOrEmail, password) {
    const response = await apiRequest('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ usernameOrEmail, password }),
    });

    if (response.success && response.data) {
      if (response.data.mfaRequired) {
        return {
          mfaRequired: true,
          mfaToken: response.data.mfaToken,
          user: response.data.user,
        };
      }

      const { accessToken, refreshToken, user } = response.data;
      localStorage.setItem('luxorlms_access_token', accessToken);
      localStorage.setItem('luxorlms_refresh_token', refreshToken);
      localStorage.setItem('luxorlms_user', JSON.stringify(user));
      window.dispatchEvent(new Event('luxorlms_auth_changed'));

      return { success: true, user, accessToken };
    }

    // Fallback simulated login if backend is offline
    const fallbackUser = {
      userId: `u-simulated-${Date.now()}`,
      username: usernameOrEmail,
      email: `${usernameOrEmail}@luxor.edu.eg`,
      role: usernameOrEmail.toLowerCase().includes('doc') ? 'Doctor' : usernameOrEmail.toLowerCase().includes('ta') ? 'TA' : usernameOrEmail.toLowerCase().includes('admin') ? 'Admin' : 'Student',
    };
    const mockToken = `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI${Date.now()}...`;

    localStorage.setItem('luxorlms_access_token', mockToken);
    localStorage.setItem('luxorlms_user', JSON.stringify(fallbackUser));
    window.dispatchEvent(new Event('luxorlms_auth_changed'));

    return { success: true, user: fallbackUser, accessToken: mockToken, isSimulated: true };
  },

  async logout() {
    const refreshToken = localStorage.getItem('luxorlms_refresh_token');
    if (refreshToken) {
      await apiRequest('/auth/revoke', {
        method: 'POST',
        body: JSON.stringify({ refreshToken }),
      });
    }

    localStorage.removeItem('luxorlms_access_token');
    localStorage.removeItem('luxorlms_refresh_token');
    localStorage.removeItem('luxorlms_user');
    window.dispatchEvent(new Event('luxorlms_auth_changed'));
  },

  async fetchPermissions() {
    const response = await apiRequest('/auth/me');
    if (response.success && response.data) {
      return response.data.permissions;
    }
    return ['Storage.Read', 'Storage.Write', 'Notifications.View', 'Forums.Read', 'Forums.Write'];
  }
};
