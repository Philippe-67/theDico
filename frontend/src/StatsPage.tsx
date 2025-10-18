import { useEffect, useState } from 'react';

interface QuizProgression {
  quizId: string;
  date: string;
  score: number;
  maxScore: number;
  languagePair?: string;
}

interface UserStats {
  wordCount: number;
  quizCount: number;
  progression: QuizProgression[];
}

export default function StatsPage() {
  const [stats, setStats] = useState<UserStats | null>(null);
  const [error, setError] = useState('');

  useEffect(() => {
    async function fetchStats() {
      setError('');
      try {
        const res = await fetch('/api/stats', {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
        });
        if (!res.ok) throw new Error('Erreur chargement stats');
        setStats(await res.json());
      } catch (e) {
        setError('Impossible de charger les statistiques');
      }
    }
    fetchStats();
  }, []);

  return (
    <div className="stats-page">
      <h2>Mes statistiques</h2>
      {error && <div style={{ color: 'red' }}>{error}</div>}
      {stats && (
        <div>
          <p><b>Nombre de mots :</b> {stats.wordCount}</p>
          <p><b>Nombre de quiz :</b> {stats.quizCount}</p>
          <h3>Progression des quiz</h3>
          <table border={1} cellPadding={5} style={{ marginTop: 10 }}>
            <thead>
              <tr>
                <th>Date</th>
                <th>Score</th>
                <th>Max</th>
                <th>Langue</th>
              </tr>
            </thead>
            <tbody>
              {stats.progression.map(p => (
                <tr key={p.quizId}>
                  <td>{new Date(p.date).toLocaleString()}</td>
                  <td>{p.score}</td>
                  <td>{p.maxScore}</td>
                  <td>{p.languagePair}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
