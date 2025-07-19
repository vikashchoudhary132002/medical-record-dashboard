import ProfileForm from './components/ProfileForm';
import FileUploadForm from './components/FileUploadForm';
import FileList from './components/FileList';

export default function DashboardPage() {
  return (
    <div className="min-h-screen bg-gray-100 p-4">
      <h1 className="text-3xl font-bold text-center text-blue-700 mb-8">
        🩺 Medical Record Dashboard
      </h1>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        {/* Left: Profile Section */}
        <div className="bg-white rounded-xl shadow-lg p-6 border border-gray-200">
          <h2 className="text-xl font-semibold mb-4 text-gray-700">👤 User Profile</h2>
          <ProfileForm />
        </div>

        {/* Right: File Upload Section */}
        <div className="bg-white rounded-xl shadow-lg p-6 border border-gray-200">
          <h2 className="text-xl font-semibold mb-4 text-gray-700">📤 Upload Medical File</h2>
          <FileUploadForm/>
        </div>
      </div>

      {/* Bottom: Uploaded Files Preview */}
      <div className="bg-white rounded-xl shadow-lg p-6 mt-6 border border-gray-200">
        <h2 className="text-xl font-semibold mb-4 text-gray-700">📁 Uploaded Files</h2>
        <FileList/>
      </div>
    </div>
  );
}
