'use client';
import { useEffect, useState } from 'react';
import { apiRequest } from '@/lib/api';

export default function ProfileForm() {
  const [form, setForm] = useState({
    email: '',
    gender: '',
    phoneNumber: ''
  });

  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Load existing profile
    const loadProfile = async () => {
      try {
        const data = await apiRequest('Auth/profile', {
          method: 'GET',
        });

        setForm({
          email: data.email,
          gender: data.gender,
          phoneNumber: data.phoneNumber,
        });

        setLoading(false);
      } catch (error: any) {
        alert('❌ Failed to load profile: ' + error.message);
        setLoading(false);
      }
    };

    loadProfile();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      await apiRequest('Auth/profile', {
        method: 'PUT',
        body: form,
      });

      alert('✅ Profile updated successfully!');
    } catch (error: any) {
      alert('❌ Update failed: ' + error.message);
    }
  };

  if (loading) return <p>Loading profile...</p>;

  return (
    <form onSubmit={handleSubmit} className="space-y-4 max-w-md bg-white p-6 rounded shadow">
      <h2 className="text-xl font-semibold mb-4">Update Profile</h2>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
        <input
          type="email"
          name="email"
          value={form.email}
          onChange={handleChange}
          className="w-full p-2 border border-gray-300 rounded"
        />
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">Gender</label>
        <select
          name="gender"
          value={form.gender}
          onChange={handleChange}
          className="w-full p-2 border border-gray-300 rounded"
        >
          <option value="">Select Gender</option>
          <option value="Male">Male</option>
          <option value="Female">Female</option>
        </select>
      </div>

      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1">Phone Number</label>
        <input
          type="tel"
          name="phoneNumber"
          value={form.phoneNumber}
          onChange={handleChange}
          className="w-full p-2 border border-gray-300 rounded"
        />
      </div>

      <button type="submit" className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700">
        Save Changes
      </button>
    </form>
  );
}
