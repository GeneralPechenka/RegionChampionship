// src/services/AuthService.js
import axios from 'axios';

export default class AuthService {
  constructor() {
    this.baseUrl = 'https://localhost:7000/api/Auth';
  }

  // Вход
  static async login(email, password) {
    const response = await axios.post(`https://localhost:7000/Auth/login`, {
      email,
      passwordHash: password
    });
    console.log(response);
    return response.data;
  }

  // Выход
  static async logout() {
    await axios.post(`${this.baseUrl}/logout`);
  }

  // Регистрация
  static async register(email, password, fullname, role, companyId) {
    const response = await axios.post(`${this.baseUrl}/register`, {
      email,
      passwordHash: password,
      fullname,
      role,
      companyId
    });
    return response.data;
  }

  // Обновить токен
  static async refreshToken() {
    const response = await axios.post(`${this.baseUrl}/refresh`);
    return response.data;
  }
}