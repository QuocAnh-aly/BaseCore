import React, { useState, useEffect } from 'react';
import { gameAccountApi, categoryApi } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const GameAccounts = () => {
    const [accounts, setAccounts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    const [selectedGame, setSelectedGame] = useState('');
    const [showModal, setShowModal] = useState(false);
    const [editingAccount, setEditingAccount] = useState(null);
    const [formData, setFormData] = useState({
        gameName: '',
        accountName: '',
        password: '',
        description: '',
        price: 0,
        imageUrl: '',
    });
    const [error, setError] = useState('');
    const { isAdmin } = useAuth();

    useEffect(() => {
        loadCategories();
    }, []);

    useEffect(() => {
        loadAccounts();
    }, [selectedGame]);

    const loadCategories = async () => {
        try {
            const response = await categoryApi.getAll();
            setCategories(response.data || []);
        } catch (error) {
            console.error('Failed to load categories:', error);
        }
    };

    const loadAccounts = async () => {
        setLoading(true);
        try {
            const response = await gameAccountApi.getAll(selectedGame);
            setAccounts(response.data || []);
        } catch (error) {
            console.error('Failed to load game accounts:', error);
            setAccounts([]);
        } finally {
            setLoading(false);
        }
    };

    const openModal = (account = null) => {
        if (account) {
            setEditingAccount(account);
            setFormData({
                gameName: account.gameName || '',
                accountName: account.accountName || '',
                password: account.password || '',
                description: account.description || '',
                price: account.price || 0,
                imageUrl: account.imageUrl || '',
            });
        } else {
            setEditingAccount(null);
            setFormData({
                gameName: categories[0]?.name || '',
                accountName: '',
                password: '',
                description: '',
                price: 0,
                imageUrl: '',
            });
        }
        setError('');
        setShowModal(true);
    };

    const closeModal = () => {
        setShowModal(false);
        setEditingAccount(null);
        setError('');
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');

        try {
            const data = {
                ...formData,
                price: parseFloat(formData.price),
            };

            if (editingAccount) {
                await gameAccountApi.update(editingAccount.id, data);
            } else {
                await gameAccountApi.create(data);
            }

            closeModal();
            loadAccounts();
        } catch (error) {
            setError(error.response?.data?.message || 'Operation failed');
        }
    };

    const handleDelete = async (id) => {
        if (!window.confirm('Are you sure you want to delete this game account?')) return;

        try {
            await gameAccountApi.delete(id);
            loadAccounts();
        } catch (error) {
            alert('Failed to delete game account');
        }
    };

    return (
        <div className="content-wrapper">
            <div className="content-header">
                <div className="container-fluid">
                    <div className="row mb-2">
                        <div className="col-sm-6">
                            <h1 className="m-0">Game Accounts Management</h1>
                        </div>
                    </div>
                </div>
            </div>

            <section className="content">
                <div className="container-fluid">
                    <div className="card">
                        <div className="card-header">
                            <div className="row">
                                <div className="col-md-6">
                                    <form className="form-inline" onSubmit={(e) => e.preventDefault()}>
                                        <select
                                            className="form-control mr-2"
                                            value={selectedGame}
                                            onChange={(e) => setSelectedGame(e.target.value)}
                                        >
                                            <option value="">All Games</option>
                                            {categories.map(cat => (
                                                <option key={cat.id} value={cat.name}>{cat.name}</option>
                                            ))}
                                        </select>
                                    </form>
                                </div>
                                <div className="col-md-6 text-right">
                                    {isAdmin() && (
                                        <button className="btn btn-success" onClick={() => openModal()}>
                                            <i className="fas fa-plus"></i> Add Game Account
                                        </button>
                                    )}
                                </div>
                            </div>
                        </div>
                        <div className="card-body">
                            {loading ? (
                                <div className="text-center py-5">
                                    <div className="spinner-border text-primary"></div>
                                </div>
                            ) : (
                                <>
                                    <table className="table table-bordered table-striped" style={{tableLayout: 'fixed'}}>
                                        <thead>
                                            <tr>
                                                <th style={{width: "50px"}}>ID</th>
                                                <th>Game Name</th>
                                                <th>Account Name</th>
                                                <th>Price</th>
                                                <th>Status</th>
                                                {isAdmin() && <th style={{width: "100px"}}>Actions</th>}
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {accounts.length === 0 ? (
                                                <tr>
                                                    <td colSpan={isAdmin() ? 6 : 5} className="text-center">
                                                        No game accounts found
                                                    </td>
                                                </tr>
                                            ) : (
                                                accounts.map(acc => (
                                                    <tr key={acc.id}>
                                                        <td>{acc.id}</td>
                                                        <td style={{whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis'}}>{acc.gameName}</td>
                                                        <td style={{whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis'}}>{acc.accountName}</td>
                                                        <td>{acc.price?.toLocaleString()} VND</td>
                                                        <td>
                                                            {acc.isSold ? (
                                                                <span className="badge badge-danger">Sold</span>
                                                            ) : (
                                                                <span className="badge badge-success">Available</span>
                                                            )}
                                                        </td>
                                                        {isAdmin() && (
                                                            <td>
                                                                <button
                                                                    className="btn btn-sm btn-info mr-1"
                                                                    onClick={() => openModal(acc)}
                                                                >
                                                                    <i className="fas fa-edit"></i>
                                                                </button>
                                                                <button
                                                                    className="btn btn-sm btn-danger"
                                                                    onClick={() => handleDelete(acc.id)}
                                                                >
                                                                    <i className="fas fa-trash"></i>
                                                                </button>
                                                            </td>
                                                        )}
                                                    </tr>
                                                ))
                                            )}
                                        </tbody>
                                    </table>
                                </>
                            )}
                        </div>
                    </div>
                </div>
            </section>

            {/* Modal */}
            {showModal && (
                <div className="modal fade show" style={{ display: 'block', overflow: 'auto' }} tabIndex="-1">
                    <div className="modal-dialog">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">
                                    {editingAccount ? 'Edit Game Account' : 'Add Game Account'}
                                </h5>
                                <button type="button" className="close" onClick={closeModal}>
                                    <span>&times;</span>
                                </button>
                            </div>
                            <form onSubmit={handleSubmit}>
                                <div className="modal-body">
                                    {error && <div className="alert alert-danger">{error}</div>}
                                    <div className="form-group">
                                        <label>Game Category</label>
                                        <select
                                            className="form-control"
                                            value={formData.gameName}
                                            onChange={(e) => setFormData({ ...formData, gameName: e.target.value })}
                                            required
                                        >
                                            <option value="">Select Game</option>
                                            {categories.map(cat => (
                                                <option key={cat.id} value={cat.name}>{cat.name}</option>
                                            ))}
                                        </select>
                                    </div>
                                    <div className="form-group">
                                        <label>Account Name / Title</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            value={formData.accountName}
                                            onChange={(e) => setFormData({ ...formData, accountName: e.target.value })}
                                            required
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Password</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            value={formData.password}
                                            onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                                            required={!editingAccount}
                                            placeholder={editingAccount ? "Leave blank to keep old password" : "Enter password"}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Price (VND)</label>
                                        <input
                                            type="number"
                                            className="form-control"
                                            value={formData.price}
                                            onChange={(e) => setFormData({ ...formData, price: e.target.value })}
                                            required
                                            min="0"
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Image URL</label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            value={formData.imageUrl}
                                            onChange={(e) => setFormData({ ...formData, imageUrl: e.target.value })}
                                        />
                                    </div>
                                    <div className="form-group">
                                        <label>Description</label>
                                        <textarea
                                            className="form-control"
                                            value={formData.description}
                                            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                                            rows="3"
                                        />
                                    </div>
                                </div>
                                <div className="modal-footer">
                                    <button type="button" className="btn btn-secondary" onClick={closeModal}>
                                        Cancel
                                    </button>
                                    <button type="submit" className="btn btn-primary">
                                        {editingAccount ? 'Update' : 'Create'}
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            )}
            {showModal && <div className="modal-backdrop fade show"></div>}
        </div>
    );
};

export default GameAccounts;
