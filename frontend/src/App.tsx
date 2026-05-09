import { Navigate, Route, Routes } from "react-router-dom";
import { Toaster } from "sonner";
import { AuthProvider } from "./features/auth/AuthContext";
import { LandingPage } from "./pages/LandingPage";
import { NotFoundPage } from "./pages/NotFoundPage";
import { TasksPage } from "./pages/TasksPage";
import { Navbar } from "./shared/components/Navbar";
import { ProtectedRoute } from "./shared/routes/ProtectedRoute";

function App() {
  return (
    <AuthProvider>
      <div className="flex min-h-screen flex-col">
        <Navbar />
        <Routes>
          <Route path="/" element={<LandingPage />} />
          <Route
            path="/tasks"
            element={
              <ProtectedRoute>
                <TasksPage />
              </ProtectedRoute>
            }
          />
          <Route path="/not-found" element={<NotFoundPage />} />
          <Route path="*" element={<Navigate to="/not-found" replace />} />
        </Routes>
      </div>
      <Toaster richColors position="top-right" closeButton visibleToasts={3} />
    </AuthProvider>
  );
}

export default App;
