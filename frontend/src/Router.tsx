import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import AuthPage from './AuthPage';
import App from './App';
import HomePage from './HomePage';
import WordsPage from './WordsPage';
import QuizPage from './QuizPage';
import StatsPage from './StatsPage';
import type { JSX } from 'react';

function RequireAuth({ children }: { children: JSX.Element }) {
  const token = localStorage.getItem('token');
  if (!token) return <Navigate to="/auth" />;
  return children;
}

export default function Router() {
  return (
    <BrowserRouter>
      <Routes>
  <Route path="/auth" element={<AuthPage />} />
  <Route path="/" element={<RequireAuth><HomePage /></RequireAuth>} />
  <Route path="/words" element={<RequireAuth><WordsPage /></RequireAuth>} />
  <Route path="/quiz" element={<RequireAuth><QuizPage /></RequireAuth>} />
  <Route path="/stats" element={<RequireAuth><StatsPage /></RequireAuth>} />
      </Routes>
    </BrowserRouter>
  );
}
