'use client';

import { useEffect, useState } from 'react';

type UploadedFile = {
  id: number;
  fileName: string;
  fileType: string;
  uploadDate: string;
  fileSize: number;
  fileUrl: string;
};

export default function FileList() {
  const [files, setFiles] = useState<UploadedFile[]>([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  // Fetch uploaded files
  const fetchFiles = async () => {
    setLoading(true);
    try {
      const response = await fetch(`${process.env.NEXT_PUBLIC_API_BASE_URL}Files`, {
        credentials: 'include',
      });

      if (!response.ok) {
        const err = await response.text();
        throw new Error(err);
      }

      const data = await response.json();
      setFiles(data);
    } catch (err: any) {
      setError(err.message || 'Failed to load files');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchFiles();
  }, []);

  const handleView = (url: string) => {
    window.open(url, '_blank');
  };

  const handleDelete = async (fileId: number) => {
    const confirmed = confirm('Are you sure you want to delete this file?');
    if (!confirmed) return;

    try {
      const response = await fetch(`${process.env.NEXT_PUBLIC_API_BASE_URL}Files/${fileId}`, {
        method: 'DELETE',
        credentials: 'include',
      });

      if (!response.ok && response.status !== 204) {
        const err = await response.text();
        throw new Error(err);
      }

      // Remove from local state
      setFiles(prev => prev.filter(file => file.id !== fileId));
    } catch (err: any) {
      alert(`❌ Failed to delete: ${err.message}`);
    }
  };

  return (
    <div className="p-6">
      <h2 className="text-2xl font-semibold mb-4">Your Uploaded Files</h2>

      {loading && <p className="text-sm text-gray-500 italic">Loading...</p>}
      {error && <p className="text-red-600">❌ {error}</p>}

      {files.length === 0 && !loading && !error ? (
        <p className="text-gray-500 italic">No files uploaded yet.</p>
      ) : (
        <div className="space-y-4">
          {files.map(file => (
            <div
              key={file.id}
              className="flex justify-between items-center p-4 border rounded bg-gray-50 hover:shadow-sm"
            >
              <div>
                <p className="font-semibold text-gray-800">{file.fileName}</p>
                <p className="text-sm text-gray-500">{file.fileType}</p>
              </div>
              <div className="space-x-2">
                <button
                  onClick={() => handleView(file.fileUrl)}
                  className="px-3 py-1 bg-blue-500 text-white text-sm rounded hover:bg-blue-600"
                >
                  View
                </button>
                <button
                  onClick={() => handleDelete(file.id)}
                  className="px-3 py-1 bg-red-500 text-white text-sm rounded hover:bg-red-600"
                >
                  Delete
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
