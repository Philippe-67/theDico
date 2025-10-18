import { Link } from 'react-router-dom';

export default function HomePage() {
  return (
    <div className="home-page">
      <h2>Bienvenue sur le Dico !</h2>
      <p>Choisissez une activité :</p>
      <ul>
        <li><Link to="/words">Gérer mes mots</Link></li>
        <li><Link to="/quiz">Quiz de vocabulaire</Link></li>
        <li><Link to="/stats">Voir mes statistiques</Link></li>
        <li><Link to="/auth" onClick={() => { localStorage.removeItem('token'); }}>Déconnexion</Link></li>
      </ul>
    </div>
  );
}
