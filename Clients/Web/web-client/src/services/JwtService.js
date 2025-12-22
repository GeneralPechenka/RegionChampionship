// src/services/JwtService.js
export default class JwtService {
  static tokenKey = 'jwt_token';
  static userDataKey = 'user_data';

  // Сохранить токен и данные
  static setToken(token, data) {
    if (token) {
      localStorage.setItem(JwtService.tokenKey, token);
    }
    if (data) {
      localStorage.setItem(JwtService.userDataKey, JSON.stringify(data));
    }
  }

  // Получить токен
  static getToken() {
    return localStorage.getItem(JwtService.tokenKey);
  }

  // Получить информацию о пользователе
  static getUserData() {
    const data = localStorage.getItem(JwtService.userDataKey);
    return data ? JSON.parse(data) : null;
  }

  // Удалить токен
  static removeToken() {
    localStorage.removeItem(JwtService.tokenKey);
    localStorage.removeItem(JwtService.userDataKey);
  }

  // Проверить авторизацию
  static isAuthenticated() {
    return !!JwtService.getToken();
  }
}