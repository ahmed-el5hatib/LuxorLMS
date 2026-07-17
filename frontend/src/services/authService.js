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

    return { success: false, error: response.error, description: response.description };
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
      return response.data.permissions || [];
    }
    return [];
  }
};
