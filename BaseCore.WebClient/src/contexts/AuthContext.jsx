import React, { createContext, useContext, useState, useEffect } from 'react';
import { authApi } from '../services/api';

const AuthContext = createContext(null);

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

   useEffect(() => {
    const storedUser = localStorage.getItem('user');
    const token = localStorage.getItem('token');

    if (storedUser && token) {
        setUser(JSON.parse(storedUser));
    }

    setLoading(false);
}, []);

    const login = async (username, password) => {
    try {
        const response = await authApi.login(username, password);

        const data = response.data;

        const token = data.token;
        const user = data.user;

        localStorage.setItem('token', token);
        localStorage.setItem('user', JSON.stringify(user));

        setUser(user);

        return { success: true };
    } catch (error) {
        const message = error.response?.data?.message || 'Login failed';
        return { success: false, message };
    }
};

    const isAdmin = () => {
        return user?.role === 'Admin';
    };
    const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    setUser(null);
};
    const value = {
        user,
        login,
        logout,
        isAdmin,
        isAuthenticated: !!user,
        loading,
    };

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
};


export default AuthContext;
