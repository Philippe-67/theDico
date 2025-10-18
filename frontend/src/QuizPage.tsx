import { useState } from 'react';

interface QuizQuestion {
  wordId: string;
  sourceWord: string;
  targetWord: string;
  grammaticalClass?: string;
  semanticFamily?: string;
  userAnswer?: string;
  isCorrect?: boolean;
}

interface Quiz {
  id: string;
  questions: QuizQuestion[];
  maxScore: number;
  score: number;
}

export default function QuizPage() {
  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const [answers, setAnswers] = useState<{ [id: string]: string }>({});
  const [score, setScore] = useState<number | null>(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [langPair, setLangPair] = useState({ source: 'fr', target: 'en' });
  const [questionCount, setQuestionCount] = useState(5);

  async function generateQuiz(e: React.FormEvent) {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await fetch('/api/quiz/generate', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify({ sourceLanguage: langPair.source, targetLanguage: langPair.target, questionCount })
      });
      if (!res.ok) throw new Error('Erreur génération quiz');
      setQuiz(await res.json());
      setAnswers({});
      setScore(null);
    } catch (e) {
      setError('Impossible de générer le quiz');
    } finally {
      setLoading(false);
    }
  }

  async function submitQuiz(e: React.FormEvent) {
    e.preventDefault();
    if (!quiz) return;
    setError('');
    setLoading(true);
    try {
      const res = await fetch(`/api/quiz/submit/${quiz.id}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(quiz.questions.map(q => ({ wordId: q.wordId, userAnswer: answers[q.wordId] || '' })))
      });
      if (!res.ok) throw new Error('Erreur soumission quiz');
      const result = await res.json();
      setScore(result.score);
    } catch (e) {
      setError('Impossible de soumettre le quiz');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="quiz-page">
      <h2>Quiz de vocabulaire</h2>
      <form onSubmit={generateQuiz} style={{ marginBottom: 20 }}>
        <label>
          Langue source :
          <select value={langPair.source} onChange={e => setLangPair(l => ({ ...l, source: e.target.value }))}>
            <option value="fr">Français</option>
          </select>
        </label>
        <label style={{ marginLeft: 10 }}>
          Langue cible :
          <select value={langPair.target} onChange={e => setLangPair(l => ({ ...l, target: e.target.value }))}>
            <option value="en">Anglais</option>
          </select>
        </label>
        <label style={{ marginLeft: 10 }}>
          Nombre de questions :
          <input type="number" min={1} max={20} value={questionCount} onChange={e => setQuestionCount(Number(e.target.value))} />
        </label>
        <button type="submit" disabled={loading}>Générer le quiz</button>
      </form>
      {error && <div style={{ color: 'red' }}>{error}</div>}
      {quiz && (
        <form onSubmit={submitQuiz}>
          <ul>
            {quiz.questions.map(q => (
              <li key={q.wordId}>
                <b>{q.sourceWord}</b> <i>({q.grammaticalClass})</i> [{q.semanticFamily}]<br />
                Traduction : <input value={answers[q.wordId] || ''} onChange={e => setAnswers(a => ({ ...a, [q.wordId]: e.target.value }))} required />
              </li>
            ))}
          </ul>
          <button type="submit" disabled={loading}>Soumettre le quiz</button>
        </form>
      )}
      {score !== null && quiz && (
        <div style={{ marginTop: 20 }}>
          <b>Score : {score} / {quiz.maxScore}</b>
        </div>
      )}
    </div>
  );
}
