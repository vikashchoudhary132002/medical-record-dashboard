'use client';
import { useState } from 'react';
import { apiRequest } from '@/lib/api';

export default function UploadFileForm() {
  const [file, setFile] = useState<File | null>(null);
  const [fileType, setFileType] = useState('');
  const [fileName, setFileName] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!file || !fileType || !fileName) {
      alert("Please fill all fields.");
      return;
    }

    const formData = new FormData();
    formData.append('File', file);
    formData.append('FileType', fileType);
    formData.append('FileName', fileName);

    try {
      const response = await fetch(`${process.env.NEXT_PUBLIC_API_BASE_URL}Files/upload`, {
        method: 'POST',
        credentials: 'include', // Needed if session is later fixed
        body: formData,
      });

      if (!response.ok) {
        const err = await response.text();
        throw new Error(err);
      }

      const result = await response.json();
      alert(`✅ File uploaded: ${result.fileName}`);
    } catch (err: any) {
      alert(`❌ Failed: ${err.message}`);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4 bg-white p-4 rounded shadow max-w-md mx-auto mt-8">
      <h2 className="text-xl font-semibold">Upload Medical File</h2>

      <input
        type="text"
        placeholder="File Name"
        className="w-full border p-2 rounded"
        value={fileName}
        onChange={e => setFileName(e.target.value)}
      />

      <select
        className="w-full border p-2 rounded"
        value={fileType}
        onChange={e => setFileType(e.target.value)}
      >
        <option value="">Select File Type</option>
        <option value="Medical Report">Medical Report</option>
        <option value="X-ray">X-ray</option>
        <option value="Prescription">Prescription</option>
        <option value="Lab Result">Lab Result</option>
      </select>

      <input
        type="file"
        className="w-full"
        onChange={e => setFile(e.target.files?.[0] || null)}
      />

      <button type="submit" className="bg-blue-600 text-white py-2 px-4 rounded hover:bg-blue-700">
        Upload
      </button>
    </form>
  );
}
