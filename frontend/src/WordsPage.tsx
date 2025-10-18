import { useEffect, useState } from 'react';

interface Word {
  id: string;
  sourceWord: string;
  targetWord: string;
  grammaticalClass?: string;
  semanticFamily?: string;
  sourceLanguage?: string;
  targetLanguage?: string;
}

export default function WordsPage() {
  const [words, setWords] = useState<Word[]>([]);
  const [error, setError] = useState('');
  const [newWord, setNewWord] = useState({ sourceWord: '', targetWord: '', grammaticalClass: '', semanticFamily: '', sourceLanguage: 'fr', targetLanguage: 'en' });
  const [editId, setEditId] = useState<string | null>(null);
  const [editWord, setEditWord] = useState({ sourceWord: '', targetWord: '', grammaticalClass: '', semanticFamily: '', sourceLanguage: 'fr', targetLanguage: 'en' });
  const [filter, setFilter] = useState({ grammaticalClass: '', semanticFamily: '' });

  async function fetchWords() {
    setError('');
    let url = '/api/words';
    const params = [];
    if (filter.grammaticalClass) params.push(`grammaticalClass=${encodeURIComponent(filter.grammaticalClass)}`);
    if (filter.semanticFamily) params.push(`semanticFamily=${encodeURIComponent(filter.semanticFamily)}`);
    if (params.length) url = `/api/words/filter?${params.join('&')}`;
    try {
      const res = await fetch(url, {
        headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
      });
      if (!res.ok) throw new Error('Erreur chargement');
      setWords(await res.json());
    } catch (e) {
      setError('Impossible de charger les mots');
    }
  }

  useEffect(() => { fetchWords(); }, []);

  async function handleAdd(e: React.FormEvent) {
    e.preventDefault();
    setError('');
    try {
      const res = await fetch('/api/words', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(newWord)
      });
      if (!res.ok) throw new Error('Erreur ajout');
      setNewWord({ sourceWord: '', targetWord: '', grammaticalClass: '', semanticFamily: '', sourceLanguage: 'fr', targetLanguage: 'en' });
      fetchWords();
    } catch (e) {
      setError('Impossible d\'ajouter le mot');
    }
  }

  async function handleDelete(id: string) {
    setError('');
    try {
      const res = await fetch(`/api/words/${id}`, {
        method: 'DELETE',
        headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
      });
      if (!res.ok) throw new Error('Erreur suppression');
      fetchWords();
    } catch (e) {
      setError('Impossible de supprimer le mot');
    }
  }

  function startEdit(word: Word) {
    setEditId(word.id);
    setEditWord({
      sourceWord: word.sourceWord,
      targetWord: word.targetWord,
      grammaticalClass: word.grammaticalClass || '',
      semanticFamily: word.semanticFamily || '',
      sourceLanguage: word.sourceLanguage || 'fr',
      targetLanguage: word.targetLanguage || 'en'
    });
  }

  async function handleEdit(e: React.FormEvent) {
    e.preventDefault();
    if (!editId) return;
    setError('');
    try {
      const res = await fetch(`/api/words/${editId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(editWord)
      });
      if (!res.ok) throw new Error('Erreur édition');
      setEditId(null);
      fetchWords();
    } catch (e) {
      setError('Impossible de modifier le mot');
    }
  }

  return (
    <div className="words-page">
      <h2>Mes mots de vocabulaire</h2>
      <form onSubmit={handleAdd} style={{ marginBottom: 20 }}>
        <input placeholder="Mot français" value={newWord.sourceWord} onChange={e => setNewWord(w => ({ ...w, sourceWord: e.target.value }))} required />
        <input placeholder="Traduction anglaise" value={newWord.targetWord} onChange={e => setNewWord(w => ({ ...w, targetWord: e.target.value }))} required />
        <input placeholder="Classe grammaticale" value={newWord.grammaticalClass} onChange={e => setNewWord(w => ({ ...w, grammaticalClass: e.target.value }))} />
        <input placeholder="Famille sémantique" value={newWord.semanticFamily} onChange={e => setNewWord(w => ({ ...w, semanticFamily: e.target.value }))} />
        <button type="submit">Ajouter</button>
      </form>

      <form style={{ marginBottom: 20 }} onSubmit={e => { e.preventDefault(); fetchWords(); }}>
        <input placeholder="Filtrer par classe grammaticale" value={filter.grammaticalClass} onChange={e => setFilter(f => ({ ...f, grammaticalClass: e.target.value }))} />
        <input placeholder="Filtrer par famille sémantique" value={filter.semanticFamily} onChange={e => setFilter(f => ({ ...f, semanticFamily: e.target.value }))} />
        <button type="submit">Filtrer</button>
        <button type="button" onClick={() => { setFilter({ grammaticalClass: '', semanticFamily: '' }); fetchWords(); }} style={{ marginLeft: 10 }}>Réinitialiser</button>
      </form>

      {error && <div style={{ color: 'red' }}>{error}</div>}
      <ul>
        {words.map(word => (
          <li key={word.id}>
            {editId === word.id ? (
              <form onSubmit={handleEdit} style={{ display: 'inline' }}>
                <input value={editWord.sourceWord} onChange={e => setEditWord(w => ({ ...w, sourceWord: e.target.value }))} required />
                <input value={editWord.targetWord} onChange={e => setEditWord(w => ({ ...w, targetWord: e.target.value }))} required />
                <input value={editWord.grammaticalClass} onChange={e => setEditWord(w => ({ ...w, grammaticalClass: e.target.value }))} />
                <input value={editWord.semanticFamily} onChange={e => setEditWord(w => ({ ...w, semanticFamily: e.target.value }))} />
                <button type="submit">Valider</button>
                <button type="button" onClick={() => setEditId(null)}>Annuler</button>
              </form>
            ) : (
              <>
                <b>{word.sourceWord}</b> → {word.targetWord} <i>({word.grammaticalClass})</i> [{word.semanticFamily}]
                <button onClick={() => handleDelete(word.id)} style={{ marginLeft: 10 }}>Supprimer</button>
                <button onClick={() => startEdit(word)} style={{ marginLeft: 10 }}>Éditer</button>
              </>
            )}
          </li>
        ))}
      </ul>
    </div>
  );
}
