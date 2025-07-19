'use client';
import { apiRequest } from '@/lib/api';
import Link from 'next/link';
import { useState } from "react";

export default function SignupPage() {
  const [form, setForm] = useState({
    fullName: "",
    email: "",
    gender: "",
    phone: "",
    password: "",
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    console.log('📨 Submitting Register Form', form); 
    try {
      await apiRequest('Auth/register', {
        method: 'POST',
        body: {
          fullName: form.fullName,
          email: form.email,
          gender: form.gender,
          phoneNumber: form.phone,
          password: form.password,
        },
      });
  debugger
      alert('✅ Registered successfully! Please log in.');
      window.location.href = '/login';
    } catch (error: any) {
      alert('❌ ' + error.message); // from your backend (e.g. "Email already exists")
    }
  };
  

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100 px-4">
      <form onSubmit={handleSubmit} className="bg-white p-8 rounded-md shadow-md w-full max-w-md space-y-4">
        <h2 className="text-2xl font-semibold text-center">Sign Up</h2>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Full Name</label>
          <input
            type="text"
            name="fullName"
            placeholder="Enter your full name"
            className="w-full p-2 border border-gray-300 rounded"
            onChange={handleChange}
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
          <input
            type="email"
            name="email"
            placeholder="Enter your email"
            className="w-full p-2 border border-gray-300 rounded"
            onChange={handleChange}
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Gender</label>
          <select
            name="gender"
            className="w-full p-2 border border-gray-300 rounded"
            onChange={handleChange}
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
            name="phone"
            placeholder="Enter your phone number"
            className="w-full p-2 border border-gray-300 rounded"
            onChange={handleChange}
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Password</label>
          <input
            type="password"
            name="password"
            placeholder="Enter your password"
            className="w-full p-2 border border-gray-300 rounded"
            onChange={handleChange}
          />
        </div>

        <button type="submit" className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700">
          Sign Up
        </button>
        <p className="text-sm text-center text-gray-600 mt-4">
          Already have an account?{' '}
          <Link href="/login" className="text-blue-600 hover:underline">
            Log in
          </Link>
        </p>
      </form>
    </div>
  );
}
